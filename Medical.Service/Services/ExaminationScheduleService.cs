using AutoMapper;
using Ganss.Excel;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class ExaminationScheduleService : DomainService<ExaminationSchedules, SearchExaminationSchedule>, IExaminationScheduleService
    {
        private IConfiguration configuration;
        private INotificationService notificationService;
        private IHubContext<NotificationHub> hubContext;
        private IHubContext<NotificationAppHub> appHubContext;

        public ExaminationScheduleService(IMedicalUnitOfWork unitOfWork
            , IMedicalDbContext medicalDbContext
            , IMapper mapper
            , IConfiguration configuration
            , IServiceProvider serviceProvider
            , IHubContext<NotificationHub> hubContext
            , IHubContext<NotificationAppHub> appHubContext
            ) : base(unitOfWork, medicalDbContext, mapper)
        {
            this.configuration = configuration;
            this.notificationService = serviceProvider.GetRequiredService<INotificationService>();
            this.hubContext = hubContext;
            this.appHubContext = appHubContext;
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
                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@ExaminationScheduleId", baseSearch.ExaminationScheduleId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Thêm mới thông tin lịch trực
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ExaminationSchedules item)
        {
            bool result = false;
            if (item != null)
            {
                using (var contextTransactionTask = medicalDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Lấy thông tin bệnh viện
                        var hospitalInfo = await this.unitOfWork.Repository<Hospitals>().GetQueryable().Where(e => e.Id == item.HospitalId).FirstOrDefaultAsync();
                        // Lấy ra thông tin tất cả buổi cấu hình của bệnh viện
                        var sessionTypes = await this.unitOfWork.Repository<SessionTypes>().GetQueryable()
                            .Where(e => !e.Deleted && e.Active && e.HospitalId == hospitalInfo.Id).ToListAsync();

                        // Nếu theo cấu hình của bệnh viện thì tính theo số phút được cấu hình ở thông tin bệnh viện
                        // Ngược lại => theo cấu hình của lịch
                        if (item.IsUseHospitalConfig)
                        {
                            if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0)
                            {
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
                                                        if (maximumPatient > 0) item.MaximumMorningExamination = maximumPatient;
                                                    }
                                                }
                                                break;
                                            // Lấy thông tin buổi sáng => tính tổng số bệnh nhân tối đa cho buổi chiều.
                                            case "BC":
                                                {
                                                    if (totalMinuteExamination > 0)
                                                    {
                                                        if (maximumPatient > 0) item.MaximumAfternoonExamination = maximumPatient;
                                                    }
                                                }
                                                break;
                                            // Lấy thông tin buổi chiều => tính tổng số bệnh nhân tối đa cho ngoài giờ.
                                            default:
                                                {
                                                    if (maximumPatient > 0) item.MaximumOtherExamination = maximumPatient;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }

                            foreach (var examinationScheduleDetail in item.ExaminationScheduleDetails)
                            {
                                // Kiểm tra khung giờ có tối đa bao nhiu bệnh nhân dụa trên số phút khám mỗi ca của bệnh viện.
                                if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0
                                    )
                                {
                                    var totalMinuteExamination = (examinationScheduleDetail.ToTime ?? 0) - (examinationScheduleDetail.FromTime ?? 0);
                                    if (totalMinuteExamination > 0)
                                    {
                                        var maximumPatient = (totalMinuteExamination / hospitalInfo.MinutePerPatient);
                                        if (maximumPatient > 0) examinationScheduleDetail.MaximumExamination = maximumPatient;
                                    }
                                }
                            }

                        }

                        // Tính lại số ca tối đa theo từng buổi trực
                        // BUỔI SÁNG
                        var sessionTypeMorning = sessionTypes.Where(e => e.Code == "BS").FirstOrDefault();
                        if (sessionTypeMorning != null)
                        {
                            var totalDetailMorningValids = item.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeMorning.Id
                            //&& !e.IsUseHospitalConfig 
                            && !e.Deleted).ToList();
                            if (totalDetailMorningValids != null && totalDetailMorningValids.Any())
                            {
                                item.MaximumMorningExamination = totalDetailMorningValids.Sum(e => e.MaximumExamination ?? 0);
                            }
                        }
                        // BUỔI CHIỀU
                        var sessionTypeAfternoon = sessionTypes.Where(e => e.Code == "BC").FirstOrDefault();
                        if (sessionTypeAfternoon != null)
                        {
                            var totalDetailAfternoonValids = item.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeAfternoon.Id
                            //&& !e.IsUseHospitalConfig 
                            && !e.Deleted).ToList();
                            if (totalDetailAfternoonValids != null && totalDetailAfternoonValids.Any())
                            {
                                item.MaximumAfternoonExamination = totalDetailAfternoonValids.Sum(e => e.MaximumExamination ?? 0);
                            }
                        }
                        // NGOÀI GIỜ
                        var sessionTypeOther = sessionTypes.Where(e => e.Code != "BC" && e.Code != "BS").FirstOrDefault();
                        if (sessionTypeOther != null)
                        {
                            var totalDetailOtherValids = item.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeOther.Id
                            //&& !e.IsUseHospitalConfig 
                            && !e.Deleted).ToList();
                            if (totalDetailOtherValids != null && totalDetailOtherValids.Any())
                            {
                                item.MaximumOtherExamination = totalDetailOtherValids.Sum(e => e.MaximumExamination ?? 0);
                            }
                        }

                        item.ImportScheduleId = Guid.NewGuid();
                        await unitOfWork.Repository<ExaminationSchedules>().CreateAsync(item);
                        await unitOfWork.SaveAsync();

                        // Thêm thông tin y tá của ca trực
                        if (item.NurseIds != null && item.NurseIds.Any())
                        {
                            var nurseInfoSelecteds = await this.unitOfWork.Repository<Doctors>().GetQueryable()
                                .Where(e => item.NurseIds.Contains(e.Id))
                                .Select(e => new Doctors()
                                {
                                    Id = e.Id,
                                    UserId = e.UserId
                                })
                                .ToListAsync();
                            foreach (var nurseInfoSelected in nurseInfoSelecteds)
                            {
                                ExaminationScheduleMappingUsers examinationScheduleMappingUsers = new ExaminationScheduleMappingUsers()
                                {
                                    Deleted = false,
                                    Active = true,
                                    Created = DateTime.Now,
                                    CreatedBy = item.CreatedBy,
                                    DoctorId = nurseInfoSelected.Id,
                                    HospitalId = item.HospitalId,
                                    ExaminationScheduleId = item.Id,
                                    UserId = nurseInfoSelected.UserId ?? 0,
                                    ImportScheduleId = item.ImportScheduleId
                                };
                                this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().Create(examinationScheduleMappingUsers);
                            }
                        }

                        // Cập nhật thông tin chi tiết ca
                        if (item.ExaminationScheduleDetails != null && item.ExaminationScheduleDetails.Any())
                        {
                            foreach (var examinationScheduleDetail in item.ExaminationScheduleDetails)
                            {
                                examinationScheduleDetail.ScheduleId = item.Id;
                                examinationScheduleDetail.Created = DateTime.Now;
                                examinationScheduleDetail.Active = true;
                                examinationScheduleDetail.Id = 0;
                                // Kiểm tra khung giờ có tối đa bao nhiu bệnh nhân dụa trên số phút khám mỗi ca của bệnh viện.
                                //if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0 
                                //    && examinationScheduleDetail.IsUseHospitalConfig
                                //    )
                                //{
                                //    var totalMinuteExamination = (examinationScheduleDetail.ToTime ?? 0) - (examinationScheduleDetail.FromTime ?? 0);
                                //    if (totalMinuteExamination > 0)
                                //    {
                                //        var maximumPatient = (totalMinuteExamination / hospitalInfo.MinutePerPatient);
                                //        if (maximumPatient > 0) examinationScheduleDetail.MaximumExamination = maximumPatient;
                                //    }
                                //}
                                examinationScheduleDetail.ImportScheduleId = item.ImportScheduleId;
                                await unitOfWork.Repository<ExaminationScheduleDetails>().CreateAsync(examinationScheduleDetail);
                            }
                        }
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
                }

            }
            return result;
        }

        /// <summary>
        ///  Cập nhật thông tin lịch trực
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(ExaminationSchedules item)
        {
            using (var contextTransactionTask = medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();
                    List<int> examinationScheduleDetailIds = new List<int>();

                    // Danh sách ca trực thay đổi
                    List<int> examinationScheduleDetailChangeIds = new List<int>();

                    // Danh sách phiếu khám ngoài giới hạn
                    List<int> examinationFormOverRatedIds = new List<int>();

                    // Lấy danh sách phiếu bị ảnh hưởng bởi lịch trực
                    List<int> examinationFormDeletedIds = new List<int>();
                    // Danh sách app mobile user id nhận thông báo
                    List<int> appMobileUserIds = new List<int>();

                    if (exists != null)
                    {
                        // Lấy thông tin bệnh viện
                        var hospitalInfo = await this.unitOfWork.Repository<Hospitals>().GetQueryable().Where(e => e.Id == item.HospitalId).FirstOrDefaultAsync();
                        // Lấy ra thông tin tất cả buổi cấu hình của bệnh viện
                        var sessionTypes = await this.unitOfWork.Repository<SessionTypes>().GetQueryable()
                            .Where(e => !e.Deleted && e.Active && e.HospitalId == hospitalInfo.Id).ToListAsync();
                        // Nếu theo cấu hình của bệnh viện thì tính theo số phút được cấu hình ở thông tin bệnh viện
                        // Ngược lại => theo cấu hình của lịch
                        if (item.IsUseHospitalConfig)
                        {
                            if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0)
                            {
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
                                                        if (maximumPatient > 0) item.MaximumMorningExamination = maximumPatient;
                                                    }
                                                }
                                                break;
                                            // Lấy thông tin buổi sáng => tính tổng số bệnh nhân tối đa cho buổi chiều.
                                            case "BC":
                                                {
                                                    if (totalMinuteExamination > 0)
                                                    {
                                                        if (maximumPatient > 0) item.MaximumAfternoonExamination = maximumPatient;
                                                    }
                                                }
                                                break;
                                            // Lấy thông tin buổi chiều => tính tổng số bệnh nhân tối đa cho ngoài giờ.
                                            default:
                                                {
                                                    if (maximumPatient > 0) item.MaximumOtherExamination = maximumPatient;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }

                            foreach (var examinationScheduleDetail in item.ExaminationScheduleDetails)
                            {
                                // Kiểm tra khung giờ có tối đa bao nhiu bệnh nhân dụa trên số phút khám mỗi ca của bệnh viện.
                                if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0
                                    )
                                {
                                    var totalMinuteExamination = (examinationScheduleDetail.ToTime ?? 0) - (examinationScheduleDetail.FromTime ?? 0);
                                    if (totalMinuteExamination > 0)
                                    {
                                        var maximumPatient = (totalMinuteExamination / hospitalInfo.MinutePerPatient);
                                        if (maximumPatient > 0) examinationScheduleDetail.MaximumExamination = maximumPatient;
                                    }
                                }
                            }

                        }

                        // Tính lại số ca tối đa theo từng buổi trực
                        // BUỔI SÁNG
                        var sessionTypeMorning = sessionTypes.Where(e => e.Code == "BS").FirstOrDefault();
                        if (sessionTypeMorning != null)
                        {
                            var totalDetailMorningValids = item.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeMorning.Id
                            //&& !e.IsUseHospitalConfig 
                            && !e.Deleted).ToList();
                            if (totalDetailMorningValids != null && totalDetailMorningValids.Any())
                            {
                                item.MaximumMorningExamination = totalDetailMorningValids.Sum(e => e.MaximumExamination ?? 0);
                            }
                        }
                        // BUỔI CHIỀU
                        var sessionTypeAfternoon = sessionTypes.Where(e => e.Code == "BC").FirstOrDefault();
                        if (sessionTypeAfternoon != null)
                        {
                            var totalDetailAfternoonValids = item.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeAfternoon.Id
                            //&& !e.IsUseHospitalConfig 
                            && !e.Deleted).ToList();
                            if (totalDetailAfternoonValids != null && totalDetailAfternoonValids.Any())
                            {
                                item.MaximumAfternoonExamination = totalDetailAfternoonValids.Sum(e => e.MaximumExamination ?? 0);
                            }
                        }
                        // NGOÀI GIỜ
                        var sessionTypeOther = sessionTypes.Where(e => e.Code != "BC" && e.Code != "BS").FirstOrDefault();
                        if (sessionTypeOther != null)
                        {
                            var totalDetailOtherValids = item.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeOther.Id
                            //&& !e.IsUseHospitalConfig 
                            && !e.Deleted).ToList();
                            if (totalDetailOtherValids != null && totalDetailOtherValids.Any())
                            {
                                item.MaximumOtherExamination = totalDetailOtherValids.Sum(e => e.MaximumExamination ?? 0);
                            }
                        }

                        var importScheduleId = exists.ImportScheduleId;
                        exists = mapper.Map<ExaminationSchedules>(item);
                        exists.ImportScheduleId = importScheduleId;
                        unitOfWork.Repository<ExaminationSchedules>().Update(exists);

                        // Cập nhật thông tin y tá/điều dưỡng cho ca trực
                        if (item.NurseIds != null && item.NurseIds.Any())
                        {
                            // LẤY RA THÔNG TIN Y TÁ/ĐIỀU DƯỠNG ĐƯỢC CHỌN
                            var nurseInfoSelecteds = await this.unitOfWork.Repository<Doctors>().GetQueryable()
                                .Where(e => item.NurseIds.Contains(e.Id))
                                .Select(e => new Doctors()
                                {
                                    Id = e.Id,
                                    UserId = e.UserId
                                }).ToListAsync();
                            // CẬP NHẬT LẠI THÔNG TIN Y TÁ/ĐIỀU DƯỠNG TRONG CA TRỰC
                            foreach (var nurseInfoSelected in nurseInfoSelecteds)
                            {
                                var existScheduleMappingUser = await this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().GetQueryable()
                                    .Where(e => e.DoctorId == nurseInfoSelected.Id).FirstOrDefaultAsync();
                                if (existScheduleMappingUser != null)
                                {
                                    existScheduleMappingUser.Updated = DateTime.Now;
                                    existScheduleMappingUser.UpdatedBy = item.UpdatedBy;
                                    existScheduleMappingUser.ImportScheduleId = importScheduleId;
                                    this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().Update(existScheduleMappingUser);
                                }
                                else
                                {
                                    ExaminationScheduleMappingUsers examinationScheduleMappingUsers = new ExaminationScheduleMappingUsers()
                                    {
                                        Deleted = false,
                                        Active = true,
                                        Created = DateTime.Now,
                                        CreatedBy = item.UpdatedBy,
                                        DoctorId = nurseInfoSelected.Id,
                                        HospitalId = item.HospitalId,
                                        UserId = nurseInfoSelected.UserId,
                                        ExaminationScheduleId = item.Id,
                                        ImportScheduleId = importScheduleId,
                                    };
                                    this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().Create(examinationScheduleMappingUsers);
                                }
                            }

                            // XÓA CÁC Y TÁ/ĐIỀU DƯỠNG KHÔNG TỒN TẠI TRONG CA TRỰC
                            var existGroupOlds = await this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().GetQueryable()
                                .Where(e => !item.NurseIds.Contains(e.DoctorId) && e.ExaminationScheduleId == item.Id).ToListAsync();
                            if (existGroupOlds != null && existGroupOlds.Any())
                            {
                                foreach (var existGroupOld in existGroupOlds)
                                {
                                    this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().Delete(existGroupOld);
                                }
                            }

                        }
                        else
                        {
                            // LẤY HẾT THÔNG TIN Y TÁ/ĐIỀU DƯỠNG HIỆN TẠI CỦA LỊCH TRỰC => XÓA
                            var currentExaminationScheduleMappingUsers = await this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().GetQueryable()
                                .Where(e => e.ExaminationScheduleId == item.Id).ToListAsync();
                            if (currentExaminationScheduleMappingUsers != null && currentExaminationScheduleMappingUsers.Any())
                            {
                                foreach (var currentExaminationScheduleMappingUser in currentExaminationScheduleMappingUsers)
                                {
                                    this.unitOfWork.Repository<ExaminationScheduleMappingUsers>().Delete(currentExaminationScheduleMappingUser);
                                }
                            }
                        }

                        // Cập nhật thông tin chi tiết ca
                        if (item.ExaminationScheduleDetails != null && item.ExaminationScheduleDetails.Any())
                        {
                            foreach (var examinationScheduleDetail in item.ExaminationScheduleDetails)
                            {
                                var existExaminationScheduleDetail = await unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                                                                     .AsNoTracking()
                                                                     .Where(e => e.Id == examinationScheduleDetail.Id && !e.Deleted)
                                                                     .FirstOrDefaultAsync();
                                // Kiểm tra khung giờ có tối đa bao nhiu bệnh nhân dụa trên số phút khám mỗi ca của bệnh viện.
                                //if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0
                                //    //&& examinationScheduleDetail.IsUseHospitalConfig
                                //    && item.IsUseHospitalConfig
                                //    && !examinationScheduleDetail.Deleted
                                //    )
                                //{
                                //    var totalMinuteExamination = (examinationScheduleDetail.ToTime ?? 0) - (examinationScheduleDetail.FromTime ?? 0);
                                //    if (totalMinuteExamination > 0)
                                //    {
                                //        var maximumPatient = (totalMinuteExamination / hospitalInfo.MinutePerPatient);
                                //        if (maximumPatient > 0) examinationScheduleDetail.MaximumExamination = maximumPatient;
                                //    }
                                //}

                                if (existExaminationScheduleDetail != null)
                                {
                                    // TRƯỜNG HỢP CHI TIẾT CA TRỰC BỊ XÓA (THAY ĐỔI TOÀN PHẦN)
                                    // => THỰC HIỆN HOÀN TIỀN CHO LỊCH ĐÃ XÁC NHẬN/ĐÃ XÁC NHẬN TÁI KHÁM
                                    if (examinationScheduleDetail.Deleted)
                                    {
                                        // Lấy ra tất cả phiếu đăng kí ca trực này (Đã xác nhận/Đã xác nhận tái khám)
                                        var examinationFormRefunds = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                                            .Where(e => !e.Deleted && e.ExaminationScheduleDetailId == examinationScheduleDetail.Id
                                            && (e.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.New
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.WaitReExamination
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.WaitConfirm
                                            )
                                            ).ToListAsync();
                                        // Cập nhật lại trạng thái hoàn tiền cho phiếu
                                        if (examinationFormRefunds != null && examinationFormRefunds.Any())
                                        {
                                            foreach (var examinationFormRefund in examinationFormRefunds)
                                            {
                                                if (examinationFormRefund.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                                    || examinationFormRefund.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination
                                                    )
                                                    examinationFormRefund.Status = (int)CatalogueUtilities.ExaminationStatus.WaitRefund;
                                                else examinationFormRefund.Status = (int)CatalogueUtilities.ExaminationStatus.Canceled;
                                                examinationFormRefund.Updated = DateTime.Now;
                                                examinationFormRefund.UpdatedBy = item.UpdatedBy;
                                                Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                                {
                                            e => e.Status,
                                            e => e.Updated,
                                            e => e.UpdatedBy
                                                };
                                                this.unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(examinationFormRefund, includeProperties);
                                                examinationFormDeletedIds.Add(examinationFormRefund.Id);
                                            }
                                        }
                                        continue;
                                    }

                                    // TRƯỜNG HỢP THAY ĐỔI CẤU HÌNH TỪ GIỜ ĐẾN GIỜ CỦA LỊCH TRỰC => TÍNH TOÁN LẠI GIỚI HẠN NGƯỜI KHÁM CHO CHI TIẾT CA TRỰC
                                    // => HỦY PHIẾU CỦA NHỮNG USER THEO SỐ THỨ TỰ PHIẾU > Tổng số ca giới hạn mới
                                    if (existExaminationScheduleDetail.FromTime != examinationScheduleDetail.FromTime
                                        || existExaminationScheduleDetail.ToTime != examinationScheduleDetail.ToTime
                                        && !examinationScheduleDetail.Deleted
                                        )
                                    {
                                        // Lấy ra số thứ tự khám tối đa đã đăng kí ca khám này
                                        var examinationFormCheckIndexs = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                                            .Where(e => !e.Deleted && e.ExaminationScheduleDetailId == existExaminationScheduleDetail.Id
                                            && e.SystemIndex.HasValue
                                            && e.SystemIndex.Value > examinationScheduleDetail.MaximumExamination
                                            ).ToListAsync();
                                        // Hủy tất cả lịch đăng kí đang ngoài giới hạn
                                        if (examinationFormCheckIndexs != null && examinationFormCheckIndexs.Any())
                                        {
                                            foreach (var examinationFormCheckIndex in examinationFormCheckIndexs)
                                            {
                                                // NẾU ĐÃ THANH TOÁN (ĐÃ XÁC NHẬN/ĐÃ XÁC NHẬN TÁI KHÁM) => CHUYỂN VỀ TRẠNG THÁI CHỜ HOÀN TIỀN
                                                if (examinationFormCheckIndex.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                                    || examinationFormCheckIndex.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination
                                                    )
                                                    examinationFormCheckIndex.Status = (int)CatalogueUtilities.ExaminationStatus.WaitRefund;
                                                // NHỮNG TRẠNG THÁI CÒN LẠI => CHUYỂN VỀ TRẠNG THÁI HỦY PHIẾU
                                                else examinationFormCheckIndex.Status = (int)CatalogueUtilities.ExaminationStatus.Canceled;
                                                examinationFormCheckIndex.Updated = DateTime.Now;
                                                examinationFormCheckIndex.UpdatedBy = item.UpdatedBy;
                                                Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                                {
                                            e => e.Status,
                                            e => e.Updated,
                                            e => e.UpdatedBy
                                                };
                                                this.unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(examinationFormCheckIndex, includeProperties);
                                                examinationFormOverRatedIds.Add(examinationFormCheckIndex.Id);
                                            }
                                        }
                                        examinationScheduleDetailChangeIds.Add(existExaminationScheduleDetail.Id);
                                    }
                                    existExaminationScheduleDetail = mapper.Map<ExaminationScheduleDetails>(examinationScheduleDetail);
                                    existExaminationScheduleDetail.ScheduleId = exists.Id;
                                    existExaminationScheduleDetail.Updated = DateTime.Now;
                                    existExaminationScheduleDetail.ImportScheduleId = importScheduleId;
                                    unitOfWork.Repository<ExaminationScheduleDetails>().Update(examinationScheduleDetail);
                                }
                                else
                                {
                                    if (examinationScheduleDetail.Deleted) continue;
                                    examinationScheduleDetail.ScheduleId = exists.Id;
                                    examinationScheduleDetail.Created = DateTime.Now;
                                    examinationScheduleDetail.Id = 0;
                                    examinationScheduleDetail.ImportScheduleId = importScheduleId;
                                    unitOfWork.Repository<ExaminationScheduleDetails>().Create(examinationScheduleDetail);
                                    continue;
                                }
                                // Nếu lịch có bác sĩ thay thế => không cần cập nhật bác sĩ thay thế trong khung giờ
                                if (item.ReplaceDoctorId.HasValue && item.ReplaceDoctorId.Value > 0) continue;



                                // Cập nhật lại phiếu đăng kí với bác sĩ thay thế
                                var examinationFormUpdateDoctor = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                                    .Where(e => !e.Deleted && e.Active && e.ExaminationScheduleDetailId.HasValue
                                    && e.ExaminationScheduleDetailId == examinationScheduleDetail.Id
                                    ).FirstOrDefaultAsync();
                                // Bác sĩ thay thế của ca trực
                                if (examinationFormUpdateDoctor != null && examinationScheduleDetail.ReplaceDoctorId.HasValue
                                    && examinationScheduleDetail.ReplaceDoctorId.Value > 0)
                                {
                                    examinationFormUpdateDoctor.DoctorId = examinationScheduleDetail.ReplaceDoctorId;
                                    examinationFormUpdateDoctor.Updated = DateTime.Now;
                                    examinationFormUpdateDoctor.UpdatedBy = item.UpdatedBy;
                                    Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                    {
                                e => e.DoctorId,
                                e => e.Updated,
                                e => e.UpdatedBy
                                    };
                                    this.unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(examinationFormUpdateDoctor, includeProperties);


                                    // THÔNG BÁO CA TRỰC CHO BÁC SĨ THAY THẾ + BÁC SĨ CŨ - WEBAPP

                                    // THÔNG BÁO CHO USER ĐỔI BÁC SĨ THAY THẾ - APP MOBILE

                                }
                            }
                        }
                        await unitOfWork.SaveAsync();
                        // Cập nhật lại thông tin phiếu đăng kí nếu LỊCH có bác sĩ thay thế
                        if (item.ReplaceDoctorId.HasValue && item.ReplaceDoctorId.Value > 0)
                        {
                            var examinationScheduleDetails = await this.unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                                .Where(e => !e.Deleted && e.Active && e.ScheduleId == item.Id)
                                .Select(e => new ExaminationScheduleDetails()
                                {
                                    Id = e.Id
                                }).ToListAsync();
                            if (examinationScheduleDetails != null && examinationScheduleDetails.Any())
                            {
                                examinationScheduleDetailIds = examinationScheduleDetails.Select(e => e.Id).ToList();
                                var examinationFormUpdateDoctors = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                                    .Where(e => !e.Deleted && e.Active && e.ExaminationScheduleDetailId.HasValue
                                    && examinationScheduleDetailIds.Contains(e.ExaminationScheduleDetailId.Value)).ToListAsync();
                                if (examinationFormUpdateDoctors != null && examinationFormUpdateDoctors.Any())
                                {
                                    foreach (var examinationFormUpdateDoctor in examinationFormUpdateDoctors)
                                    {
                                        examinationFormUpdateDoctor.DoctorId = item.ReplaceDoctorId;
                                        examinationFormUpdateDoctor.Updated = DateTime.Now;
                                        examinationFormUpdateDoctor.UpdatedBy = item.UpdatedBy;
                                        Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                        {
                                            e => e.Updated,
                                            e => e.DoctorId,
                                            e => e.UpdatedBy
                                        };
                                        this.unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(examinationFormUpdateDoctor, includeProperties);
                                    }
                                    await this.unitOfWork.SaveAsync();


                                    // THÔNG BÁO CHO BÁC SĨ THAY THẾ + BÁC SĨ LỊCH CŨ

                                    // THÔNG BÁO CHO USER ĐỔI BÁC SĨ THAY THẾ

                                }
                            }
                        }

                        // TẠO THÔNG BÁO HỦY CA TRỰC:
                        if (examinationFormDeletedIds != null && examinationFormDeletedIds.Any())
                        {
                            await this.notificationService.CreateCustomNotificationUser(null
                            , item.HospitalId
                            , null
                            , string.Format("/schedule/my-examination-schedule/{0}", exists.Id)
                            , string.Empty
                            , item.UpdatedBy
                            , null
                            , false
                            , "USER"
                            , CoreContants.TEMPLATE_EXAMINATION_SCHEDULE_DETAIL_DT_DELETE
                            , examinationFormDeletedIds
                            , null
                            , null
                            );
                            await this.hubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                            await this.appHubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                        }
                        // TẠO THÔNG BÁO CHỈNH SỬA CA TRỰC CHO BÁC SĨ
                        if (examinationScheduleDetailChangeIds != null && examinationScheduleDetailChangeIds.Any())
                        {
                            await this.notificationService.CreateCustomNotificationUser(null
                            , item.HospitalId
                            , null
                            , string.Format("/schedule/my-examination-schedule/{0}", exists.Id)
                            , string.Empty
                            , item.UpdatedBy
                            , null
                            , false
                            , "USER"
                            , CoreContants.TEMPLATE_EXAMINATION_SCHEDULE_DETAIL_DT_DELETE
                            , examinationFormDeletedIds
                            , null
                            , null
                            );
                            await this.hubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                        }


                        // TẠO THÔNG BÁO HỦY LỊCH ĐĂNG KÍ CHO USER (NẾU CÓ)


                    }
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
            }
        }

        /// <summary>
        /// Xóa lịch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(int id)
        {
            var exists = Queryable
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == id);
            if (exists != null)
            {
                exists.Deleted = true;
                unitOfWork.Repository<ExaminationSchedules>().Update(exists);
                await unitOfWork.SaveAsync();

                return true;
            }
            else
            {
                throw new Exception(id + " not exists");
            }
        }

        /// <summary>
        /// Xóa thông tin nhiều lịch
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="deletedBy"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteAsync(List<int> ids)
        {
            using (var contextTransaction = await this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // LẤY RA THÔNG TIN LỊCH CẦN XÓA
                    var existExaminationSchedules = await this.unitOfWork.Repository<ExaminationSchedules>().GetQueryable()
                        .Where(e => ids.Contains(e.Id)).ToListAsync();
                    if (existExaminationSchedules == null || !existExaminationSchedules.Any()) throw new AppException("Không tìm thấy thông tin item");
                    // Lấy ra danh sách id của schedule
                    var examinationScheduleIds = existExaminationSchedules.Select(e => e.Id).ToList();
                    // LẤY RA THÔNG TIN CHI TIẾT CA TRỰC CẦN XÓA
                    var examinationScheduleDetailInfos = await this.unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                        .Where(e => !e.Deleted && examinationScheduleIds.Contains(e.ScheduleId)).ToListAsync();
                    if (examinationScheduleDetailInfos != null && examinationScheduleDetailInfos.Any())
                    {
                        // Lấy ra danh sách ID Ca trực
                        var examinationScheduleDetailIds = examinationScheduleDetailInfos.Select(e => e.Id).ToList();
                        // CẬP NHẬT LẠI TRẠNG THÁI PHIẾU ĐĂNG KÍ THEO CA TRỰC BỊ HỦY
                        await this.UpdateExaminationFormStatus(examinationScheduleDetailIds);

                        // XÓA TÁT CẢ CA TRỰC THEO THÔNG TIN PHÒNG ĐƯỢC CHỌN
                        this.DeleteExaminationScheduleDetail(examinationScheduleDetailInfos);
                    }

                    // XÓA THÔNG TIN CA TRỰC ĐƯỢC CHỌN
                    foreach (var existExaminationSchedule in existExaminationSchedules)
                    {
                        existExaminationSchedule.Deleted = true;
                        existExaminationSchedule.Updated = DateTime.Now;
                        existExaminationSchedule.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                        Expression<Func<ExaminationSchedules, object>>[] includeProperties = new Expression<Func<ExaminationSchedules, object>>[]
                        {
                            e => e.Deleted,
                            e => e.Updated,
                            e => e.UpdatedBy
                        };
                        this.unitOfWork.Repository<ExaminationSchedules>().UpdateFieldsSave(existExaminationSchedule, includeProperties);
                    }


                    //------------------------------------------------------ THÔNG BÁO CHO BÁC SĨ + Y TÁ THÔNG TIN CÁC CA TRỰC BỊ HỦY


                    await this.unitOfWork.SaveAsync();
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    contextTransaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Thêm mới nhanh lịch trực của bác sĩ
        /// </summary>
        /// <param name="examinationScheduleExtensions"></param>
        /// <returns></returns>
        public async Task<bool> AddMultipleExaminationSchedule(IList<ExaminationScheduleExtensions> examinationScheduleExtensions
            , int hospitalId, bool isImport = false)
        {
            bool result = true;
            // Danh sách lịch trực được đăng kí
            DataTable dataTableSchedule = SetDataTable("ExaminationSchedules");
            IList<ExaminationSchedules> examinationSchedules = new List<ExaminationSchedules>();
            // Danh sách chi tiết ca trực
            DataTable dataTableScheduleDetail = SetDataTable("ExaminationScheduleDetails");
            IList<ExaminationScheduleDetails> examinationScheduleDetails = new List<ExaminationScheduleDetails>();

            // Lấy thông tin bệnh viện
            var hospitalInfo = await this.unitOfWork.Repository<Hospitals>().GetQueryable().Where(e => e.Id == hospitalId).FirstOrDefaultAsync();
            // Lấy ra thông tin tất cả buổi cấu hình của bệnh viện
            var sessionTypes = await this.unitOfWork.Repository<SessionTypes>().GetQueryable()
                .Where(e => !e.Deleted && e.Active && e.HospitalId == hospitalId).ToListAsync();

            foreach (var examinationScheduleExtension in examinationScheduleExtensions)
            {
                // Kiểm tra dữ liệu từ ngày/đến ngày
                //if (!examinationScheduleExtension.FromDate.HasValue || !examinationScheduleExtension.ToDate.HasValue)
                //    throw new AppException("Vui lòng chọn từ ngày/đến ngày");
                //if (examinationScheduleExtension.FromDate.Value.Date > examinationScheduleExtension.ToDate.Value.Date)
                //    throw new AppException("Vui lòng chọn từ ngày nhỏ hơn đến ngày");
                TimeSpan ts = new TimeSpan(0, 0, 0, 0);
                //// Từ ngày
                //DateTime fromDate = examinationScheduleExtension.FromDate.Value.Date + ts;
                //// Đến ngày
                //DateTime toDate = examinationScheduleExtension.ToDate.Value.Date + ts;

                // Lấy ra danh sách ca trực của bác sĩ từ ngày đến ngày
                var examinationScheduleChecks = await this.Queryable.Where(e => !e.Deleted
                //&& e.ExaminationDate.Date >= fromDate.Date
                //&& e.ExaminationDate.Date <= toDate.Date
                && examinationScheduleExtension.ExaminationDates.Contains(e.ExaminationDate.Date)
                && e.DoctorId == examinationScheduleExtension.DoctorId
                && e.HospitalId == examinationScheduleExtension.HospitalId
                ).ToListAsync();
                // Nếu tồn tại lịch trong khoảng ngày => trả ra FE
                if (examinationScheduleChecks != null && examinationScheduleChecks.Any() && !isImport)
                {
                    var listExaminationDate_s = examinationScheduleChecks.Select(e => e.ExaminationDate.ToString("dd/MM/yyyy")).ToList();
                    throw new AppException(string.Format("Lịch khám của ngày {0} đã tồn tại", string.Join(", ", listExaminationDate_s)));
                }
                bool isUseHospitalConfig = true;
                if ((examinationScheduleExtension.MaximumMorningExamination.HasValue && examinationScheduleExtension.MaximumMorningExamination.Value > 0)
                    || (examinationScheduleExtension.MaximumOtherExamination.HasValue && examinationScheduleExtension.MaximumOtherExamination.Value > 0)
                    || (examinationScheduleExtension.MaximumAfternoonExamination.HasValue && examinationScheduleExtension.MaximumAfternoonExamination.Value > 0)
                    )
                    isUseHospitalConfig = false;

                foreach (var fromDate in examinationScheduleExtension.ExaminationDates)
                {
                    ExaminationSchedules examinationSchedule = new ExaminationSchedules()
                    {
                        Deleted = false,
                        Active = true,
                        DoctorId = examinationScheduleExtension.DoctorId ?? 0,
                        HospitalId = examinationScheduleExtension.HospitalId,
                        ExaminationDate = fromDate,
                        SpecialistTypeId = examinationScheduleExtension.SpecialistTypeId ?? 0,
                        MaximumMorningExamination = examinationScheduleExtension.MaximumMorningExamination,
                        MaximumOtherExamination = examinationScheduleExtension.MaximumOtherExamination,
                        MaximumAfternoonExamination = examinationScheduleExtension.MaximumAfternoonExamination,
                        Created = DateTime.Now,
                        CreatedBy = examinationScheduleExtension.CreatedBy,
                        ExaminationScheduleDetails = examinationScheduleExtension.ExaminationScheduleDetails,
                        IsUseHospitalConfig = !isImport ? examinationScheduleExtension.IsUseHospitalConfig : isUseHospitalConfig,
                        ImportScheduleId = Guid.NewGuid()
                    };

                    // Nếu theo cấu hình của bệnh viện thì tính theo số phút được cấu hình ở thông tin bệnh viện
                    // Ngược lại => theo cấu hình của lịch
                    if (examinationSchedule.IsUseHospitalConfig)
                    {
                        if (hospitalInfo != null && hospitalInfo.MinutePerPatient > 0)
                        {
                            foreach (var detail in examinationScheduleExtension.ExaminationScheduleDetails)
                            {
                                var totalMinuteExamination = (detail.ToTime ?? 0) - (detail.FromTime ?? 0);
                                if (totalMinuteExamination > 0)
                                {
                                    var maximumPatient = (totalMinuteExamination / hospitalInfo.MinutePerPatient);
                                    if (maximumPatient > 0) detail.MaximumExamination = maximumPatient;
                                }
                            }

                            // Tính lại số ca tối đa theo từng buổi trực
                            // BUỔI SÁNG
                            var sessionTypeMorning = sessionTypes.Where(e => e.Code == "BS").FirstOrDefault();
                            if (sessionTypeMorning != null)
                            {
                                var totalDetailMorningValids = examinationScheduleExtension.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeMorning.Id
                                //&& !e.IsUseHospitalConfig 
                                && !e.Deleted).ToList();
                                if (totalDetailMorningValids != null && totalDetailMorningValids.Any())
                                {
                                    examinationSchedule.MaximumMorningExamination = totalDetailMorningValids.Sum(e => e.MaximumExamination ?? 0);
                                }
                            }
                            // BUỔI CHIỀU
                            var sessionTypeAfternoon = sessionTypes.Where(e => e.Code == "BC").FirstOrDefault();
                            if (sessionTypeAfternoon != null)
                            {
                                var totalDetailAfternoonValids = examinationScheduleExtension.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeAfternoon.Id
                                //&& !e.IsUseHospitalConfig 
                                && !e.Deleted).ToList();
                                if (totalDetailAfternoonValids != null && totalDetailAfternoonValids.Any())
                                {
                                    examinationSchedule.MaximumAfternoonExamination = totalDetailAfternoonValids.Sum(e => e.MaximumExamination ?? 0);
                                }
                            }
                            // NGOÀI GIỜ
                            var sessionTypeOther = sessionTypes.Where(e => e.Code != "BC" && e.Code != "BS").FirstOrDefault();
                            if (sessionTypeOther != null)
                            {
                                var totalDetailOtherValids = examinationScheduleExtension.ExaminationScheduleDetails.Where(e => e.SessionTypeId == sessionTypeOther.Id
                                //&& !e.IsUseHospitalConfig 
                                && !e.Deleted).ToList();
                                if (totalDetailOtherValids != null && totalDetailOtherValids.Any())
                                {
                                    examinationSchedule.MaximumOtherExamination = totalDetailOtherValids.Sum(e => e.MaximumExamination ?? 0);
                                }
                            }

                        }
                    }

                    if (examinationScheduleExtension.ExaminationScheduleDetails != null)
                    {
                        foreach (var detail in examinationScheduleExtension.ExaminationScheduleDetails)
                        {
                            detail.Created = DateTime.Now;
                            detail.Active = true;

                            detail.ImportScheduleId = examinationSchedule.ImportScheduleId;
                            // Thêm giá trị vào datatable detail
                            dataTableScheduleDetail.Rows.Add(detail.Id, detail.ScheduleId, detail.RoomExaminationId, DateTime.Now, examinationSchedule.CreatedBy, null, null, false, true, detail.MaximumExamination, null, detail.FromTime, detail.FromTimeText, detail.ToTime, detail.ToTimeText, detail.IsUseHospitalConfig, detail.SessionTypeId, detail.ImportScheduleId);
                        }
                    }
                    // Thêm giá trị vào datatable
                    dataTableSchedule.Rows.Add(examinationSchedule.Id, examinationSchedule.DoctorId, examinationSchedule.ExaminationDate, DateTime.Now, examinationSchedule.CreatedBy, null, null, false, true, hospitalId, examinationSchedule.SpecialistTypeId, examinationSchedule.MaximumAfternoonExamination, examinationSchedule.MaximumMorningExamination, examinationSchedule.MaximumOtherExamination, null, examinationSchedule.IsUseHospitalConfig, examinationSchedule.ImportScheduleId);
                }

            }

            // Bulk Insert bảng lịch
            if (dataTableSchedule != null && dataTableSchedule.Rows.Count > 0)
            {
                var connectionString = string.Format(configuration.GetConnectionString("MedicalDbContext"));
                using (SqlBulkCopy bc = new SqlBulkCopy(connectionString))
                {
                    bc.DestinationTableName = "ExaminationSchedules";
                    bc.BulkCopyTimeout = 4000;
                    await bc.WriteToServerAsync(dataTableSchedule);
                }
            }
            // Bulk Insert bảng chi tiết ca trực
            if (dataTableScheduleDetail != null && dataTableScheduleDetail.Rows.Count > 0)
            {
                var connectionString = string.Format(configuration.GetConnectionString("MedicalDbContext"));
                using (SqlBulkCopy bc = new SqlBulkCopy(connectionString))
                {
                    bc.DestinationTableName = "ExaminationScheduleDetails";
                    bc.BulkCopyTimeout = 4000;
                    await bc.WriteToServerAsync(dataTableScheduleDetail);
                }
            }
            result = true;
            return result;
        }

        /// <summary>
        /// Import danh sách ca trực của bác sĩ
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="createdBy"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public async Task<AppDomainImportResult> ImportExaminationSchedule(Stream stream, int hospitalId)
        {
            AppDomainImportResult appDomainImportResult = new AppDomainImportResult();
            IList<ExaminationScheduleExtensions> examinationScheduleExtensionResult = new List<ExaminationScheduleExtensions>();
            // Lấy danh sách bác sĩ hiện tại
            var currentDoctorInfos = this.unitOfWork.Repository<Doctors>().GetQueryable()
                .Where(e => !e.Deleted && e.HospitalId == hospitalId).ToList();
            // Lấy danh sách chuyên khoa của bác sĩ
            IList<DoctorDetails> currentDoctorDetailInfos = new List<DoctorDetails>();
            if (currentDoctorInfos != null && currentDoctorInfos.Any())
            {
                var currentDoctorIds = currentDoctorInfos.Select(e => e.Id).Distinct().ToList();
                currentDoctorDetailInfos = this.unitOfWork.Repository<DoctorDetails>().GetQueryable()
                .Where(e => !e.Deleted && currentDoctorIds.Contains(e.DoctorId)).ToList();
            }
            // Lấy danh sách chuyên khoa hiện tại của bệnh viện
            var currentSpecialistTypes = this.unitOfWork.Repository<SpecialistTypes>().GetQueryable()
                .Where(e => !e.Deleted && e.HospitalId == hospitalId).ToList();
            // Lấy danh sách phòng khám hiện tại của bệnh viện
            var currentRoomExaminations = this.unitOfWork.Repository<RoomExaminations>().GetQueryable()
                .Where(e => !e.Deleted && e.HospitalId == hospitalId).ToList();
            // Lấy thông tin danh sách số buổi của bệnh viện
            var sessionTypeMorning = await this.unitOfWork.Repository<SessionTypes>().GetQueryable()
                .Where(e => !e.Deleted && e.HospitalId == hospitalId && e.Code == "BS").FirstOrDefaultAsync();
            var sessionTypeAfternoon = await this.unitOfWork.Repository<SessionTypes>().GetQueryable()
                .Where(e => !e.Deleted && e.HospitalId == hospitalId && e.Code == "BC").FirstOrDefaultAsync();
            var sessionTypeOther = await this.unitOfWork.Repository<SessionTypes>().GetQueryable()
                .Where(e => !e.Deleted && e.HospitalId == hospitalId && e.Code != "BS" && e.Code != "BC").FirstOrDefaultAsync();
            // Lấy danh sách lịch hiện tại của bệnh viện
            var examinationScheduleChecks = this.Queryable.Where(e => !e.Deleted
            && e.HospitalId == hospitalId
            );
            using (var package = new ExcelPackage(stream))
            {
                int totalFailed = 0;
                int totalSuccess = 0;
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                    throw new Exception("Sheet name không tồn tại");
                var examinationScheduleMappers = new ExcelMapper(stream) { HeaderRow = false, MinRowNumber = 4 }.Fetch<ExaminationScheduleMapper>()
                    .ToList();
                if (examinationScheduleMappers != null && examinationScheduleMappers.Any())
                {
                    // Loop => gán lại giá trị cho những row merge
                    string doctorCode = string.Empty;
                    string specialistTypeCode = string.Empty;
                    string fromDate_s = null;
                    string toDate_s = null;
                    int? maximumExaminationMorning = null;
                    int? maximumExaminationAfternoon = null;
                    int? maximumExaminationOther = null;
                    foreach (var examinationScheduleMapper in examinationScheduleMappers)
                    {
                        if (!string.IsNullOrEmpty(examinationScheduleMapper.SpecialistTypeCode))
                            specialistTypeCode = examinationScheduleMapper.SpecialistTypeCode;
                        else examinationScheduleMapper.SpecialistTypeCode = specialistTypeCode;
                        if (!string.IsNullOrEmpty(examinationScheduleMapper.DoctorCode))
                            doctorCode = examinationScheduleMapper.DoctorCode;
                        else examinationScheduleMapper.DoctorCode = doctorCode;
                        if (!string.IsNullOrEmpty(examinationScheduleMapper.FromDate_s))
                            fromDate_s = examinationScheduleMapper.FromDate_s;
                        else examinationScheduleMapper.FromDate_s = fromDate_s;
                        if (!string.IsNullOrEmpty(examinationScheduleMapper.ToDate_s))
                            toDate_s = examinationScheduleMapper.ToDate_s;
                        else examinationScheduleMapper.ToDate_s = toDate_s;
                        if (examinationScheduleMapper.MaximumExaminationMorning.HasValue)
                            maximumExaminationMorning = examinationScheduleMapper.MaximumExaminationMorning;
                        else examinationScheduleMapper.MaximumExaminationMorning = maximumExaminationMorning;
                        if (examinationScheduleMapper.MaximumExaminationAfternoon.HasValue)
                            maximumExaminationAfternoon = examinationScheduleMapper.MaximumExaminationAfternoon;
                        else examinationScheduleMapper.MaximumExaminationAfternoon = maximumExaminationAfternoon;

                        if (examinationScheduleMapper.MaximumExaminationOther.HasValue)
                            maximumExaminationOther = examinationScheduleMapper.MaximumExaminationOther;
                        else examinationScheduleMapper.MaximumExaminationOther = maximumExaminationOther;
                    }

                    // Danh sách lịch trực group theo từng thông tin bác sĩ + chuyên khoa + theo ngày
                    var examinationScheduleExtensions = examinationScheduleMappers.GroupBy(e => new
                    {
                        e.SpecialistTypeCode,
                        e.DoctorCode,
                        e.FromDate_s,
                        e.ToDate_s,
                    }).Select(e => new ExaminationScheduleExtensions()
                    {
                        DoctorCode = e.Key.DoctorCode,
                        SpecialistTypeCode = e.Key.SpecialistTypeCode,
                        FromDate = !string.IsNullOrEmpty(e.Key.FromDate_s) ? e.Key.FromDate_s.ToDate() : null,
                        ToDate = !string.IsNullOrEmpty(e.Key.ToDate_s) ? e.Key.ToDate_s.ToDate() : null,
                        MaximumAfternoonExamination = e.Sum(s => s.MaximumExaminationAfternoon ?? 0),
                        MaximumOtherExamination = e.Sum(s => s.MaximumExaminationOther ?? 0),
                        MaximumMorningExamination = e.Sum(s => s.MaximumExaminationMorning ?? 0),
                        IsUseHospitalConfig = true,
                        HospitalId = hospitalId,
                        ExamintionScheduleDetailExtensions = e.Select(s => new ExamintionScheduleDetailExtensions()
                        {
                            FromTimeText = s.FromTime,
                            MaximumExamination = s.MaximumExamination,
                            ToTimeText = s.ToTime,
                            RoomExaminationCode = s.RoomExaminationCode
                        }).ToList()
                    }).ToList();
                    if (examinationScheduleExtensions != null && examinationScheduleExtensions.Any())
                    {
                        // Set giá trị index cho item sau khi group
                        int importIndex = 0;
                        int totalDetailItem = 0;
                        foreach (var examinationScheduleExtension in examinationScheduleExtensions)
                        {
                            if (importIndex == 0)
                            {
                                examinationScheduleExtension.ImportIndex = 1;
                                totalDetailItem = examinationScheduleExtension.ExamintionScheduleDetailExtensions.Count();
                                importIndex++;
                                continue;
                            }
                            examinationScheduleExtension.ImportIndex = totalDetailItem + 1;
                            totalDetailItem += examinationScheduleExtension.ExamintionScheduleDetailExtensions.Count();
                            importIndex++;
                        }


                        Parallel.ForEach(examinationScheduleExtensions, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, item =>
                        {
                            int index = examinationScheduleExtensions.IndexOf(item);
                            int resultIndex = (item.ImportIndex ?? 0) + 4;
                            IList<string> errors = new List<string>();
                            IList<ExaminationScheduleDetails> examinationScheduleDetails = new List<ExaminationScheduleDetails>();
                            // Kiểm tra mã bác sĩ
                            if (string.IsNullOrEmpty(item.DoctorCode)) errors.Add("Vui lòng nhập mã bác sĩ");
                            else
                            {
                                var doctorInfo = currentDoctorInfos.Where(e => e.Code == item.DoctorCode).FirstOrDefault();
                                if (doctorInfo == null) errors.Add("Không tìm thấy thông tin bác sĩ");
                                else item.DoctorId = doctorInfo.Id;
                            }
                            // Kiểm tra mã chuyên khoa
                            if (string.IsNullOrEmpty(item.SpecialistTypeCode)) errors.Add("Vui lòng nhập mã chuyên khoa");
                            else
                            {
                                var specialistTypeInfo = currentSpecialistTypes.Where(e => e.Code == item.SpecialistTypeCode).FirstOrDefault();
                                if (specialistTypeInfo == null) errors.Add("Không tìm thấy thông tin chuyên khoa");
                                else item.SpecialistTypeId = specialistTypeInfo.Id;
                            }
                            // Kiểm tra từ ngày/đến ngày
                            if (!item.FromDate.HasValue) errors.Add("Vui lòng chọn từ ngày");
                            if (!item.ToDate.HasValue) errors.Add("Vui lòng chọn đến ngày");
                            if (item.FromDate.HasValue && item.ToDate.HasValue && item.FromDate.Value > item.ToDate.Value)
                                errors.Add("Từ ngày phải nhỏ hơn đến ngày");

                            // Kiểm tra đã có lịch tồn tại trong khoảng ngày chưa?
                            lock (examinationScheduleExtensionResult)
                            {
                                if (examinationScheduleChecks.Any(x => !x.Deleted
                             && x.ExaminationDate >= item.FromDate && x.ExaminationDate <= item.ToDate
                             && x.DoctorId == item.DoctorId
                            ) && item.FromDate.HasValue && item.ToDate.HasValue)
                                    errors.Add(string.Format("Trong khoảng từ ngày {0} đến ngày {1} đã có lịch tồn tại", item.FromDate.Value.ToString("dd/MM/yyyy"), item.ToDate.Value.ToString("dd/MM/yyyy")));
                            }
                            // Kiểm tra thông tin chi tiết ca trực
                            item.ExaminationScheduleDetails = new List<ExaminationScheduleDetails>();
                            if (item.ExamintionScheduleDetailExtensions.Any())
                            {
                                foreach (var examinationScheduleDetailExtension in item.ExamintionScheduleDetailExtensions)
                                {
                                    bool isErrorDetail = false;
                                    int? roomExaminationId = null;
                                    int indexDetail = item.ExamintionScheduleDetailExtensions.IndexOf(examinationScheduleDetailExtension);
                                    if (string.IsNullOrEmpty(examinationScheduleDetailExtension.RoomExaminationCode))
                                    {
                                        isErrorDetail = true;
                                        errors.Add("Vui lòng nhập mã phòng");
                                    }
                                    else
                                    {
                                        var roomExaminationInfo = currentRoomExaminations.Where(e => e.Code == examinationScheduleDetailExtension.RoomExaminationCode).FirstOrDefault();
                                        if (roomExaminationInfo == null) errors.Add("Không tìm thấy thông tin phòng khám");
                                        else roomExaminationId = roomExaminationInfo.Id;
                                    }
                                    if (string.IsNullOrEmpty(examinationScheduleDetailExtension.FromTimeText))
                                    {
                                        isErrorDetail = true;
                                        errors.Add("Vui lòng nhập từ giờ");
                                    }
                                    if (string.IsNullOrEmpty(examinationScheduleDetailExtension.ToTimeText))
                                    {
                                        isErrorDetail = true;
                                        errors.Add("Vui lòng nhập đến giờ");
                                    }
                                    int fromTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetailExtension.FromTimeText);
                                    int toTime = DateTimeUtilities.ConvertTimeToTotalIMinute(examinationScheduleDetailExtension.ToTimeText);




                                    if (fromTime > toTime)
                                    {
                                        isErrorDetail = true;
                                        errors.Add("Từ giờ phải nhỏ hơn đến giờ");
                                    }
                                    string fromTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(fromTime);
                                    string toTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(toTime);
                                    // Kiểm tra khoảng thời gian đã tồn tại trong file chưa
                                    if (item.ExaminationScheduleDetails.Any(x => (x.FromTime <= fromTime && x.ToTime >= fromTime)
                                     || (x.FromTime <= toTime && x.ToTime >= toTime)
                                    ))
                                    {
                                        errors.Add(string.Format("Khung giờ {0} đã tồn tại!", examinationScheduleDetailExtension.FromTimeText + " - " + examinationScheduleDetailExtension.ToTimeText));
                                        isErrorDetail = true;
                                    }
                                    if (isErrorDetail) break;
                                    bool isUseHospitalConfig = false;
                                    if (examinationScheduleDetailExtension.MaximumExamination.HasValue && examinationScheduleDetailExtension.MaximumExamination.Value > 0) isUseHospitalConfig = true;
                                    // Check khung giờ buổi sáng
                                    if (sessionTypeMorning != null && fromTime >= sessionTypeMorning.FromTime && fromTime <= sessionTypeMorning.ToTime)
                                    {
                                        // Thuộc buổi sáng
                                        if (toTime <= sessionTypeMorning.ToTime)
                                        {
                                            // Thêm chi tiết ca trực
                                            ExaminationScheduleDetails examinationScheduleDetailMorning = new ExaminationScheduleDetails()
                                            {
                                                Deleted = false,
                                                Active = true,
                                                FromTime = fromTime,
                                                ToTime = toTime,
                                                RoomExaminationId = roomExaminationId ?? 0,
                                                MaximumExamination = examinationScheduleDetailExtension.MaximumExamination,
                                                SessionTypeId = sessionTypeMorning.Id,
                                                FromTimeText = fromTimeText,
                                                ToTimeText = toTimeText,
                                                IsUseHospitalConfig = isUseHospitalConfig
                                            };
                                            item.ExaminationScheduleDetails.Add(examinationScheduleDetailMorning);
                                        }
                                        else
                                        {
                                            // Thêm chi tiết ca trực buổi chiều
                                            if (sessionTypeAfternoon != null && toTime <= sessionTypeAfternoon.ToTime)
                                            {
                                                ExaminationScheduleDetails examinationScheduleDetailAfternoon = new ExaminationScheduleDetails()
                                                {
                                                    Deleted = false,
                                                    Active = true,
                                                    FromTime = sessionTypeAfternoon.FromTime,
                                                    ToTime = toTime,
                                                    FromTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(sessionTypeAfternoon.FromTime ?? 0),
                                                    ToTimeText = toTimeText,
                                                    RoomExaminationId = roomExaminationId ?? 0,
                                                    MaximumExamination = examinationScheduleDetailExtension.MaximumExamination,
                                                    SessionTypeId = sessionTypeAfternoon.Id,
                                                    IsUseHospitalConfig = isUseHospitalConfig
                                                };
                                                item.ExaminationScheduleDetails.Add(examinationScheduleDetailAfternoon);
                                            }
                                            // Thêm chi tiết ca trực ngoài giờ
                                            else if (sessionTypeOther != null && toTime >= sessionTypeOther.FromTime)
                                            {
                                                ExaminationScheduleDetails examinationScheduleDetailAfternoon = new ExaminationScheduleDetails()
                                                {
                                                    Deleted = false,
                                                    Active = true,
                                                    FromTime = sessionTypeOther.FromTime,
                                                    ToTime = toTime,
                                                    FromTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(sessionTypeOther.FromTime ?? 0),
                                                    ToTimeText = toTimeText,
                                                    RoomExaminationId = roomExaminationId ?? 0,
                                                    MaximumExamination = examinationScheduleDetailExtension.MaximumExamination,
                                                    SessionTypeId = sessionTypeOther.Id,
                                                    IsUseHospitalConfig = isUseHospitalConfig
                                                };
                                                item.ExaminationScheduleDetails.Add(examinationScheduleDetailAfternoon);
                                            }
                                        }
                                    }
                                    // Check khung giờ buổi chiều
                                    else if (sessionTypeAfternoon != null && fromTime >= sessionTypeAfternoon.FromTime && fromTime <= sessionTypeAfternoon.ToTime)
                                    {
                                        // Thuộc buổi sáng
                                        if (toTime <= sessionTypeAfternoon.ToTime)
                                        {
                                            // Thêm chi tiết ca trực
                                            ExaminationScheduleDetails examinationScheduleDetailAfternoon = new ExaminationScheduleDetails()
                                            {
                                                Deleted = false,
                                                Active = true,
                                                FromTime = fromTime,
                                                ToTime = toTime,
                                                FromTimeText = fromTimeText,
                                                ToTimeText = toTimeText,

                                                RoomExaminationId = roomExaminationId ?? 0,
                                                MaximumExamination = examinationScheduleDetailExtension.MaximumExamination,
                                                SessionTypeId = sessionTypeAfternoon.Id,
                                                IsUseHospitalConfig = isUseHospitalConfig
                                            };
                                            item.ExaminationScheduleDetails.Add(examinationScheduleDetailAfternoon);
                                        }
                                        else
                                        {
                                            if (sessionTypeOther != null && toTime >= sessionTypeOther.FromTime)
                                            {
                                                ExaminationScheduleDetails examinationScheduleDetailAfternoon = new ExaminationScheduleDetails()
                                                {
                                                    Deleted = false,
                                                    Active = true,
                                                    FromTime = sessionTypeOther.FromTime,
                                                    ToTime = toTime,
                                                    FromTimeText = DateTimeUtilities.ConvertTotalMinuteToStringText(sessionTypeOther.FromTime ?? 0),
                                                    ToTimeText = toTimeText,
                                                    RoomExaminationId = roomExaminationId ?? 0,
                                                    MaximumExamination = examinationScheduleDetailExtension.MaximumExamination,
                                                    SessionTypeId = sessionTypeOther.Id,
                                                    IsUseHospitalConfig = isUseHospitalConfig
                                                };
                                                item.ExaminationScheduleDetails.Add(examinationScheduleDetailAfternoon);
                                            }
                                        }
                                    }
                                    // Còn lại là khung ngoài giờ
                                    else
                                    {
                                        if (sessionTypeOther != null && toTime >= sessionTypeOther.FromTime)
                                        {
                                            ExaminationScheduleDetails examinationScheduleDetailAfternoon = new ExaminationScheduleDetails()
                                            {
                                                Deleted = false,
                                                Active = true,
                                                FromTime = fromTime,
                                                ToTime = toTime,
                                                FromTimeText = fromTimeText,
                                                ToTimeText = toTimeText,
                                                RoomExaminationId = roomExaminationId ?? 0,
                                                MaximumExamination = examinationScheduleDetailExtension.MaximumExamination,
                                                SessionTypeId = sessionTypeOther.Id,
                                                IsUseHospitalConfig = isUseHospitalConfig
                                            };
                                            item.ExaminationScheduleDetails.Add(examinationScheduleDetailAfternoon);
                                        }
                                    }
                                }
                            }
                            // Có lỗi  
                            if (errors.Any())
                            {
                                ws.Cells["L" + resultIndex].Value = string.Join(", ", errors);
                                totalFailed += 1;
                            }
                            else
                            {
                                ws.Cells["L" + resultIndex].Value = "Thành công";
                                totalSuccess += 1;
                                lock (examinationScheduleExtensionResult)
                                {
                                    List<DateTime> examinationDates = new List<DateTime>();
                                    TimeSpan ts = new TimeSpan(0, 0, 0, 0);
                                    var fromDate = item.FromDate.Value.Date + ts;
                                    var toDate = item.ToDate.Value.Date + ts;

                                    while (fromDate.Date <= toDate.Date)
                                    {
                                        examinationDates.Add(fromDate);
                                        fromDate = fromDate.AddDays(1);
                                    }
                                    item.ExaminationDates = examinationDates;
                                    examinationScheduleExtensionResult.Add(item);
                                }
                            }
                        });
                    }
                }
                ws.Column(12).Hidden = false;
                ws.Cells.AutoFitColumns();
                appDomainImportResult.Data = new
                {
                    TotalSuccess = totalSuccess,
                    TotalFailed = totalFailed,
                };
                appDomainImportResult.ResultFile = package.GetAsByteArray();
            }
            // Cập nhật lịch trực cho bác sĩ
            if (examinationScheduleExtensionResult != null && examinationScheduleExtensionResult.Any())
                await this.AddMultipleExaminationSchedule(examinationScheduleExtensionResult, hospitalId);

            return appDomainImportResult;
        }

        /// <summary>
        /// Kiểm tra lịch đã tồn tại trong hệ thống chưa
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(ExaminationSchedules item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistSchedule = await Queryable.AnyAsync(x => !x.Deleted
            && x.DoctorId == item.DoctorId
            && x.Id != item.Id
            && x.ExaminationDate.Date == item.ExaminationDate.Date
            && x.HospitalId == item.HospitalId
            );
            if (isExistSchedule)
                messages.Add(string.Format("Bác sĩ đã có lịch tại ngày {0}", item.ExaminationDate.ToString("dd/MM/yyyy")));

            // Nếu có bác sĩ thay thế => kiểm tra thông tin lịch bs thay thế
            if (item.Id > 0 && item.ReplaceDoctorId.HasValue && item.ReplaceDoctorId.Value > 0)
            {
                if (item.ReplaceDoctorId.Value == item.DoctorId)
                    messages.Add("Vui lòng chọn bác sĩ thay thế khác!");

                // Kiểm tra bác sĩ thay thế có lịch đăng kí chưa
                bool isExistReplaceDoctorSchedule = await Queryable.AnyAsync(e => !e.Deleted && e.Active
                && e.ExaminationDate.Date == item.ExaminationDate.Date
                && e.HospitalId == item.HospitalId
                && e.DoctorId == item.ReplaceDoctorId.Value
                );
                if (isExistReplaceDoctorSchedule)
                    messages.Add(string.Format("Bác sĩ thay thế đã có lịch tại ngày {0}", item.ExaminationDate.ToString("dd/MM/yyyy")));
            }

            // Kiểm tra có dk trùng ca khám không?
            if (item.ExaminationScheduleDetails != null && item.ExaminationScheduleDetails.Any())
            {
                foreach (var detail in item.ExaminationScheduleDetails)
                {
                    if (detail.Deleted)
                        continue;
                    var index = item.ExaminationScheduleDetails.IndexOf(detail);
                    if (detail.FromTime.HasValue && detail.ToTime.HasValue
                        && item.ExaminationScheduleDetails.Any(x => item.ExaminationScheduleDetails.IndexOf(x) != index
                    && detail.FromTime >= x.FromTime && detail.FromTime <= x.ToTime
                    && detail.ToTime >= x.FromTime && detail.ToTime <= x.ToTime
                    ))
                    {
                        messages.Add(string.Format("Ca {0} - {1} đã được đăng ký", detail.FromTimeText, detail.ToTimeText));
                        break;
                    }
                }
            }
            if (messages.Any())
                result = string.Join("; ", messages);
            return result;
        }

        /// <summary>
        /// Danh sách lịch theo chuyên khoa và bệnh viện
        /// </summary>
        /// <param name="searchExaminationScheduleDetail"></param>
        /// <returns></returns>
        public async Task<IList<ExaminationSchedules>> GetExaminationSchedules(SearchExaminationScheduleForm searchExaminationScheduleDetail)
        {
            IList<ExaminationSchedules> examinationSchedules = new List<ExaminationSchedules>();
            IList<ExaminationScheduleDetails> examinationScheduleDetails = new List<ExaminationScheduleDetails>();
            var examinationDate = searchExaminationScheduleDetail.ExaminationDate.HasValue ? searchExaminationScheduleDetail.ExaminationDate.Value : DateTime.Now;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchExaminationScheduleDetail.HospitalId),
                new SqlParameter("@SpecialistTypeId", searchExaminationScheduleDetail.SpecialistTypeId),
                new SqlParameter("@ExaminationDate", examinationDate),
                new SqlParameter("@DoctorId", searchExaminationScheduleDetail.DoctorId),
                new SqlParameter("@ExaminationScheduleDetailId", searchExaminationScheduleDetail.ExaminationScheduleDetailId),

            };
            examinationScheduleDetails = await unitOfWork.Repository<ExaminationScheduleDetails>().ExcuteStoreAsync("ExaminationScheduleDetail_GetInfo", sqlParameters);
            examinationSchedules = examinationScheduleDetails
                .GroupBy(e => e.ScheduleId)
                .Select(e => new ExaminationSchedules()
                {
                    Id = e.Key,
                    DoctorName = e.FirstOrDefault().DoctorDisplayName,
                    SpecialistTypeName = e.FirstOrDefault().SpecialistTypeName,
                    SpecialistTypeId = searchExaminationScheduleDetail.SpecialistTypeId ?? 0,
                    HospitalId = searchExaminationScheduleDetail.HospitalId,
                    DoctorId = e.FirstOrDefault().DoctorId ?? 0,
                    ExaminationDate = examinationDate,
                    ExaminationScheduleDetails = e.ToList()
                }).ToList();
            return examinationSchedules;
        }

        public async Task<IList<ExaminationScheduleDetails>> GetExaminationScheduleDetails(SearchExaminationScheduleForm searchExaminationScheduleDetail)
        {
            IList<ExaminationScheduleDetails> examinationScheduleDetails = new List<ExaminationScheduleDetails>();
            var examinationDate = searchExaminationScheduleDetail.ExaminationDate.HasValue ? searchExaminationScheduleDetail.ExaminationDate.Value : DateTime.Now;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchExaminationScheduleDetail.HospitalId),
                new SqlParameter("@SpecialistTypeId", searchExaminationScheduleDetail.SpecialistTypeId),
                new SqlParameter("@ExaminationDate", examinationDate),
                new SqlParameter("@DoctorId", searchExaminationScheduleDetail.DoctorId),
                new SqlParameter("@ExaminationScheduleDetailId", searchExaminationScheduleDetail.ExaminationScheduleDetailId),


            };
            examinationScheduleDetails = await unitOfWork.Repository<ExaminationScheduleDetails>().ExcuteStoreAsync("ExaminationScheduleDetail_GetInfo", sqlParameters);
            return examinationScheduleDetails;
        }

        /// <summary>
        /// Lấy danh sách tất cả lịch + chuyên khoa khám bệnh
        /// </summary>
        /// <param name="searchExaminationScheduleDetailV2"></param>
        /// <returns></returns>
        public async Task<PagedList<ExaminationSchedules>> GetAllExaminationSchedules(SearchExaminationScheduleDetailV2 searchExaminationScheduleDetailV2)
        {
            PagedList<ExaminationSchedules> pagedList = new PagedList<ExaminationSchedules>();
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
                new SqlParameter("@Month", searchExaminationScheduleDetailV2.Month),
                new SqlParameter("@Year", searchExaminationScheduleDetailV2.Year),

                new SqlParameter("@DoctorTypeId", searchExaminationScheduleDetailV2.DoctorTypeId),
                new SqlParameter("@OrderBy", searchExaminationScheduleDetailV2.OrderBy),
                new SqlParameter("@SearchContent", searchExaminationScheduleDetailV2.SearchContent),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            pagedList = await unitOfWork.Repository<ExaminationSchedules>().ExcuteQueryPagingAsync("ExaminationScheduleDetailV2_GetInfo", sqlParameters);
            pagedList.PageSize = searchExaminationScheduleDetailV2.PageSize;
            pagedList.PageIndex = searchExaminationScheduleDetailV2.PageIndex;

            if (pagedList != null && pagedList.Items.Any())
            {
                foreach (var schedule in pagedList.Items)
                {
                    int totalExaminationAfternoon = 0;
                    int totalExaminationMorning = 0;
                    int totalExaminationOther = 0;
                    schedule.ConfigTimeExaminationDayOfWeeks = schedule.ConfigTimeExaminationDayOfWeekValue;
                    IList<ConfigTimeExaminationDayOfWeek> configTimeExaminationDayOfWeeks = new List<ConfigTimeExaminationDayOfWeek>();
                    if (schedule.ConfigTimeExaminationDayOfWeeks != null && schedule.ConfigTimeExaminationDayOfWeeks.Any())
                    {

                        foreach (var detail in schedule.ConfigTimeExaminationDayOfWeeks)
                        {
                            if (!detail.ExaminationScheduleDetailId.HasValue) continue;
                            int totalUserExamination = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                                .Where(e => !e.Deleted && e.Active
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.New
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.Canceled
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed
                                && e.ExaminationScheduleDetailId == detail.ExaminationScheduleDetailId.Value
                                ).CountAsync();
                            // Đếm số phiếu khám của user buổi sáng
                            if (detail.SessionTypeCode == CatalogueUtilities.SessionType.BS.ToString())
                            {
                                totalExaminationMorning += totalUserExamination;
                                if (detail.MaximumExamination.HasValue && totalUserExamination >= detail.MaximumExamination.Value)
                                    detail.IsMaximum = true;
                                configTimeExaminationDayOfWeeks.Add(detail);
                                continue;
                            }
                            // Đếm số phiếu khám của user buổi chiều
                            if (detail.SessionTypeCode == CatalogueUtilities.SessionType.BC.ToString())
                            {

                                totalExaminationAfternoon += totalUserExamination;
                                if (detail.MaximumExamination.HasValue && totalUserExamination >= detail.MaximumExamination.Value)
                                    detail.IsMaximum = true;
                                configTimeExaminationDayOfWeeks.Add(detail);
                                continue;
                            }

                            totalExaminationOther += totalUserExamination;
                            if (detail.MaximumExamination.HasValue && totalUserExamination >= detail.MaximumExamination.Value)
                                detail.IsMaximum = true;
                            configTimeExaminationDayOfWeeks.Add(detail);
                        }

                        schedule.ConfigTimeExaminationDayOfWeeks = configTimeExaminationDayOfWeeks;
                    }

                    // So với tổng số ca khám buổi chiều
                    if (schedule.MaximumMorningExamination.HasValue && schedule.MaximumMorningExamination.Value > 0
                        && totalExaminationMorning > 0
                        && totalExaminationMorning >= schedule.MaximumMorningExamination.Value
                        )
                        schedule.IsMaximumMorning = true;

                    // So với tổng số ca khám buổi chiều
                    if (schedule.MaximumAfternoonExamination.HasValue && schedule.MaximumAfternoonExamination.Value > 0
                        && totalExaminationAfternoon > 0
                        && totalExaminationAfternoon >= schedule.MaximumAfternoonExamination.Value
                        )
                    {
                        schedule.IsMaximumAfternoon = true;
                    }

                    // So với tổng số ca khám buổi khác
                    if (schedule.MaximumOtherExamination.HasValue && schedule.MaximumOtherExamination.Value > 0
                        && totalExaminationOther > 0
                        && totalExaminationOther >= schedule.MaximumOtherExamination.Value
                        )
                    {
                        schedule.IsMaximumOther = true;
                    }

                }
            }

            return pagedList;
        }

        /// <summary>
        /// Xóa tất cả ca trực theo thông tin phòng khám được chọn
        /// </summary>
        /// <param name="roomIds">Danh sách phòng khám</param>
        /// <param name="deletedBy">Người thực hiện xóa</param>
        /// <returns></returns>
        public async Task<bool> DeleteRoomExaminationSchedule(List<int> roomIds)
        {
            using (var contextTransaction = await this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // LẤY RA TẤT CẢ CA TRỰC THUỘC PHÒNG ĐƯỢC CHỌN XÓA
                    var examinationScheduleDetailSelecteds = await this.unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                        .Where(e => !e.Deleted && roomIds.Contains(e.RoomExaminationId)).ToListAsync();

                    // XÓA TÁT CẢ CA TRỰC THEO THÔNG TIN PHÒNG ĐƯỢC CHỌN
                    this.DeleteExaminationScheduleDetail(examinationScheduleDetailSelecteds);

                    // Lấy ra danh sách ID của ca trực
                    var examinationScheduleDetailIds = examinationScheduleDetailSelecteds.Select(e => e.Id).ToList();

                    // LẤY RA TẤT CẢ PHIẾU ĐĂNG KÍ KHÁM CỦA CA TRỰC ĐƯỢC CHỌN KHÁM
                    await this.UpdateExaminationFormStatus(examinationScheduleDetailIds);

                    // CẬP NHẬT TRẠNG THÁI PHIẾU ĐĂNG KÍ KHÁM THÀNH HỦY/HOÀN TIỀN

                    //------------------------------------------------------ THÔNG BÁO CHO BÁC SĨ + Y TÁ THÔNG TIN CÁC CA TRỰC BỊ HỦY

                    await this.unitOfWork.SaveAsync();
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    contextTransaction.Rollback();
                    return false;
                }
            }


        }

        #region Private Methods

        /// <summary>
        /// Lấy thông tin datatable của bảng
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private DataTable SetDataTable(string tableName)
        {
            DataTable table = new DataTable();
            var connectionString = string.Format(configuration.GetConnectionString("MedicalDbContext"));
            using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM " + tableName, connectionString))
            {
                adapter.Fill(table);
            };
            return table;
        }

        /// <summary>
        /// CẬP NHẬT LẠI TRẠNG THÁI CỦA PHIẾU ĐĂNG KÍ KHÁM
        /// </summary>
        /// <param name="examinationScheduleDetailIds">Danh sách ca trực được cập nhật</param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        private async Task UpdateExaminationFormStatus(List<int> examinationScheduleDetailIds)
        {
            // LẤY RA DANH SÁCH PHIẾU ĐĂNG KÍ KHÁM THEO DANH SÁCH CA TRỰC ĐƯỢC CHỌN
            var examinationFormInfoSelecteds = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                .Where(e => !e.Deleted && e.ExaminationScheduleDetailId.HasValue
                && examinationScheduleDetailIds.Contains(e.ExaminationScheduleDetailId.Value)
                && (e.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.New
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.WaitReExamination
                                            || e.Status == (int)CatalogueUtilities.ExaminationStatus.WaitConfirm)
                ).ToListAsync();
            // CẬP NHẬT LẠI TRẠNG THÁI ĐĂNG KÍ CỦA PHIẾU KHÁM
            if (examinationFormInfoSelecteds != null && examinationFormInfoSelecteds.Any())
            {
                foreach (var examinationFormSelected in examinationFormInfoSelecteds)
                {
                    // NẾU ĐÃ THANH TOÁN (ĐÃ XÁC NHẬN/ĐÃ XÁC NHẬN TÁI KHÁM) => CHUYỂN VỀ TRẠNG THÁI CHỜ HOÀN TIỀN
                    if (examinationFormSelected.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                        || examinationFormSelected.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination
                        )
                        examinationFormSelected.Status = (int)CatalogueUtilities.ExaminationStatus.WaitRefund;
                    // NHỮNG TRẠNG THÁI CÒN LẠI => CHUYỂN VỀ TRẠNG THÁI HỦY PHIẾU
                    else examinationFormSelected.Status = (int)CatalogueUtilities.ExaminationStatus.Canceled;
                    examinationFormSelected.Updated = DateTime.Now;
                    examinationFormSelected.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[]
                    {
                                            e => e.Status,
                                            e => e.Updated,
                                            e => e.UpdatedBy
                    };
                    this.unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(examinationFormSelected, includeProperties);
                }
                await this.unitOfWork.SaveAsync();


                //------------------------------------------------------ THÔNG BÁO CHO USER CÓ PHIẾU ĐĂNG KÍ KHÁM VỚI CA TRỰC BỊ HỦY/HOÀN TIỀN


            }
        }

        /// <summary>
        /// XÓA THÔNG TIN CHI TIẾT CA TRỰC
        /// </summary>
        /// <param name="examinationScheduleDetailIds"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        private void DeleteExaminationScheduleDetail(List<ExaminationScheduleDetails> examinationScheduleDetailSelecteds)
        {
            if (examinationScheduleDetailSelecteds != null && examinationScheduleDetailSelecteds.Any())
            {
                foreach (var examinationScheduleDetailSelected in examinationScheduleDetailSelecteds)
                {
                    examinationScheduleDetailSelected.Deleted = true;
                    examinationScheduleDetailSelected.Updated = DateTime.Now;
                    examinationScheduleDetailSelected.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;

                    Expression<Func<ExaminationScheduleDetails, object>>[] updateProperties = new Expression<Func<ExaminationScheduleDetails, object>>[]
                    {
                                x => x.Deleted,
                                x => x.Updated,
                                x => x.UpdatedBy
                    };
                    this.unitOfWork.Repository<ExaminationScheduleDetails>().UpdateFieldsSave(examinationScheduleDetailSelected, updateProperties);
                }
            }
        }

        #endregion

    }
}
