using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class ExaminationScheduleService : DomainService<ExaminationSchedules, SearchExaminationSchedule>, IExaminationScheduleService
    {
        public ExaminationScheduleService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationSchedule_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchExaminationSchedule baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@DoctorId", baseSearch.DoctorId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        public override async Task<bool> CreateAsync(ExaminationSchedules item)
        {
            bool result = false;
            if (item != null)
            {
                // Lưu thông tin bác sĩ
                await unitOfWork.Repository<ExaminationSchedules>().CreateAsync(item);
                await unitOfWork.SaveAsync();
                // Cập nhật thông tin chi tiết ca
                if (item.ExaminationScheduleDetails != null && item.ExaminationScheduleDetails.Any())
                {
                    foreach (var examinationScheduleDetail in item.ExaminationScheduleDetails)
                    {
                        examinationScheduleDetail.ScheduleId = item.Id;
                        examinationScheduleDetail.Created = DateTime.Now;
                        await unitOfWork.Repository<ExaminationScheduleDetails>().CreateAsync(examinationScheduleDetail);
                    }
                }
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        public override async Task<bool> UpdateAsync(ExaminationSchedules item)
        {
            bool result = false;
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();

            if (exists != null)
            {
                exists = mapper.Map<ExaminationSchedules>(item);
                unitOfWork.Repository<ExaminationSchedules>().Update(exists);

                // Cập nhật thông tin chi tiết ca
                if (item.ExaminationScheduleDetails != null && item.ExaminationScheduleDetails.Any())
                {
                    foreach (var examinationScheduleDetail in item.ExaminationScheduleDetails)
                    {
                        var existExaminationScheduleDetail = await unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                                                             .AsNoTracking()
                                                             .Where(e => e.Id == examinationScheduleDetail.Id && !e.Deleted)
                                                             .FirstOrDefaultAsync();
                        if (existExaminationScheduleDetail != null)
                        {
                            existExaminationScheduleDetail = mapper.Map<ExaminationScheduleDetails>(examinationScheduleDetail);
                            existExaminationScheduleDetail.ScheduleId = exists.Id;
                            existExaminationScheduleDetail.Updated = DateTime.Now;
                            unitOfWork.Repository<ExaminationScheduleDetails>().Update(examinationScheduleDetail);
                        }
                        else
                        {
                            examinationScheduleDetail.ScheduleId = exists.Id;
                            examinationScheduleDetail.Created = DateTime.Now;
                            await unitOfWork.Repository<ExaminationScheduleDetails>().CreateAsync(examinationScheduleDetail);
                        }
                    }
                }
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }


        public override async Task<string> GetExistItemMessage(ExaminationSchedules item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistSchedule = await Queryable.AnyAsync(x => !x.Deleted
            && x.DoctorId == item.Id
            && x.Id != item.Id
            && x.ExaminationDate.Date == item.ExaminationDate.Date);
            if (isExistSchedule)
                messages.Add(string.Format("Bác sĩ đã có lịch tại ngày {0}", item.ExaminationDate.ToString("dd/MM/yyyy")));
            // Kiểm tra có dk trùng ca khám không?
            if (item.ExaminationScheduleDetails != null && item.ExaminationScheduleDetails.Any())
            {
                List<int> configTimeExaminationIds = new List<int>();
                foreach (var detail in item.ExaminationScheduleDetails)
                {
                    if (detail.Deleted)
                        continue;

                    if (configTimeExaminationIds.Any(x => x == detail.ConfigTimeExaminationId))
                    {
                        var configTimeExaminationInfo = await unitOfWork.Repository<ConfigTimeExaminations>().GetQueryable().Where(e => e.Id == detail.ConfigTimeExaminationId).FirstOrDefaultAsync();
                        if (configTimeExaminationInfo != null)
                        {
                            messages.Add(string.Format("Ca {0} đã được đăng ký", configTimeExaminationInfo.Value));
                            continue;
                        }
                    }
                    configTimeExaminationIds.Add(detail.ConfigTimeExaminationId);
                }
            }
            if (messages.Any())
                result = string.Join("; ", messages);
            return result;
        }

    }
}
