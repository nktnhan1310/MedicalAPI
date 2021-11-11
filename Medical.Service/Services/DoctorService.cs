using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.AspNetCore.SignalR;
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
    public class DoctorService : DomainService<Doctors, SearchDoctor>, IDoctorService
    {
        public DoctorService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Doctor_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchDoctor baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@DegreeId", baseSearch.DegreeId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@SpecialListTypeId", baseSearch.SpecialListTypeId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Tạo mới thông tin bác sĩ
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Doctors item)
        {
            bool result = false;
            if (item != null)
            {
                // Lưu thông tin bác sĩ
                await unitOfWork.Repository<Doctors>().CreateAsync(item);
                await unitOfWork.SaveAsync();
                // Cập nhật thông tin chuyên khoa của bác sĩ
                if (item.DoctorDetails != null && item.DoctorDetails.Any())
                {
                    foreach (var doctorDetail in item.DoctorDetails)
                    {
                        doctorDetail.DoctorId = item.Id;
                        doctorDetail.Created = DateTime.Now;
                        doctorDetail.Active = true;
                        doctorDetail.Id = 0;
                        await unitOfWork.Repository<DoctorDetails>().CreateAsync(doctorDetail);
                    }
                }
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin bác sĩ
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Doctors item)
        {
            bool result = false;
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();

            if (exists != null)
            {
                exists = mapper.Map<Doctors>(item);
                unitOfWork.Repository<Doctors>().Update(exists);

                // Cập nhật thông tin chuyên khoa của bác sĩ
                if (item.DoctorDetails != null && item.DoctorDetails.Any())
                {
                    foreach (var doctorDetail in item.DoctorDetails)
                    {
                        var existDoctorDetail = await unitOfWork.Repository<DoctorDetails>().GetQueryable()
                                                             .AsNoTracking()
                                                             .Where(e => e.Id == doctorDetail.Id && !e.Deleted)
                                                             .FirstOrDefaultAsync();
                        if (existDoctorDetail != null)
                        {
                            existDoctorDetail = mapper.Map<DoctorDetails>(doctorDetail);
                            existDoctorDetail.DoctorId = exists.Id;
                            existDoctorDetail.Updated = DateTime.Now;
                            unitOfWork.Repository<DoctorDetails>().Update(existDoctorDetail);
                        }
                        else
                        {
                            doctorDetail.DoctorId = exists.Id;
                            doctorDetail.Created = DateTime.Now;
                            doctorDetail.Id = 0;
                            await unitOfWork.Repository<DoctorDetails>().CreateAsync(doctorDetail);
                        }
                    }
                }
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra bác sĩ đã tồn tại chưa?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(Doctors item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            if (Queryable.Any(e => !e.Deleted && e.HospitalId == item.HospitalId && e.Id != item.Id && e.Code == item.Code))
                return "Mã bác sĩ đã tồn tại";
            if(Queryable.Any(e => !e.Deleted && e.UserId.HasValue && e.HospitalId == item.HospitalId && e.Id != item.Id && e.UserId == item.UserId))
                return "Tài khoản đã được chọn";
            if (item.DoctorDetails != null && item.DoctorDetails.Any())
            {
                List<int> specialistTypeIds = new List<int>();
                foreach (var doctorDetail in item.DoctorDetails)
                {
                    if (doctorDetail.Deleted)
                        continue;
                    var specialistType = await this.unitOfWork.Repository<SpecialistTypes>().GetQueryable().Where(e => e.Id == doctorDetail.SpecialistTypeId).FirstOrDefaultAsync();
                    if (specialistTypeIds.Contains(doctorDetail.SpecialistTypeId))
                    {
                        messages.Add(string.Format("Chuyên khoa {0} của bác sĩ đã tồn tại", specialistType.Name));
                        continue;
                    }
                    specialistTypeIds.Add(doctorDetail.SpecialistTypeId);
                }
            }
            if (messages.Any())
                result = string.Join("; ", messages);
            return result;
        }

        /// <summary>
        /// Lấy danh sách bác sĩ có lịch trực
        /// </summary>
        /// <param name="searchExaminationScheduleDetailV2"></param>
        /// <returns></returns>
        public async Task<PagedList<DoctorDetails>> GetListDoctorExaminations(SearchExaminationScheduleDetailV2 searchExaminationScheduleDetailV2)
        {
            PagedList<DoctorDetails> pagedList = new PagedList<DoctorDetails>();
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", searchExaminationScheduleDetailV2.PageIndex),
                new SqlParameter("@PageSize", searchExaminationScheduleDetailV2.PageSize),
                new SqlParameter("@HospitalId", searchExaminationScheduleDetailV2.HospitalId),
                new SqlParameter("@DoctorId", searchExaminationScheduleDetailV2.DoctorId),
                new SqlParameter("@SpecialistTypeId", searchExaminationScheduleDetailV2.SpecialistTypeId),
                new SqlParameter("@Gender", searchExaminationScheduleDetailV2.Gender),
                new SqlParameter("@DegreeTypeId", searchExaminationScheduleDetailV2.DegreeTypeId),
                new SqlParameter("@SessionId", searchExaminationScheduleDetailV2.SessionId),
                new SqlParameter("@DayOfWeek", searchExaminationScheduleDetailV2.DayOfWeek),
                new SqlParameter("@ExaminationDate", searchExaminationScheduleDetailV2.ExaminationDate),
                new SqlParameter("@ExaminationScheduleDetailId", searchExaminationScheduleDetailV2.ExaminationScheduleDetailId),
                new SqlParameter("@DoctorTypeId", searchExaminationScheduleDetailV2.DoctorTypeId),
                new SqlParameter("@OrderBy", searchExaminationScheduleDetailV2.OrderBy),
                new SqlParameter("@SearchContent", searchExaminationScheduleDetailV2.SearchContent),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };

            pagedList = await unitOfWork.Repository<DoctorDetails>().ExcuteQueryPagingAsync("DocTorDetail_GetInfo", sqlParameters);
            pagedList.PageSize = searchExaminationScheduleDetailV2.PageSize;
            pagedList.PageIndex = searchExaminationScheduleDetailV2.PageIndex;

            return pagedList;
        }


    }
}
