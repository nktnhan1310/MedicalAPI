using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class HospitalService : DomainService<Hospitals, SearchHospital>, IHospitalService
    {
        private readonly IConfiguration configuration;
<<<<<<< HEAD

        public HospitalService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper
            , IConfiguration configuration
            ) : base(unitOfWork, medicalDbContext, mapper)
=======
        public HospitalService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IServiceProvider serviceProvider) : base(unitOfWork, mapper)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        {
            this.configuration = configuration;
        }

        protected override string GetStoreProcName()
        {
            return "Hospital_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchHospital baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@HospitalTypeId", baseSearch.HospitalTypeId),
                new SqlParameter("@HospitalFunctionTypeId", baseSearch.HospitalFunctionTypeId),
                new SqlParameter("@Email", string.IsNullOrEmpty(baseSearch.Email) ? DBNull.Value : (object)baseSearch.Email),
                new SqlParameter("@Phone", baseSearch.Phone),
                new SqlParameter("@TotalVisitNo", baseSearch.TotalVisitNo),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Cập nhật thông tin bệnh viện
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Hospitals item)
        {
            if (item == null) throw new AppException("Vui lòng chọn thông tin item");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
<<<<<<< HEAD
                try
=======
                await unitOfWork.Repository<Hospitals>().CreateAsync(item);
                //await unitOfWork.SaveAsync();
                // Cập nhật thông tin dịch vụ bệnh viện
                if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                {
                    await unitOfWork.Repository<Hospitals>().CreateAsync(item);
                    //await unitOfWork.SaveAsync();
                    // Cập nhật thông tin dịch vụ bệnh viện
                    if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
                    {
<<<<<<< HEAD
                        foreach (var serviceTypeMappingHospital in item.ServiceTypeMappingHospitals)
                        {
                            serviceTypeMappingHospital.HospitalId = item.Id;
                            serviceTypeMappingHospital.Created = DateTime.Now;
                            serviceTypeMappingHospital.CreatedBy = item.CreatedBy;
                            serviceTypeMappingHospital.Active = true;
                            serviceTypeMappingHospital.Id = 0;
                            unitOfWork.Repository<ServiceTypeMappingHospital>().Create(serviceTypeMappingHospital);
                        }
                    }
                    if (item.ChannelIds != null && item.ChannelIds.Any())
=======
                        serviceTypeMappingHospital.HospitalId = item.Id;
                        serviceTypeMappingHospital.Created = DateTime.Now;
                        serviceTypeMappingHospital.CreatedBy = item.CreatedBy;
                        serviceTypeMappingHospital.Active = true;
                        serviceTypeMappingHospital.Id = 0;
                        unitOfWork.Repository<ServiceTypeMappingHospital>().Create(serviceTypeMappingHospital);
                    }
                }
                if (item.ChannelIds != null && item.ChannelIds.Any())
                {
                    foreach (var channelId in item.ChannelIds)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                    {
                        foreach (var channelId in item.ChannelIds)
                        {
<<<<<<< HEAD
                            ChannelMappingHospital channelMappingHospital = new ChannelMappingHospital()
                            {
                                Created = DateTime.Now,
                                CreatedBy = item.CreatedBy,
                                Active = true,
                                Deleted = false,
                                ChannelId = channelId,
                                HospitalId = item.Id,
                                Id = 0
                            };
                            unitOfWork.Repository<ChannelMappingHospital>().Create(channelMappingHospital);
                        }
=======
                            Created = DateTime.Now,
                            CreatedBy = item.CreatedBy,
                            Active = true,
                            Deleted = false,
                            ChannelId = channelId,
                            HospitalId = item.Id,
                            Id = 0
                        };
                        unitOfWork.Repository<ChannelMappingHospital>().Create(channelMappingHospital);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                    }

                    // Cập nhật thông tin file
                    if (item.HospitalFiles != null && item.HospitalFiles.Any())
                    {
                        foreach (var hospitalFile in item.HospitalFiles)
                        {
                            hospitalFile.HospitalId = item.Id;
                            hospitalFile.Created = DateTime.Now;
                            hospitalFile.CreatedBy = item.CreatedBy;
                            hospitalFile.Active = true;
                            hospitalFile.Id = 0;
                            await unitOfWork.Repository<HospitalFiles>().CreateAsync(hospitalFile);
                        }
                    }
                    // Cập nhật thông ngân hàng liên kết
                    if (item.BankInfos != null && item.BankInfos.Any())
                    {
                        foreach (var bankInfo in item.BankInfos)
                        {
                            bankInfo.HospitalId = item.Id;
                            bankInfo.Created = DateTime.Now;
                            bankInfo.CreatedBy = item.CreatedBy;
                            bankInfo.Active = true;
                            bankInfo.Id = 0;
                            await unitOfWork.Repository<BankInfos>().CreateAsync(bankInfo);
                        }
                    }
                    await unitOfWork.SaveAsync();
                    string oldDataJson = JsonSerializer.Serialize<Hospitals>(item);

                    // Thêm lịch sử chỉnh sửa bệnh viện
                    HospitalHistories hospitalHistories = new HospitalHistories()
                    {
                        Created = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        OldHospitalDataJson = oldDataJson,
                        HospitalId = item.Id,
                        Active = true,
                        Deleted = false,
                    };
                    await this.unitOfWork.Repository<HospitalHistories>().CreateAsync(hospitalHistories);
                    await unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
<<<<<<< HEAD
=======
                await unitOfWork.SaveAsync();
                result = true;
                string oldDataJson = JsonSerializer.Serialize<Hospitals>(item);

                // Thêm lịch sử chỉnh sửa bệnh viện
                HospitalHistories hospitalHistories = new HospitalHistories()
                {
                    Created = DateTime.Now,
                    CreatedBy = item.CreatedBy,
                    OldHospitalDataJson = oldDataJson,
                    HospitalId = item.Id,
                    Active = true,
                    Deleted = false,
                };
                await this.unitOfWork.Repository<HospitalHistories>().CreateAsync(hospitalHistories);
                await unitOfWork.SaveAsync();

>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            }
        }

        /// <summary>
        /// Cập nhật thông tin bệnh viện
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Hospitals item)
        {
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();
<<<<<<< HEAD
            if (exists == null) throw new AppException("Thông tin bệnh viện không tồn tại");

                var currentMinutePerPatient = exists.MinutePerPatient;
            string oldDataJson = string.Empty;
            string newDataJson = string.Empty;

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
=======
            var currentMinutePerPatient = exists.MinutePerPatient;
            string oldDataJson = string.Empty;
            string newDataJson = string.Empty;
            if (exists != null)
            {
                oldDataJson = JsonSerializer.Serialize<Hospitals>(exists);
                exists = mapper.Map<Hospitals>(item);

                unitOfWork.Repository<Hospitals>().Update(exists);
                // Cập nhật thông tin dịch vụ
                if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                {
                    oldDataJson = JsonSerializer.Serialize<Hospitals>(exists);
                    exists = mapper.Map<Hospitals>(item);

                    unitOfWork.Repository<Hospitals>().Update(exists);
                    // Cập nhật thông tin dịch vụ
                    if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
                    {
                        foreach (var serviceTypeMappingHospital in item.ServiceTypeMappingHospitals)
                        {
<<<<<<< HEAD
                            var existServiceTypeMapping = await unitOfWork.Repository<ServiceTypeMappingHospital>().GetQueryable()
                                                                 .AsNoTracking()
                                                                 .Where(e => e.Id == serviceTypeMappingHospital.Id && !e.Deleted)
                                                                 .FirstOrDefaultAsync();
                            if (existServiceTypeMapping != null)
                            {
                                serviceTypeMappingHospital.Updated = DateTime.Now;
                                serviceTypeMappingHospital.UpdatedBy = item.UpdatedBy;
                                existServiceTypeMapping = mapper.Map<ServiceTypeMappingHospital>(serviceTypeMappingHospital);
                                unitOfWork.Repository<ServiceTypeMappingHospital>().Update(existServiceTypeMapping);
                            }
                            else
                            {
                                serviceTypeMappingHospital.HospitalId = item.Id;
                                serviceTypeMappingHospital.Created = DateTime.Now;
                                serviceTypeMappingHospital.CreatedBy = item.UpdatedBy;
                                serviceTypeMappingHospital.Id = 0;
                                serviceTypeMappingHospital.Active = true;
                                await unitOfWork.Repository<ServiceTypeMappingHospital>().CreateAsync(serviceTypeMappingHospital);
                            }
=======
                            serviceTypeMappingHospital.Updated = DateTime.Now;
                            serviceTypeMappingHospital.UpdatedBy = item.UpdatedBy;
                            existServiceTypeMapping = mapper.Map<ServiceTypeMappingHospital>(serviceTypeMappingHospital);
                            unitOfWork.Repository<ServiceTypeMappingHospital>().Update(existServiceTypeMapping);
                        }
                        else
                        {
                            serviceTypeMappingHospital.HospitalId = item.Id;
                            serviceTypeMappingHospital.Created = DateTime.Now;
                            serviceTypeMappingHospital.CreatedBy = item.UpdatedBy;
                            serviceTypeMappingHospital.Id = 0;
                            serviceTypeMappingHospital.Active = true;
                            await unitOfWork.Repository<ServiceTypeMappingHospital>().CreateAsync(serviceTypeMappingHospital);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                        }
                    }

                    // Cập nhật thông tin kênh thanh toán
                    if (item.ChannelIds != null && item.ChannelIds.Any())
                    {
                        foreach (var channelId in item.ChannelIds)
                        {
                            var existChannelMappintHospital = await this.unitOfWork.Repository<ChannelMappingHospital>().GetQueryable()
                                .Where(e => e.ChannelId == channelId && e.HospitalId == exists.Id).FirstOrDefaultAsync();
                            if (existChannelMappintHospital != null)
                            {
                                existChannelMappintHospital.ChannelId = channelId;
                                existChannelMappintHospital.HospitalId = exists.Id;
                                existChannelMappintHospital.Updated = DateTime.Now;
                                this.unitOfWork.Repository<ChannelMappingHospital>().Update(existChannelMappintHospital);
                            }
                            else
                            {
                                ChannelMappingHospital channelMappingHospital = new ChannelMappingHospital()
                                {
                                    Created = DateTime.Now,
                                    CreatedBy = item.CreatedBy,
                                    ChannelId = channelId,
                                    HospitalId = exists.Id,
                                    Active = true,
                                    Deleted = false,
                                    Id = 0
                                };
                                await this.unitOfWork.Repository<ChannelMappingHospital>().CreateAsync(channelMappingHospital);
                            }
                        }

                        // Kiểm tra những item không có trong role chọn => Xóa đi
                        var existGroupOlds = await this.unitOfWork.Repository<ChannelMappingHospital>().GetQueryable().Where(e => !item.ChannelIds.Contains(e.ChannelId) && e.HospitalId == exists.Id).ToListAsync();
                        if (existGroupOlds != null)
                        {
                            foreach (var existGroupOld in existGroupOlds)
                            {
                                this.unitOfWork.Repository<ChannelMappingHospital>().Delete(existGroupOld);
                            }
                        }

                    }
                    else
                    {
                        var channelMappingHospitals = await this.unitOfWork.Repository<ChannelMappingHospital>().GetQueryable()
                            .Where(e => e.HospitalId == exists.Id).ToListAsync();
                        if (channelMappingHospitals != null && channelMappingHospitals.Any())
                        {
<<<<<<< HEAD
                            foreach (var channelMappingHospital in channelMappingHospitals)
                            {

                                this.unitOfWork.Repository<ChannelMappingHospital>().Delete(channelMappingHospital);
                            }
=======

                            this.unitOfWork.Repository<ChannelMappingHospital>().Delete(channelMappingHospital);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                        }
                    }
                    // Cập nhật thông tin file của bệnh viện
                    if (item.HospitalFiles != null && item.HospitalFiles.Any())
                    {
                        foreach (var hospitalFile in item.HospitalFiles)
                        {
                            var existHospitalFile = await unitOfWork.Repository<HospitalFiles>().GetQueryable()
                                                                 .AsNoTracking()
                                                                 .Where(e => e.Id == hospitalFile.Id && !e.Deleted)
                                                                 .FirstOrDefaultAsync();
                            if (existHospitalFile != null)
                            {
                                hospitalFile.Updated = DateTime.Now;
                                hospitalFile.UpdatedBy = item.UpdatedBy;
                                existHospitalFile = mapper.Map<HospitalFiles>(hospitalFile);
                                unitOfWork.Repository<HospitalFiles>().Update(existHospitalFile);
                            }
                            else
                            {
                                hospitalFile.HospitalId = item.Id;
                                hospitalFile.Created = DateTime.Now;
                                hospitalFile.CreatedBy = item.UpdatedBy;
                                hospitalFile.Id = 0;
                                await unitOfWork.Repository<HospitalFiles>().CreateAsync(hospitalFile);
                            }
                        }
                    }

                    // Cập nhật thông tin ngân hàng liên kết
                    if (item.BankInfos != null && item.BankInfos.Any())
                    {
                        foreach (var bankInfo in item.BankInfos)
                        {
                            var existBankInfo = await unitOfWork.Repository<BankInfos>().GetQueryable()
                                                                 .AsNoTracking()
                                                                 .Where(e => e.Id == bankInfo.Id && !e.Deleted)
                                                                 .FirstOrDefaultAsync();
                            if (existBankInfo != null)
                            {
                                bankInfo.Updated = DateTime.Now;
                                bankInfo.UpdatedBy = item.UpdatedBy;
                                existBankInfo = mapper.Map<BankInfos>(bankInfo);
                                unitOfWork.Repository<BankInfos>().Update(existBankInfo);
                            }
                            else
                            {
                                bankInfo.HospitalId = item.Id;
                                bankInfo.Created = DateTime.Now;
                                bankInfo.CreatedBy = item.UpdatedBy;
                                bankInfo.Id = 0;
                                await unitOfWork.Repository<BankInfos>().CreateAsync(bankInfo);
                            }
                        }
                    }
                    await unitOfWork.SaveAsync();
                    // Thêm lịch sử cập nhật lịch sử
                    newDataJson = JsonSerializer.Serialize<Hospitals>(exists);
                    HospitalHistories hospitalHistories = new HospitalHistories()
                    {
                        Created = DateTime.Now,
                        CreatedBy = item.UpdatedBy,
                        HospitalId = item.Id,
                        Deleted = false,
                        Active = true,
                        OldHospitalDataJson = oldDataJson,
                        NewHospitalDataJson = newDataJson,
                    };
                    await this.unitOfWork.Repository<HospitalHistories>().CreateAsync(hospitalHistories);


                    // Cập nhật lại số lượng ca khám tối đa cho những lịch lớn hơn ngày hiện tại
                    if (item.MinutePerPatient != currentMinutePerPatient)
                        await UpdateExaminationSchedule(exists);

                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
<<<<<<< HEAD
=======
                await unitOfWork.SaveAsync();
                result = true;

                // Thêm lịch sử cập nhật lịch sử
                newDataJson = JsonSerializer.Serialize<Hospitals>(exists);
                HospitalHistories hospitalHistories = new HospitalHistories()
                {
                    Created = DateTime.Now,
                    CreatedBy = item.UpdatedBy,
                    HospitalId = item.Id,
                    Deleted = false,
                    Active = true,
                    OldHospitalDataJson = oldDataJson,
                    NewHospitalDataJson = newDataJson,
                };
                await this.unitOfWork.Repository<HospitalHistories>().CreateAsync(hospitalHistories);


                // Cập nhật lại số lượng ca khám tối đa cho những lịch lớn hơn ngày hiện tại
                if (item.MinutePerPatient != currentMinutePerPatient)
                    await UpdateExaminationSchedule(exists);

                await this.unitOfWork.SaveAsync();

>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            }
        }

        /// <summary>
        /// Check trùng mã bệnh viện
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(Hospitals item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;

            bool isExistItem = await this.Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.Code == item.Code);
            if (isExistItem)
            {
                messages.Add("Mã bệnh viện đã tồn tại");
            }
            // Kiểm tra trùng kênh đăng ký
            if (item.ChannelMappingHospitals != null && item.ChannelMappingHospitals.Any())
            {
                List<int> channelIds = new List<int>();
                foreach (var channelMappingHospital in item.ChannelMappingHospitals)
                {
                    if (channelIds.Any(x => x == channelMappingHospital.ChannelId))
                    {
                        var channelInfo = await this.unitOfWork.Repository<Channels>().GetQueryable().Where(e => e.Id == channelMappingHospital.ChannelId).FirstOrDefaultAsync();
                        if (channelInfo != null)
                            messages.Add(string.Format("Kênh đăng ký {0} đã tồn tại!", channelInfo.Name));
                        else messages.Add("Kênh đăng ký không tồn tại!");
                    }
                    channelIds.Add(channelMappingHospital.ChannelId);
                }
            }
            // Kiểm tra trùng dịch vụ
            if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
            {
                List<int> serviceTypeIds = new List<int>();
                foreach (var serviceTypeMappingHospital in item.ServiceTypeMappingHospitals)
                {
                    if (serviceTypeIds.Any(x => x == serviceTypeMappingHospital.ServiceTypeId))
                    {
                        var serviceTypeInfo = await this.unitOfWork.Repository<ServiceTypes>().GetQueryable().Where(e => e.Id == serviceTypeMappingHospital.ServiceTypeId).FirstOrDefaultAsync();
                        if (serviceTypeInfo != null)
                            messages.Add(string.Format("Dịch vụ {0} đã tồn tại!", serviceTypeInfo.Name));
                        else messages.Add("Dịch vụ không tồn tại!");
                    }
                    serviceTypeIds.Add(serviceTypeMappingHospital.ServiceTypeId);
                }
            }


            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

        /// <summary>
        /// Cập nhật lại tất cả lịch lớn hơn ngày hiện tại, có cấu hình theo số phút khám của bệnh viện
        /// </summary>
        /// <param name="hospitalInfo"></param>
        /// <returns></returns>
        private async Task UpdateExaminationSchedule(Hospitals hospitalInfo)
        {
            var examinationSchedules = await this.unitOfWork.Repository<ExaminationSchedules>().GetQueryable()
                .Where(e => !e.Deleted && e.Active && e.HospitalId == hospitalInfo.Id
                && e.ExaminationDate >= DateTime.Now
                && e.IsUseHospitalConfig
                )
                .ToListAsync();
            // Lấy ra thông tin tất cả buổi cấu hình của bệnh viện
            var sessionTypes = await this.unitOfWork.Repository<SessionTypes>().GetQueryable()
                .Where(e => !e.Deleted && e.Active && e.HospitalId == hospitalInfo.Id).ToListAsync();
            if (examinationSchedules != null && examinationSchedules.Any())
            {
                foreach (var examinationSchedule in examinationSchedules)
                {
                    if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0)
                    {
                        // Cập nhật lại giới hạn số người khám theo buổi cho lịch
                        if (sessionTypes != null && sessionTypes.Any())
                        {
                            foreach (var sessionType in sessionTypes)
                            {
                                var totalMinuteExamination = (sessionType.ToTime ?? 0) - (sessionType.FromTime ?? 0);
                                int maximumPatient = 0;
                                maximumPatient = (totalMinuteExamination / hospitalInfo.MinutePerPatient);
                                switch (sessionType.Code)
                                {
                                    // Lấy thông tin buổi sáng => tính tổng số bệnh nhân tối đa cho buổi sáng.
                                    case "BS":
                                        {
                                            if (totalMinuteExamination > 0)
                                            {
                                                if (maximumPatient > 0) examinationSchedule.MaximumMorningExamination = maximumPatient;
                                            }
                                        }
                                        break;
                                    // Lấy thông tin buổi sáng => tính tổng số bệnh nhân tối đa cho buổi chiều.
                                    case "BC":
                                        {
                                            if (totalMinuteExamination > 0)
                                            {
                                                if (maximumPatient > 0) examinationSchedule.MaximumAfternoonExamination = maximumPatient;
                                            }
                                        }
                                        break;
                                    // Lấy thông tin buổi chiều => tính tổng số bệnh nhân tối đa cho ngoài giờ.
                                    default:
                                        {
                                            if (maximumPatient > 0) examinationSchedule.MaximumOtherExamination = maximumPatient;
                                        }
                                        break;
                                }
                            }

                            examinationSchedule.Updated = DateTime.Now;
                            examinationSchedule.UpdatedBy = hospitalInfo.UpdatedBy;

                            Expression<Func<ExaminationSchedules, object>>[] expressions = new Expression<Func<ExaminationSchedules, object>>[]
                            {
                                e => e.MaximumAfternoonExamination,
                                e => e.MaximumMorningExamination,
                                e => e.MaximumOtherExamination,
                                e => e.Updated,
                                e => e.UpdatedBy
                            };
                            this.unitOfWork.Repository<ExaminationSchedules>().UpdateFieldsSave(examinationSchedule, expressions);
                        }

                        // Cập nhật lại số lượng khám theo chi tiết lịch
                        var examinationScheduleDetails = await this.unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                            .Where(e => !e.Deleted && e.Active && e.ScheduleId == examinationSchedule.Id
                            && e.IsUseHospitalConfig
                            )
                            .ToListAsync();
                        if (examinationScheduleDetails != null && examinationScheduleDetails.Any())
                        {
                            foreach (var examinationScheduleDetail in examinationScheduleDetails)
                            {
                                var totalMinuteExamination = (examinationScheduleDetail.ToTime ?? 0) - (examinationScheduleDetail.FromTime ?? 0);
                                if (totalMinuteExamination > 0)
                                {
                                    var maximumPatient = (totalMinuteExamination / hospitalInfo.MinutePerPatient);
                                    if (maximumPatient > 0)
                                    {
                                        examinationScheduleDetail.MaximumExamination = maximumPatient;
                                        examinationScheduleDetail.Updated = DateTime.Now;
                                        examinationScheduleDetail.UpdatedBy = hospitalInfo.UpdatedBy;
                                        Expression<Func<ExaminationScheduleDetails, object>>[] expressionDetails = new Expression<Func<ExaminationScheduleDetails, object>>[]
                                        {
                                            e => e.MaximumExamination,
                                            e => e.Updated,
                                            e => e.UpdatedBy
                                        };
                                        this.unitOfWork.Repository<ExaminationScheduleDetails>().UpdateFieldsSave(examinationScheduleDetail, expressionDetails);
                                    }
                                }
                            }
                        }
                    }
                }
                await this.unitOfWork.SaveAsync();
            }
        }
    }
}
