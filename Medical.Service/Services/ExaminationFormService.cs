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
using Medical.Utilities;
using Medical.Entities.Extensions;
using System.Linq.Expressions;
using Medical.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medical.Interface.DbContext;
using Hangfire;
using Medical.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Medical.Service
{
    public class ExaminationFormService : DomainService<ExaminationForms, SearchExaminationForm>, IExaminationFormService
    {
        private readonly ISMSConfigurationService sMSConfigurationService;
        private IHttpContextAccessor httpContextAccessor;
        private IConfiguration configuration;
        private IHospitalConfigFeeService hospitalConfigFeeService;
        private INotificationService notificationService;
        public ExaminationFormService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper
            , IServiceProvider serviceProvider
            , IHttpContextAccessor httpContextAccessor
            , IConfiguration configuration
            ) : base(unitOfWork, medicalDbContext, mapper)
        {
            this.sMSConfigurationService = serviceProvider.GetRequiredService<ISMSConfigurationService>();
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
            this.hospitalConfigFeeService = serviceProvider.GetRequiredService<IHospitalConfigFeeService>(); ;
            this.notificationService = serviceProvider.GetRequiredService<INotificationService>();
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationForm_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchExaminationForm baseSearch)
        {
            string statusIds = baseSearch.StatusIds != null ? string.Join(';', baseSearch.StatusIds) : string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@RecordId", baseSearch.RecordId),
                new SqlParameter("@TypeId", baseSearch.TypeId),

                new SqlParameter("@DoctorId", baseSearch.DoctorId),
                new SqlParameter("@Status", baseSearch.Status),


                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@ExaminationFormId", baseSearch.ExaminationFormId),
                new SqlParameter("@ReExaminationDate", baseSearch.ReExaminationDate),
                new SqlParameter("@StatusIds", statusIds),
                new SqlParameter("@IsReExamination", baseSearch.IsReExamination),


                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Cập nhật thông trạng thái lịch hẹn (phiếu khám)
        /// </summary>
        /// <param name="updateExaminationStatus"></param>
        /// <returns></returns>
        public async Task<bool> UpdateExaminationStatus(UpdateExaminationStatus updateExaminationStatus, bool isAdmin = false)
        {
            using (var contextTransactionTask = medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await UpdateExaminationStatusPrivate(updateExaminationStatus, isAdmin);
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
        /// Cập nhật trạng thái phiếu
        /// </summary>
        /// <param name="updateExaminationStatus"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        private async Task UpdateExaminationStatusPrivate(UpdateExaminationStatus updateExaminationStatus, bool isAdmin = false)
        {
            int? action = null;
            var existExaminationFormInfo = await Queryable.Where(e => e.Id == updateExaminationStatus.ExaminationFormId).AsNoTracking().FirstOrDefaultAsync();
            this.medicalDbContext.Entry<ExaminationForms>(existExaminationFormInfo).State = EntityState.Detached;
            var currentExaminationStatus = existExaminationFormInfo.Status;
            Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[] { };

            if (existExaminationFormInfo != null)
            {
                if (updateExaminationStatus.Status.HasValue)
                {
                    existExaminationFormInfo.Status = updateExaminationStatus.Status.Value;
                    existExaminationFormInfo.Updated = DateTime.Now;
                    existExaminationFormInfo.UpdatedBy = updateExaminationStatus.CreatedBy;
                    existExaminationFormInfo.Active = true;
                    if (updateExaminationStatus.PaymentMethodId.HasValue)
                        existExaminationFormInfo.PaymentMethodId = updateExaminationStatus.PaymentMethodId;
                    if (updateExaminationStatus.BankInfoId.HasValue)
                        existExaminationFormInfo.BankInfoId = updateExaminationStatus.BankInfoId;

                    // Kiểm tra loại dịch vụ có phải chích ngừa không => Đã chích hay chưa?
                    //var cnServiceType = await this.unitOfWork.Repository<ServiceTypes>().GetQueryable().Where(e => e.Code == CoreContants.CN_SERVICE_TYPE).FirstOrDefaultAsync();

                    if (updateExaminationStatus.ReExaminationDate.HasValue)
                    {
                        if (updateExaminationStatus.ReExaminationDate.Value.Date < existExaminationFormInfo.ExaminationDate.Date)
                            throw new Exception("Ngày tái khám phải lớn hơn ngày khám hiện tại");
                        existExaminationFormInfo.ReExaminationDate = updateExaminationStatus.ReExaminationDate;
                    }
                    //else throw new Exception("Vui lòng chọn ngày tái khám");
                    switch (updateExaminationStatus.Status)
                    {
                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                            {
                                if (!updateExaminationStatus.PaymentMethodId.HasValue)
                                    throw new Exception("Vui lòng chọn phương thức thanh toán");
                                // NẾU LÀ THANH TOÁN COD => TRẢ RA STT 
                                existExaminationFormInfo = await this.GetExaminationIndex(existExaminationFormInfo);
                                // LẤY RA THÔNG TIN DỊCH VỤ PHÁT SINH ĐỂ TÍNH TOÁN RA PHÍ THANH TOÁN CHO PHIẾU KHÁM
                                List<int> additionServiceIds = null;
                                var additionServiceMappingInfos = await this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().GetQueryable()
                                    .Where(e => !e.Deleted && e.ExaminationFormId == existExaminationFormInfo.Id).ToListAsync();
                                if (additionServiceMappingInfos != null && additionServiceMappingInfos.Any())
                                    additionServiceIds = additionServiceMappingInfos.Select(e => e.AdditionServiceId).ToList();
                                // LẤY RA THÔNG TIN CHI TIẾT DỊCH VỤ PHÁT SINH
                                List<int> additionServiceDetailIds = null;
                                var additionServiceDetailMappingInfos = await this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>().GetQueryable().Where(e => !e.Deleted && e.ExaminationFormId == existExaminationFormInfo.Id).ToListAsync();
                                if (additionServiceDetailMappingInfos != null && additionServiceDetailMappingInfos.Any())
                                    additionServiceDetailIds = additionServiceDetailMappingInfos.Select(e => e.AdditionServiceDetailId).ToList();


                                FeeCaculateExaminationRequest feeCaculateExaminationRequest = new FeeCaculateExaminationRequest()
                                {
                                    HospitalId = existExaminationFormInfo.HospitalId ?? 0,
                                    PaymentMethodId = existExaminationFormInfo.PaymentMethodId.Value,
                                    //ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                                    VaccineTypeId = existExaminationFormInfo.VaccineTypeId,
                                    ExaminationFormId = existExaminationFormInfo.Id,
                                    SpecialistTypeId = existExaminationFormInfo.SpecialistTypeId,
                                    AdditionServiceIds = existExaminationFormInfo.TypeId == 0 ? additionServiceIds : null,
                                    AdditionServiceDetailIds = additionServiceDetailIds
                                };
                                var hospitalConfigFee = await this.hospitalConfigFeeService.GetFeeExamination(feeCaculateExaminationRequest);
                                existExaminationFormInfo.Price = hospitalConfigFee.TotalPayment;
                                action = (int)CatalogueUtilities.ExaminationAction.Update;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Active,
                                    x => x.PaymentMethodId,
                                    x => x.ExaminationIndex,
                                    x => x.Price
                                };
                            }
                            break;

                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                                existExaminationFormInfo = await this.GetExaminationIndex(existExaminationFormInfo);

                                // LẤY RA CẤU HÌNH SỐ PHÚT KHÁM TRUNG BÌNH HIỆN TẠI CỦA BỆNH VIỆN
                                var hospitalInfoConfig = await this.unitOfWork.Repository<Hospitals>().GetQueryable()
                                    .Where(e => e.Id == existExaminationFormInfo.HospitalId).FirstOrDefaultAsync();
                                // Lấy ra chi tiết ca trực đã chọn.
                                var examinationScheduleDetailSelected = await this.unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                                    .Where(e => !e.Deleted && e.Id == existExaminationFormInfo.ExaminationScheduleDetailId).FirstOrDefaultAsync();
                                // TÍNH TOÁN LẤY RA SỐ GIỜ KHÁM CHO USER
                                if (hospitalInfoConfig != null && hospitalInfoConfig.MinutePerPatient > 0
                                    && existExaminationFormInfo.SystemIndex.HasValue
                                    && examinationScheduleDetailSelected != null
                                    )
                                {
                                    // KIỂM TRA GIỚI HẠN CA KHÁM CỦA CA TRỰC VỚI SYSTEM INDEX
                                    // SYSTEMINDEX > SỐ GIỚI HẠN CA TRỰC => KHÔNG HỢP LỆ
                                    if (examinationScheduleDetailSelected.MaximumExamination.HasValue && existExaminationFormInfo.SystemIndex.Value > examinationScheduleDetailSelected.MaximumExamination)
                                    {
                                        throw new AppException("Số lượng đăng kí khám trong ca trực đã đạt tối đa, vui lòng chọn ca trực khác");
                                    }
                                    // Thời gian kết thúc
                                    existExaminationFormInfo.ToTimeExamination = (examinationScheduleDetailSelected.FromTime ?? 0) + (existExaminationFormInfo.SystemIndex.Value * hospitalInfoConfig.MinutePerPatient);
                                    existExaminationFormInfo.ToTimeExaminationText = DateTimeUtilities.ConvertTotalMinuteToStringText(existExaminationFormInfo.ToTimeExamination);
                                    // Thời gian bắt đầu
                                    existExaminationFormInfo.FromTimeExamination = existExaminationFormInfo.ToTimeExamination - hospitalInfoConfig.MinutePerPatient;
                                    existExaminationFormInfo.FromTimeExaminationText = DateTimeUtilities.ConvertTotalMinuteToStringText(existExaminationFormInfo.FromTimeExamination);
                                }
                                existExaminationFormInfo.Code = RandomUtilities.RandomString(6);
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Active,
                                    x => x.Code,
                                    x => x.ExaminationIndex,
                                    x => x.ExaminationPaymentIndex,
                                    x => x.PaymentMethodId,
                                    x => x.FromTimeExamination,
                                    x => x.FromTimeExaminationText,
                                    x => x.ToTimeExamination,
                                    x => x.ToTimeExaminationText,
                                    x => x.SystemIndex
                                    //x => x.ExaminationScheduleDetailId,
                                    //x => x.RoomExaminationId
                                };
                            }
                            break;
                        // Nếu trạng thái: đã hủy => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                            {
                                // Kiểm tra nếu phiếu đã xác nhận (đã thanh toán) => chuyển về trạng thái chờ hoàn tiền
                                if (currentExaminationStatus == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                    || currentExaminationStatus == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination
                                    )
                                {
                                    existExaminationFormInfo.Status = (int)CatalogueUtilities.ExaminationStatus.WaitRefund;
                                }
                                action = (int)CatalogueUtilities.ExaminationAction.Cancel;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Active,
                                };
                            }
                            break;
                        // Nếu trạng thái: Chờ xác nhận tái khám => lưu lại ngày tái khám trong phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                            {
                                //action = (int)CatalogueUtilities.ExaminationAction.Update;

                                //if (updateExaminationStatus.ExaminationScheduleDetailId.HasValue && updateExaminationStatus.ExaminationScheduleDetailId.Value > 0)
                                //    existExaminationFormInfo.ExaminationScheduleDetailId = updateExaminationStatus.ExaminationScheduleDetailId.Value;
                                //else throw new Exception("Vui lòng chọn thông tin ca khám");
                                //if (updateExaminationStatus.RoomExaminationId.HasValue && updateExaminationStatus.RoomExaminationId.Value > 0)
                                //    existExaminationFormInfo.RoomExaminationId = updateExaminationStatus.RoomExaminationId.Value;
                                //else throw new Exception("Vui lòng chọn thông tin phòng khám");
                                //if (updateExaminationStatus.DoctorId.HasValue && updateExaminationStatus.DoctorId.Value > 0)
                                //    existExaminationFormInfo.DoctorId = updateExaminationStatus.DoctorId.Value;
                                //FeeCaculateExaminationRequest feeCaculateExaminationRequest = new FeeCaculateExaminationRequest()
                                //{
                                //    HospitalId = existExaminationFormInfo.HospitalId ?? 0,
                                //    PaymentMethodId = existExaminationFormInfo.PaymentMethodId.Value,
                                //    ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                                //    SpecialistTypeId = existExaminationFormInfo.SpecialistTypeId
                                //};
                                //var hospitalConfigFee = await this.hospitalConfigFeeService.GetFeeExamination(feeCaculateExaminationRequest);
                                //existExaminationFormInfo.Price = hospitalConfigFee.TotalPayment;
                                //existExaminationFormInfo.BloodPressure = updateExaminationStatus.BloodPressure;
                                //existExaminationFormInfo.BloodSugar = updateExaminationStatus.BloodSugar;
                                //existExaminationFormInfo.HeartBeat = updateExaminationStatus.HeartBeat;
                                //existExaminationFormInfo.IsReExamination = true;
                                //includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                //{
                                //    x => x.Status,
                                //    x => x.Updated,
                                //    x => x.UpdatedBy,
                                //    x => x.ReExaminationDate,
                                //    x => x.ExaminationScheduleDetailId,
                                //    x => x.RoomExaminationId,
                                //    x => x.DoctorId,
                                //    x => x.Price,
                                //    x => x.BloodSugar,
                                //    x => x.BloodPressure,
                                //    x => x.HeartBeat,
                                //    x => x.IsReExamination
                                //};
                            }
                            break;
                        // Nếu trạng thái: đã xác nhận tái khám => lưu lại ngày tái khám trong phiếu khám + lịch sử phiếu
                        //case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                        //    {
                        //        action = (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination;
                        //        existExaminationFormInfo = await this.GetExaminationIndex(existExaminationFormInfo);
                        //        //if (existExaminationFormInfo.ReExaminationDate.HasValue)
                        //        //{
                        //        //    existExaminationFormInfo.ExaminationDate = existExaminationFormInfo.ReExaminationDate.Value;
                        //        //    existExaminationFormInfo.ReExaminationDate = null;
                        //        //}
                        //        //else throw new Exception("Vui lòng chọn ngày tái khám");

                        //        // LẤY RA CẤU HÌNH SỐ PHÚT KHÁM TRUNG BÌNH HIỆN TẠI CỦA BỆNH VIỆN
                        //        var hospitalInfoConfig = await this.unitOfWork.Repository<Hospitals>().GetQueryable()
                        //            .Where(e => e.Id == existExaminationFormInfo.HospitalId).FirstOrDefaultAsync();
                        //        // Lấy ra chi tiết ca trực đã chọn.
                        //        var examinationScheduleDetailSelected = await this.unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable()
                        //            .Where(e => !e.Deleted && e.Id == existExaminationFormInfo.ExaminationScheduleDetailId).FirstOrDefaultAsync();
                        //        // TÍNH TOÁN LẤY RA SỐ GIỜ KHÁM CHO USER
                        //        if (hospitalInfoConfig != null && hospitalInfoConfig.MinutePerPatient > 0
                        //            && existExaminationFormInfo.SystemIndex.HasValue
                        //            && examinationScheduleDetailSelected != null
                        //            )
                        //        {
                        //            // KIỂM TRA GIỚI HẠN CA KHÁM CỦA CA TRỰC VỚI SYSTEM INDEX
                        //            // SYSTEMINDEX > SỐ GIỚI HẠN CA TRỰC => KHÔNG HỢP LỆ
                        //            if (examinationScheduleDetailSelected.MaximumExamination.HasValue && existExaminationFormInfo.SystemIndex.Value > examinationScheduleDetailSelected.MaximumExamination)
                        //            {
                        //                throw new AppException("Số lượng đăng kí khám trong ca trực đã đạt tối đa, vui lòng chọn ca trực khác");
                        //            }

                        //            // Thời gian kết thúc
                        //            existExaminationFormInfo.ToTimeExamination = (examinationScheduleDetailSelected.FromTime ?? 0) + (existExaminationFormInfo.SystemIndex.Value * hospitalInfoConfig.MinutePerPatient);
                        //            existExaminationFormInfo.ToTimeExaminationText = DateTimeUtilities.ConvertTotalMinuteToStringText(existExaminationFormInfo.ToTimeExamination);
                        //            // Thời gian bắt đầu
                        //            existExaminationFormInfo.FromTimeExamination = existExaminationFormInfo.ToTimeExamination - hospitalInfoConfig.MinutePerPatient;
                        //            existExaminationFormInfo.FromTimeExaminationText = DateTimeUtilities.ConvertTotalMinuteToStringText(existExaminationFormInfo.FromTimeExamination);

                        //        }

                        //        includeProperties = new Expression<Func<ExaminationForms, object>>[]
                        //        {
                        //            x => x.Status,
                        //            x => x.Updated,
                        //            x => x.UpdatedBy,
                        //            x => x.ExaminationDate,
                        //            x => x.ReExaminationDate,
                        //            x => x.ExaminationIndex,
                        //            x => x.ExaminationPaymentIndex,
                        //            x => x.FromTimeExamination,
                        //            x => x.FromTimeExaminationText,
                        //            x => x.ToTimeExamination,
                        //            x => x.ToTimeExaminationText,
                        //            x => x.SystemIndex
                        //        };
                        //    }
                        //    break;
                        // Nếu trạng thái: Hoàn thành khám => lưu lại ngày tái khám trong phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.FinishExamination;
                                //existExaminationFormInfo.ReExaminationDate = null;
                                // Nếu có tái khám => chọn ngày tái khám
                                if (updateExaminationStatus.ReExaminationDate.HasValue)
                                {
                                    existExaminationFormInfo.ReExaminationDate = updateExaminationStatus.ReExaminationDate.Value;
                                    existExaminationFormInfo.IsReExamination = true;
                                }
                                existExaminationFormInfo.BloodPressure = updateExaminationStatus.BloodPressure;
                                existExaminationFormInfo.BloodSugar = updateExaminationStatus.BloodSugar;
                                existExaminationFormInfo.HeartBeat = updateExaminationStatus.HeartBeat;

                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Active,
                                    x => x.ReExaminationDate,
                                    x => x.BloodSugar,
                                    x => x.BloodPressure,
                                    x => x.HeartBeat,
                                    x => x.IsReExamination
                                };
                            }
                            break;
                        // Nếu trạng thái: trả về cho user, thông tin thanh toán không hợp lệ
                        case (int)CatalogueUtilities.ExaminationStatus.PaymentFailed:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Return;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Active,
                                };
                            }
                            break;
                        // Nếu trạng thái: trả về cho user, thông tin thanh toán không hợp lệ
                        case (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.ReturnReExamination;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Active,
                                };
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            // Thêm lịch sử phiếu khám/lịch hẹn
            if (action.HasValue && updateExaminationStatus.Status.HasValue)
            {
                unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(existExaminationFormInfo, includeProperties);
                await unitOfWork.SaveAsync();
                existExaminationFormInfo.Comment = updateExaminationStatus.Comment;

                switch (updateExaminationStatus.Status)
                {

                    case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                        {
                            if (!isAdmin && currentExaminationStatus == (int)CatalogueUtilities.ExaminationStatus.Canceled)
                            {
                                await this.UpdateUserEditExamination(existExaminationFormInfo.RecordId);
                            }
                        }
                        break;
                    // Nếu trạng thái là chờ xác nhận tái khám hoặc hoàn thành khám bệnh => lưu lại thông tin vào chi tiết hồ sơ bệnh án của bệnh nhân
                    //case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                    case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                        {
                            string examinationIndex = string.Empty;
                            string examinationPaymentIndex = string.Empty;

                            if (existExaminationFormInfo.PaymentMethodId.HasValue)
                            {
                                var paymentMethodInfoTask = this.unitOfWork.Repository<PaymentMethods>().GetQueryable()
                                    .Where(e => e.Id == existExaminationFormInfo.PaymentMethodId.Value).FirstOrDefaultAsync();


                                // Nếu là thanh toán COD => Lấy STT chờ khám
                                if (await paymentMethodInfoTask != null)
                                {
                                    var paymentMethodInfo = await paymentMethodInfoTask;
                                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString()
                                    || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.TRANSFER.ToString()
                                    )
                                        examinationIndex = existExaminationFormInfo.ExaminationIndex;
                                    // Nếu là thanh toán online => Lấy STT khám trực tiếp tại bệnh viện
                                    else
                                        examinationPaymentIndex = existExaminationFormInfo.ExaminationPaymentIndex;
                                }

                            }

                            int? reExaminationScheduleDetailId = null;
                            int? reRoomExaminationId = null;
                            int? reDoctorExaminationId = null;
                            // từ giờ/đến giờ tái khám nếu có
                            string reFromTimeExaminationText = string.Empty;
                            string reToTimeExaminationText = string.Empty;
                            // Nếu là tái khám thì chọn lại ca trực với ngày tái khám tương ứng cho user (nếu có)
                            if (existExaminationFormInfo.IsReExamination && existExaminationFormInfo.ExaminationScheduleDetailId != updateExaminationStatus.ExaminationScheduleDetailId)
                            {
                                reExaminationScheduleDetailId = updateExaminationStatus.ExaminationScheduleDetailId;
                                if (reExaminationScheduleDetailId.HasValue && reExaminationScheduleDetailId.Value > 0)
                                {
                                    var examinationScheduleDetailInfo = await this.unitOfWork.Repository<ExaminationScheduleDetails>()
                                        .GetQueryable().Where(e => !e.Deleted && e.Id == reExaminationScheduleDetailId.Value).FirstOrDefaultAsync();
                                    if (examinationScheduleDetailInfo != null)
                                    {
                                        reRoomExaminationId = examinationScheduleDetailInfo.RoomExaminationId;
                                        reFromTimeExaminationText = examinationScheduleDetailInfo.FromTimeText;
                                        reToTimeExaminationText = examinationScheduleDetailInfo.ToTimeText;
                                        var scheduleInfo = await this.unitOfWork.Repository<ExaminationSchedules>()
                                            .GetQueryable()
                                            .Where(e => !e.Deleted
                                            && e.ImportScheduleId == examinationScheduleDetailInfo.ImportScheduleId).FirstOrDefaultAsync();
                                        if (scheduleInfo != null)
                                        {
                                            reDoctorExaminationId = scheduleInfo.DoctorId;
                                        }
                                    }
                                }
                            }
                            MedicalRecordDetails medicalRecordDetails = new MedicalRecordDetails()
                            {
                                Created = DateTime.Now,
                                CreatedBy = updateExaminationStatus.CreatedBy,
                                Active = true,
                                Deleted = false,
                                ExaminationDate = existExaminationFormInfo.ExaminationDate,
                                ReExaminationDate = existExaminationFormInfo.ReExaminationDate,
                                ExaminationFormId = existExaminationFormInfo.Id,
                                HospitalId = existExaminationFormInfo.HospitalId,
                                MedicalRecordId = existExaminationFormInfo.RecordId,
                                Price = existExaminationFormInfo.Price,
                                //ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                                SpecialistTypeId = existExaminationFormInfo.SpecialistTypeId,
                                HasMedicalBills = updateExaminationStatus.HasMedicalBill,
                                ExaminationScheduleDetailId = existExaminationFormInfo.ExaminationScheduleDetailId,
                                ReExaminationScheduleDetailId = reExaminationScheduleDetailId,
                                DoctorComment = updateExaminationStatus.DoctorComment,
                                ExaminationIndex = examinationIndex,
                                ExaminationPaymentIndex = examinationPaymentIndex,
                                PaymentMethodId = existExaminationFormInfo.PaymentMethodId,
                                TypeId = existExaminationFormInfo.TypeId,
                                DoctorId = existExaminationFormInfo.DoctorId,
                                ReExaminationDoctorId = reDoctorExaminationId,
                                BloodPressure = existExaminationFormInfo.BloodPressure,
                                BloodSugar = existExaminationFormInfo.BloodSugar,
                                HeartBeat = existExaminationFormInfo.HeartBeat,
                                VaccineTypeId = existExaminationFormInfo.VaccineTypeId,
                                DiagnoticSickName = updateExaminationStatus.DiagnoticSickName,
                                DiagnoticTypeId = updateExaminationStatus.DiagnoticTypeId,
                                RoomExaminationId = existExaminationFormInfo.RoomExaminationId,
                                ReRoomExaminationId = reRoomExaminationId,
                                IsReExamination = existExaminationFormInfo.IsReExamination,
                                Code = RandomUtilities.RandomString(8),
                                FromTimeExamination = existExaminationFormInfo.FromTimeExamination,
                                ToTimeExamination = existExaminationFormInfo.ToTimeExamination,
                                FromTimeExaminationText = existExaminationFormInfo.FromTimeExaminationText,
                                ToTimeExaminationText = existExaminationFormInfo.ToTimeExaminationText,
                                ReFromTimeExaminationText = reFromTimeExaminationText,
                                ReToTimeExaminationText = reToTimeExaminationText,
                                Id = 0
                            };

                            await unitOfWork.Repository<MedicalRecordDetails>().CreateAsync(medicalRecordDetails);

                            await unitOfWork.SaveAsync();
                            await this.medicalDbContext.SaveChangesAsync();


                            // Lấy thông tin qrcode hình
                            this.medicalDbContext.Entry<MedicalRecordDetails>(medicalRecordDetails).State = EntityState.Detached;
                            var medicalRecordInfo = await this.medicalDbContext.Set<MedicalRecords>()
                                .Where(e => !e.Deleted && e.Id == existExaminationFormInfo.RecordId)
                                .AsNoTracking().FirstOrDefaultAsync();


                            // Lưu lại thông tin dịch vụ phát sinh khi tái khám.

                            // 1. Lưu lại tất cả thông tin dịch vụ phát sinh HIỆN TẠI vào hồ sơ bệnh án
                            var currentExaminationFormDetails = await this.unitOfWork.Repository<ExaminationFormDetails>().GetQueryable().Where(e => !e.Deleted && e.Active && e.ExaminationFormId == existExaminationFormInfo.Id).ToListAsync();
                            if (currentExaminationFormDetails != null && currentExaminationFormDetails.Any())
                            {
                                foreach (var currentExaminationFormDetail in currentExaminationFormDetails)
                                {
                                    currentExaminationFormDetail.Updated = DateTime.Now;
                                    currentExaminationFormDetail.UpdatedBy = updateExaminationStatus.CreatedBy;
                                    currentExaminationFormDetail.MedicalRecordDetailId = medicalRecordDetails.Id;
                                    Expression<Func<ExaminationFormDetails, object>>[] expressions = new Expression<Func<ExaminationFormDetails, object>>[]
                                    {
                                        e => e.Updated,
                                        e => e.UpdatedBy,
                                        e => e.MedicalRecordDetailId
                                    };
                                    await this.unitOfWork.Repository<ExaminationFormDetails>().UpdateFieldsSaveAsync(currentExaminationFormDetail, expressions);
                                }
                            }
                            // 2. Thêm mặc định dịch vụ xét nghiệm cho đợt tái khám kế tiếp.
                            // CHỈ ÁP DỤNG CHO TRẠNG THÁI CHỜ XÁC NHẬN TÁI KHÁM
                            if (updateExaminationStatus.Status == (int)CatalogueUtilities.ExaminationStatus.WaitReExamination)
                            {
                                var additionServiceXN = this.unitOfWork.Repository<AdditionServices>().GetQueryable().Where(e => e.Code == CatalogueUtilities.AdditionServiceType.XN.ToString()).FirstOrDefault();
                                if (additionServiceXN != null)
                                {
                                    ExaminationFormDetails examinationFormDetails = new ExaminationFormDetails()
                                    {
                                        Created = DateTime.Now,
                                        CreatedBy = updateExaminationStatus.CreatedBy,
                                        Active = true,
                                        Deleted = false,
                                        ExaminationFormId = existExaminationFormInfo.Id,
                                        HospitalId = existExaminationFormInfo.HospitalId,
                                        AdditionServiceId = additionServiceXN.Id,
                                        MedicalRecordId = existExaminationFormInfo.RecordId,
                                        Price = additionServiceXN.Price,
                                        Status = (int)CatalogueUtilities.AdditionServiceStatus.New,
                                        UserId = medicalRecordInfo.UserId,
                                        ExaminationDate = existExaminationFormInfo.ExaminationDate,
                                    };
                                    await this.unitOfWork.Repository<ExaminationFormDetails>().CreateAsync(examinationFormDetails);
                                }
                            }

                            await unitOfWork.SaveAsync();

                            // Thêm qrcode/barcode cho hồ sơ bệnh án
                            QRCodeUtils qRCodeUtils = new QRCodeUtils(configuration, httpContextAccessor);
                            if (medicalRecordInfo != null)
                            {
                                var filePathUrl = qRCodeUtils.GetQrImagePath(medicalRecordInfo.UserId, medicalRecordDetails.Id);
                                var barCodeUrl = qRCodeUtils.GetBarCodeImagePath(medicalRecordDetails.Code);
                                var medicalRecordDetailInfo = await this.unitOfWork.Repository<MedicalRecordDetails>()
                                    .GetQueryable()
                                    .Where(e => e.Id == medicalRecordDetails.Id)
                                    .AsNoTracking().FirstOrDefaultAsync();
                                if (medicalRecordDetailInfo != null)
                                {
                                    medicalRecordDetailInfo.QrCodeUrlFile = filePathUrl;
                                    medicalRecordDetailInfo.BarCodeUrl = barCodeUrl;
                                    Expression<Func<MedicalRecordDetails, object>>[] expressions = new Expression<Func<MedicalRecordDetails, object>>[]
                                    {
                                        e => e.QrCodeUrlFile,
                                        e => e.BarCodeUrl
                                    };
                                    await this.unitOfWork.Repository<MedicalRecordDetails>().UpdateFieldsSaveAsync(medicalRecordDetailInfo, expressions);
                                }
                            }

                            // Thêm hóa đơn toa thuốc
                            //if (updateExaminationStatus.HasMedicalBill && updateExaminationStatus.MedicalBills != null)
                            //{
                            //    var existMedicalBill = await this.unitOfWork.Repository<MedicalBills>().GetQueryable()
                            //        .Where(e => e.Id == updateExaminationStatus.MedicalBills.Id).FirstOrDefaultAsync();
                            //    if (existMedicalBill != null)
                            //    {
                            //        existMedicalBill = mapper.Map<MedicalBills>(updateExaminationStatus.MedicalBills);
                            //        existMedicalBill.Updated = DateTime.Now;
                            //        existMedicalBill.UpdatedBy = updateExaminationStatus.CreatedBy;
                            //        if (updateExaminationStatus.MedicalBills.Medicines != null
                            //            && updateExaminationStatus.MedicalBills.Medicines.Any()
                            //            )
                            //            existMedicalBill.TotalPrice = updateExaminationStatus.MedicalBills.Medicines
                            //                .Sum(e => (e.Price ?? 0) * (e.TotalAmount ?? 0));

                            //        this.unitOfWork.Repository<MedicalBills>().Update(existMedicalBill);
                            //    }
                            //    else
                            //    {
                            //        if (updateExaminationStatus.MedicalBills.Medicines != null
                            //            && updateExaminationStatus.MedicalBills.Medicines.Any()
                            //            )
                            //            updateExaminationStatus.MedicalBills.TotalPrice = updateExaminationStatus.MedicalBills.Medicines
                            //                .Sum(e => (e.Price ?? 0) * (e.TotalAmount ?? 0));


                            //        updateExaminationStatus.MedicalBills.Deleted = false;
                            //        updateExaminationStatus.MedicalBills.Active = true;
                            //        updateExaminationStatus.MedicalBills.Created = DateTime.Now;
                            //        updateExaminationStatus.MedicalBills.Status = (int)CatalogueUtilities.MedicalBillStatus.New;
                            //        updateExaminationStatus.MedicalBills.MedicalRecordDetailId = medicalRecordDetails.Id;
                            //        updateExaminationStatus.MedicalBills.HospitalId = medicalRecordDetails.HospitalId;
                            //        updateExaminationStatus.MedicalBills.ExaminationFormId = medicalRecordDetails.ExaminationFormId;
                            //        updateExaminationStatus.MedicalBills.CreatedBy = updateExaminationStatus.CreatedBy;
                            //        updateExaminationStatus.MedicalBills.MedicalRecordId = existExaminationFormInfo.RecordId;
                            //        updateExaminationStatus.MedicalBills.Id = 0;
                            //        // TH1: Tạo mã toa thuốc tự động
                            //        updateExaminationStatus.MedicalBills.Code = RandomUtilities.RandomString(8);
                            //        // Th2: Lấy mã toa thuốc từ API bệnh viện
                            //        //..............................
                            //        await this.unitOfWork.Repository<MedicalBills>().CreateAsync(updateExaminationStatus.MedicalBills);
                            //    }
                            //    await this.unitOfWork.SaveAsync();
                            //    // Thêm thông tin chi tiết toa thuốc
                            //    if (updateExaminationStatus.MedicalBills.Medicines != null && updateExaminationStatus.MedicalBills.Medicines.Any())
                            //    {
                            //        foreach (var medicine in updateExaminationStatus.MedicalBills.Medicines)
                            //        {
                            //            var existMedicine = await this.unitOfWork.Repository<Medicines>().GetQueryable().Where(e => e.Id == medicine.Id).FirstOrDefaultAsync();
                            //            if (existMedicine != null)
                            //            {
                            //                medicine.MedicalBillId = updateExaminationStatus.MedicalBills.Id;
                            //                existMedicine = mapper.Map<Medicines>(medicine);
                            //                existMedicine.Updated = DateTime.Now;
                            //                existMedicine.UpdatedBy = updateExaminationStatus.CreatedBy;
                            //                this.unitOfWork.Repository<Medicines>().Update(medicine);
                            //            }
                            //            else
                            //            {
                            //                medicine.MedicalBillId = updateExaminationStatus.MedicalBills.Id;
                            //                medicine.Created = DateTime.Now;
                            //                medicine.Active = true;
                            //                medicine.Deleted = false;
                            //                medicine.Id = 0;
                            //                medicine.CreatedBy = updateExaminationStatus.CreatedBy;
                            //                await this.unitOfWork.Repository<Medicines>().CreateAsync(medicine);
                            //            }
                            //        }
                            //        await this.unitOfWork.SaveAsync();
                            //    }

                            //}

                            // Lưu thông tin file toa thuốc/ xét nghiệm/ siêu âm/ ....
                            if (updateExaminationStatus.MedicalRecordDetailFiles != null && updateExaminationStatus.MedicalRecordDetailFiles.Any())
                            {
                                foreach (var item in updateExaminationStatus.MedicalRecordDetailFiles)
                                {
                                    item.Created = DateTime.Now;
                                    item.CreatedBy = updateExaminationStatus.CreatedBy;
                                    item.Id = 0;
                                    item.MedicalRecordDetailId = medicalRecordDetails.Id;
                                    item.UserId = medicalRecordInfo.UserId;
                                    await unitOfWork.Repository<UserFiles>().CreateAsync(item);
                                }
                                await unitOfWork.SaveAsync();
                            }
                        }
                        break;
                    // Nếu trường hợp hủy => check lại account user
                    case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                        {
                            if (!isAdmin)
                            {
                                // Xóa job hiện tại nếu có
                                var latestExaminationScheduleJob = await this.unitOfWork.Repository<ExaminationScheduleJobs>().GetQueryable()
                                    .Where(e => !e.Deleted && e.Active && e.ExaminationFormId == existExaminationFormInfo.Id)
                                    .OrderByDescending(e => e.Created)
                                    .FirstOrDefaultAsync();
                                if (latestExaminationScheduleJob != null)
                                {
                                    this.unitOfWork.Repository<ExaminationScheduleJobs>().Delete(latestExaminationScheduleJob);
                                }

                                // Lấy thông tin job id
                                var jobId = BackgroundJob.Schedule(() => UpdateCancelExaminationJob(existExaminationFormInfo.Id), TimeSpan.FromHours(12));
                                ExaminationScheduleJobs examinationScheduleJobs = new ExaminationScheduleJobs()
                                {
                                    Created = DateTime.UtcNow.AddHours(7),
                                    CreatedBy = updateExaminationStatus.CreatedBy,
                                    ExaminationFormId = existExaminationFormInfo.Id,
                                    JobId = jobId,
                                    Active = true,
                                    Deleted = false,
                                };
                                await this.unitOfWork.Repository<ExaminationScheduleJobs>().CreateAsync(examinationScheduleJobs);
                                await unitOfWork.SaveAsync();
                                //await UpdateUserInfo(existExaminationFormInfo.RecordId);
                            }
                        }
                        break;
                    // Nếu trạng thạng thái là hoàn tiền thành công => gửi sms lại cho user
                    case (int)CatalogueUtilities.ExaminationStatus.RefundSuccess:
                        {
                            var medicalRecordInfo = await unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == existExaminationFormInfo.RecordId).FirstOrDefaultAsync();
                            if (medicalRecordInfo != null)
                            {
                                var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync();
                                var hospitalInfo = await this.unitOfWork.Repository<Hospitals>().GetQueryable().Where(e => !e.Deleted && e.Active).FirstOrDefaultAsync();
                                if (userInfo != null)
                                    await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("Phieu kham cua ban tai benh vien {0} ngay kham {1} da duoc hoan tien!", hospitalInfo.Name, existExaminationFormInfo.ExaminationDate.ToString("dd/MM/yyyy")));
                            }
                        }
                        break;
                    //--------------------------- GỬI SMS STT CHO USER
                    //--------------------------- idnex string
                    //......................................................
                    case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                        //case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                        {
                            var medicalRecordInfo = await unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == existExaminationFormInfo.RecordId).FirstOrDefaultAsync();
                            if (medicalRecordInfo != null)
                            {
                                var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync();
                                if (userInfo != null)
                                {
                                    if (!string.IsNullOrEmpty(existExaminationFormInfo.ExaminationIndex))
                                        await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", existExaminationFormInfo.ExaminationIndex));
                                    else if (!string.IsNullOrEmpty(existExaminationFormInfo.ExaminationPaymentIndex))
                                        await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", existExaminationFormInfo.ExaminationPaymentIndex));
                                }
                            }

                            // Nếu trạng thái hủy => cập nhật số lần chỉnh sửa phiếu cho user
                            if (!isAdmin && currentExaminationStatus == (int)CatalogueUtilities.ExaminationStatus.Canceled)
                            {
                                await this.UpdateUserEditExamination(existExaminationFormInfo.RecordId);
                            }

                        }
                        break;
                    //--------------------------- GỬI SMS THÔNG BÁO CHO USER THÔNG TIN THANH TOÁN KHÔNG HỢP LỆ
                    //--------------------------- idnex string
                    //......................................................
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentFailed:
                        //case (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed:
                        {
                            var medicalRecordInfo = await unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == existExaminationFormInfo.RecordId).FirstOrDefaultAsync();
                            if (medicalRecordInfo != null)
                            {
                                var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync();
                                if (userInfo != null)
                                    await sMSConfigurationService.SendSMS(userInfo.Phone, "Thong tin thanh toan cua ban khong hop le!");
                            }
                        }
                        break;
                    default:
                        break;
                }

                await CreateExaminationHistory(existExaminationFormInfo, updateExaminationStatus.CreatedBy);
            }

            // NẾU ĐỔI LỊCH => CẬP NHẬT LẠI THÔNG TIN CHO LỊCH ĐỔI
            if (updateExaminationStatus.ExaminationFormChangedId.HasValue && updateExaminationStatus.ExaminationFormChangedId.Value > 0)
            {
                // LẤY THÔNG TIN GIÁ TRỊ PHIẾU ĐƯỢC THAY ĐỔI
                var examinationFormChangedInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                    .Where(e => e.Id == updateExaminationStatus.ExaminationFormChangedId.Value).FirstOrDefaultAsync();
                // KIỂM TRA TRẠNG THÁI PHIẾU KHÁM PHẢI LÀ ĐÃ THANH TOÁN HAY CHƯA (ĐÃ XÁC NHẬN)
                if (examinationFormChangedInfo == null || examinationFormChangedInfo.Status != (int)CatalogueUtilities.ExaminationStatus.Confirmed)
                    throw new AppException(string.Format("Mã phiếu được thay đổi {0} có trạng thái không hợp lệ!", examinationFormChangedInfo.Code));
                // GÁN LẠI GIÁ TRỊ PHIẾU THAY ĐỔI
                updateExaminationStatus.ExaminationFormId = updateExaminationStatus.ExaminationFormChangedId.Value;
                // Đặt lại trạng thái hủy cho phiếu
                updateExaminationStatus.Status = (int)CatalogueUtilities.ExaminationStatus.Canceled;
                // Bỏ giá trị phiếu change
                updateExaminationStatus.ExaminationFormChangedId = null;
                await this.UpdateExaminationStatusPrivate(updateExaminationStatus, isAdmin);
            }
        }

        /// <summary>
        /// Tạo mới phiếu khám bệnh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ExaminationForms item)
        {
            if (item == null) throw new AppException("Item không tồn tại");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    item.Id = 0;
                    if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
                        item = await this.GetExaminationIndex(item);

                    // LƯU THÔNG TIN TỔNG TIỀN THANH TOÁN CỦA LỊCH KHÁM
                    FeeCaculateExaminationRequest feeCaculateExaminationRequest = new FeeCaculateExaminationRequest()
                    {
                        HospitalId = item.HospitalId ?? 0,
                        PaymentMethodId = item.PaymentMethodId ?? 0,
                        //ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                        VaccineTypeId = item.VaccineTypeId,
                        ExaminationFormId = item.Id,
                        SpecialistTypeId = item.SpecialistTypeId,
                        AdditionServiceIds = item.TypeId == 0 ? item.AdditionServiceIds : null,
                        AdditionServiceDetailIds = item.AdditionServiceDetailIds
                    };
                    var hospitalConfigFee = await this.hospitalConfigFeeService.GetFeeExamination(feeCaculateExaminationRequest);
                    item.Price = hospitalConfigFee.TotalPayment;

                    // Lưu thông tin giờ khám
                    await unitOfWork.Repository<ExaminationForms>().CreateAsync(item);
                    await unitOfWork.SaveAsync();

                    // Tạo lịch sử tạo phiếu khám bệnh
                    await CreateExaminationHistory(item, item.CreatedBy);

                    // CẬP NHẬT THÔNG TIN DỊCH VỤ PHÁT SINH (NẾU LÀ KHÁM THƯỜNG)
                    if (item.TypeId == 0)
                    {
                        // Thêm dịch vụ phát sinh trong phiếu khám
                        if (item.AdditionServiceIds != null && item.AdditionServiceIds.Any())
                        {
                            var additionServiceTypeInfos = await this.unitOfWork.Repository<AdditionServices>().GetQueryable()
                                .Where(e => !e.Deleted && item.AdditionServiceIds.Contains(e.Id)).ToListAsync();

                            foreach (var additionServiceId in item.AdditionServiceIds)
                            {
                                var additionServiceTypeInfo = additionServiceTypeInfos.Where(e => e.Id == additionServiceId).FirstOrDefault();
                                if (additionServiceTypeInfo != null)
                                {
                                    ExaminationFormAdditionServiceMappings examinationFormServiceMapping = new ExaminationFormAdditionServiceMappings()
                                    {
                                        Created = DateTime.Now,
                                        CreatedBy = item.CreatedBy,
                                        Active = true,
                                        Deleted = false,
                                        Amount = additionServiceTypeInfo.Price,
                                        HospitalId = item.HospitalId,
                                        ExaminationFormId = item.Id,
                                        AdditionServiceId = additionServiceTypeInfo.Id
                                    };
                                    this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().Create(examinationFormServiceMapping);
                                }

                            }
                        }
                    }

                    // CẬP NHẬT THÔNG TIN CHI TIẾT DỊCH VỤ PHÁT SINH
                    if (item.AdditionServiceDetailIds != null && item.AdditionServiceDetailIds.Any())
                    {
                        var additionServiceDetailInfos = await this.unitOfWork.Repository<AdditionServiceDetails>().GetQueryable()
                            .Where(e => !e.Deleted && item.AdditionServiceDetailIds.Contains(e.Id)).ToListAsync();

                        foreach (var additionServiceDetailId in item.AdditionServiceDetailIds)
                        {
                            var additionServiceDetailInfo = additionServiceDetailInfos.Where(e => e.Id == additionServiceDetailId).FirstOrDefault();
                            ExaminationFormAdditionServiceDetailMappings examinationFormAdditionServiceDetailMapping = new ExaminationFormAdditionServiceDetailMappings()
                            {
                                Deleted = false,
                                Active = true,
                                ExaminationFormId = item.Id,
                                AdditionServiceDetailId = additionServiceDetailId,
                                HospitalId = item.HospitalId,
                                Amount = additionServiceDetailInfo.Price,
                            };
                            this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>().Create(examinationFormAdditionServiceDetailMapping);
                        }
                    }
                    await this.unitOfWork.SaveAsync();

                    //--------------------------- GỬI SMS STT CHO USER
                    //--------------------------- idnex string
                    //......................................................
                    if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed || item.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination)
                    {
                        var medicalRecordInfo = await unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == item.RecordId).FirstOrDefaultAsync();
                        if (medicalRecordInfo != null)
                        {
                            var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync();
                            if (userInfo != null)
                            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                if (!string.IsNullOrEmpty(item.ExaminationIndex))
                                    sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationIndex));
                                else if (!string.IsNullOrEmpty(item.ExaminationPaymentIndex))
                                    sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationPaymentIndex));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            }
                        }

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
        /// Cập nhật thông tin phiếu khám bệnh (lịch khám)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(ExaminationForms item)
        {
            if (item == null) throw new AppException("Vui lòng chọn thông tin item");

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existItem = await Queryable.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                    if (existItem != null)
                    {
                        item.Updated = DateTime.Now;
                        if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
                            item = await this.GetExaminationIndex(item);

                        // LƯU THÔNG TIN TỔNG TIỀN THANH TOÁN CỦA LỊCH KHÁM
                        FeeCaculateExaminationRequest feeCaculateExaminationRequest = new FeeCaculateExaminationRequest()
                        {
                            HospitalId = item.HospitalId ?? 0,
                            PaymentMethodId = item.PaymentMethodId ?? 0,
                            //ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                            VaccineTypeId = item.VaccineTypeId,
                            ExaminationFormId = item.Id,
                            SpecialistTypeId = item.SpecialistTypeId,
                            AdditionServiceIds = item.TypeId == 0 ? item.AdditionServiceIds : null,
                            AdditionServiceDetailIds = item.AdditionServiceDetailIds
                        };
                        var hospitalConfigFee = await this.hospitalConfigFeeService.GetFeeExamination(feeCaculateExaminationRequest);
                        item.Price = hospitalConfigFee.TotalPayment;

                        existItem = mapper.Map<ExaminationForms>(item);
                        unitOfWork.Repository<ExaminationForms>().Update(existItem);

                        // CẬP NHẬT THÔNG TIN DỊCH VỤ PHÁT SINH (NẾU LÀ KHÁM THƯỜNG)
                        if (item.TypeId == 0)
                        {
                            // Thêm dịch vụ phát sinh trong phiếu khám
                            if (item.AdditionServiceIds != null && item.AdditionServiceIds.Any())
                            {
                                var additionServiceTypeInfos = await this.unitOfWork.Repository<AdditionServices>().GetQueryable()
                                    .Where(e => !e.Deleted && item.AdditionServiceIds.Contains(e.Id)).ToListAsync();
                                var existAdditionServiceTypeMappings = await this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>()
                                    .GetQueryable()
                                    .Where(e => !e.Deleted && e.ExaminationFormId == item.Id).ToListAsync();
                                foreach (var additionServiceId in item.AdditionServiceIds)
                                {
                                    var additionServiceTypeInfo = additionServiceTypeInfos.Where(e => e.Id == additionServiceId).FirstOrDefault();
                                    if (additionServiceTypeInfo != null)
                                    {
                                        var existServiceTypeInfo = await this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().GetQueryable()
                                            .Where(e => !e.Deleted && e.ExaminationFormId == item.Id && e.AdditionServiceId == additionServiceId).FirstOrDefaultAsync();
                                        if (existServiceTypeInfo != null)
                                        {
                                            existServiceTypeInfo.Updated = DateTime.Now;
                                            existServiceTypeInfo.UpdatedBy = item.UpdatedBy;
                                            existServiceTypeInfo.ExaminationFormId = item.Id;
                                            existServiceTypeInfo.AdditionServiceId = additionServiceId;
                                            existServiceTypeInfo.Active = true;
                                            existServiceTypeInfo.Deleted = false;
                                            this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().Update(existServiceTypeInfo);
                                        }
                                        else
                                        {
                                            ExaminationFormAdditionServiceMappings examinationFormServiceMapping = new ExaminationFormAdditionServiceMappings()
                                            {
                                                Created = DateTime.Now,
                                                CreatedBy = item.CreatedBy,
                                                Active = true,
                                                Deleted = false,
                                                Amount = additionServiceTypeInfo.Price,
                                                HospitalId = item.HospitalId,
                                                ExaminationFormId = item.Id,
                                                AdditionServiceId = additionServiceTypeInfo.Id
                                            };
                                            this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().Create(examinationFormServiceMapping);
                                        }
                                    }

                                }
                                // ITEM TỒN TẠI TRONG DB, KHÔNG TỒN TẠI TRONG CHUỖI DỊCH VỤ ĐƯỢC CHỌN => XÓA
                                var existAdditionServiceOlds = await this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().GetQueryable()
                                    .Where(e => !item.AdditionServiceIds.Contains(e.AdditionServiceId) && e.ExaminationFormId == existItem.Id).ToListAsync();
                                if (existAdditionServiceOlds != null)
                                {
                                    foreach (var existAdditionServiceOld in existAdditionServiceOlds)
                                    {
                                        this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().Delete(existAdditionServiceOld);
                                    }
                                }
                            }
                            // XÓA HẾT TẤT CẢ NHỮNG DỊCH VỤ PHÁT SINH HIỆN TẠI CỦA PHIẾU
                            else
                            {
                                var currentAdditionServices = await this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().GetQueryable()
                                    .Where(e => !e.Deleted && e.ExaminationFormId == item.Id).ToListAsync();
                                if (currentAdditionServices != null && currentAdditionServices.Any())
                                {
                                    foreach (var currentAdditionService in currentAdditionServices)
                                    {
                                        this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().Delete(currentAdditionService);
                                    }
                                }
                            }
                        }
                        // KIỂM TRA XÓA TẤT CẢ DỊCH VỤ PHÁT SINH CỦA PHIẾU
                        else
                        {
                            var existAdditionServices = await this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>()
                                .GetQueryable().Where(e => !e.Deleted && e.ExaminationFormId == item.Id).ToListAsync();
                            if (existAdditionServices != null && existAdditionServices.Any())
                            {
                                foreach (var existAdditionService in existAdditionServices)
                                {
                                    this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().Delete(existAdditionService);
                                }
                            }

                        }

                        // CẬP NHẬT THÔNG TIN CHI TIẾT DỊCH VỤ PHÁT SINH
                        if (item.AdditionServiceDetailIds != null && item.AdditionServiceDetailIds.Any())
                        {
                            // CẬP NHẬT THÔNG TIN CHI TIẾT DỊCH VỤ
                            var additionServiceDetailInfos = await this.unitOfWork.Repository<AdditionServiceDetails>().GetQueryable()
                                .Where(e => !e.Deleted && item.AdditionServiceDetailIds.Contains(e.Id)).ToListAsync();
                            var existAdditionServiceDetailMappings = await this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>()
                                .GetQueryable().Where(e => e.ExaminationFormId == item.Id && item.AdditionServiceDetailIds.Contains(e.AdditionServiceDetailId)).ToListAsync();
                            foreach (var additionServiceDetailId in item.AdditionServiceDetailIds)
                            {
                                var existAdditionServiceDetailMapping = existAdditionServiceDetailMappings.Where(e => e.AdditionServiceDetailId == additionServiceDetailId).FirstOrDefault();
                                // THÊM MỚI CHI TIẾT DỊCH VỤ
                                if (existAdditionServiceDetailMapping == null)
                                {
                                    var additionServiceDetailInfo = additionServiceDetailInfos.Where(e => e.Id == additionServiceDetailId).FirstOrDefault();
                                    if (additionServiceDetailInfo != null)
                                    {
                                        ExaminationFormAdditionServiceDetailMappings examinationFormAdditionServiceDetailMapping = new ExaminationFormAdditionServiceDetailMappings()
                                        {
                                            Created = DateTime.Now,
                                            CreatedBy = item.CreatedBy,
                                            Active = true,
                                            Deleted = false,
                                            Amount = additionServiceDetailInfo.Price,
                                            HospitalId = item.HospitalId,
                                            ExaminationFormId = item.Id,
                                            AdditionServiceDetailId = additionServiceDetailInfo.Id
                                        };
                                        this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>().Create(examinationFormAdditionServiceDetailMapping);
                                    }
                                }
                            }
                            // XÓA TẤT CẢ CHI TIẾT DỊCH VỤ TỒN TẠI DƯỚI DB NHƯNG KHÔNG TỒN TẠI TRONG DANH SÁCH
                            var existAdditionServiceDetailOlds = await this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>().GetQueryable()
                                    .Where(e => !item.AdditionServiceDetailIds.Contains(e.AdditionServiceDetailId) && e.ExaminationFormId == existItem.Id).ToListAsync();
                            if (existAdditionServiceDetailOlds != null)
                            {
                                foreach (var existAdditionServiceDetailOld in existAdditionServiceDetailOlds)
                                {
                                    this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>().Delete(existAdditionServiceDetailOld);
                                }
                            }
                        }
                        else
                        {
                            // LẤY RA TẤT CẢ THÔNG TIN CHI TIẾT DỊCH VỤ PHÁT SINH
                            var currentExistAdditionServiceDetailMappings = await this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>().GetQueryable().Where(e => !e.Deleted && e.ExaminationFormId == item.Id).ToListAsync();
                            if (currentExistAdditionServiceDetailMappings != null && currentExistAdditionServiceDetailMappings.Any())
                            {
                                foreach (var currentExistAdditionServiceDetailMapping in currentExistAdditionServiceDetailMappings)
                                {
                                    this.unitOfWork.Repository<ExaminationFormAdditionServiceDetailMappings>().Delete(currentExistAdditionServiceDetailMapping);
                                }
                            }

                        }


                        await unitOfWork.SaveAsync();

                        // Tạo lịch sử tạo phiếu khám bệnh
                        await CreateExaminationHistory(item, item.UpdatedBy);
                        //--------------------------- GỬI SMS STT CHO USER
                        //--------------------------- idnex string
                        //......................................................
                        if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed || item.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination)
                        {
                            var medicalRecordInfo = await unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == item.RecordId).FirstOrDefaultAsync();
                            if (medicalRecordInfo != null)
                            {
                                var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync();
                                if (userInfo != null)
                                {
                                    if (!string.IsNullOrEmpty(item.ExaminationIndex))
                                        sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationIndex));
                                    else if (!string.IsNullOrEmpty(item.ExaminationPaymentIndex))
                                        sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationPaymentIndex));
                                }
                            }
                        }
                    }

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
            }
        }

        /// <summary>
        /// Lấy STT khám bệnh/ STT chờ lấy phiếu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<ExaminationForms> GetExaminationIndex(ExaminationForms item)
        {
            if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value > 0 && (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                || item.Status == (int)CatalogueUtilities.ExaminationStatus.WaitConfirm
                ))
            {
                var paymentMethodInfo = await unitOfWork.Repository<PaymentMethods>().GetQueryable().Where(e => !e.Deleted && e.Active
                && e.Id == item.PaymentMethodId).FirstOrDefaultAsync();
                if (paymentMethodInfo != null)
                {
                    string indexString = string.Empty;
                    //TH1: Thanh toán COD => Lấy STT chờ đăng kí khám
                    //=> Cập nhật lại STT tại phòng khám cho mẫu phiếu khám bệnh
                    if ((paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString()
                        || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.TRANSFER.ToString()
                        )
                        && item.Status == (int)CatalogueUtilities.ExaminationStatus.WaitConfirm
                        )
                    {
                        // Lấy STT khám tại BV
                        SearchExaminationIndex searchExaminationIndex = new SearchExaminationIndex()
                        {
                            HospitalId = item.HospitalId ?? 0,
                            ExaminationScheduleDetailId = item.ExaminationScheduleDetailId ?? 0,
                            ExaminationDate = item.ExaminationDate
                        };
                        if (string.IsNullOrEmpty(item.ExaminationIndex))
                        {
                            item.ExaminationIndex = await this.GetExaminationFormIndex(searchExaminationIndex);
                            item.ExaminationPaymentIndex = string.Empty;
                            indexString = item.ExaminationIndex;
                        }

                    }
                    //TH2: Thanh toán qua App => Lấy STT khám
                    //=> Cập nhật lại STT đóng tiền cho mẫu phiếu khám bệnh
                    else if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                        && paymentMethodInfo.Code != CatalogueUtilities.PaymentMethod.COD.ToString()
                        && paymentMethodInfo.Code != CatalogueUtilities.PaymentMethod.TRANSFER.ToString()
                        )
                    {
                        // Lấy STT đóng tiền tại BV
                        SearchExaminationIndex searchExaminationIndex = new SearchExaminationIndex()
                        {
                            HospitalId = item.HospitalId ?? 0,
                            ExaminationScheduleDetailId = item.ExaminationScheduleDetailId ?? 0,
                            ExaminationDate = item.ExaminationDate
                        };
                        if (string.IsNullOrEmpty(item.ExaminationPaymentIndex))
                        {
                            item.ExaminationPaymentIndex = await this.GetExaminationFormPaymentIndex(searchExaminationIndex);
                            item.ExaminationIndex = string.Empty;
                            indexString = item.ExaminationPaymentIndex;
                        }
                    }

                    // ----------------------------------------------- Lấy STT hệ thống theo khung giờ
                    if (!item.SystemIndex.HasValue || item.SystemIndex.Value <= 0)
                    {
                        // STT hệ thống
                        int systemIndex = 1;
                        // Lấy ra tất cả phiếu đăng kí theo khung giờ của phiếu hiện tại
                        var examinationForms = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                            .Where(e => !e.Deleted && e.Active && e.ExaminationScheduleDetailId == item.ExaminationScheduleDetailId
                            && e.Id != item.Id
                            && e.SystemIndex.HasValue
                            && e.SystemIndex.Value > 0
                            && e.Status != (int)CatalogueUtilities.ExaminationStatus.Canceled
                            && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
                            && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed
                            )
                            .ToListAsync();
                        // Nếu chưa có phiếu khám với khung giờ này => STT hệ thống bằng 1
                        if (examinationForms == null || !examinationForms.Any()) item.SystemIndex = systemIndex;
                        // Nếu có phiếu khám khung giờ này rồi => Lấy STT phiếu tiếp theo hoặc bị miss trong dãy stt của hệ thống
                        else
                        {
                            // Mảng STT trong hệ thống
                            int[] systemIndexArr = examinationForms.Select(e => e.SystemIndex.Value).OrderBy(e => e).ToArray();
                            // Tìm ra số bị thiếu trong mảng
                            int missingNumber = AppUtilities.FindMissingNumber(systemIndexArr);
                            // TH1: Lấy STT kế cuối
                            if (missingNumber == 0)
                            {
                                int lastArrayValue = 0;
                                // Lấy STT cuối cùng trong mảng + thêm 1
                                if (systemIndexArr != null && systemIndexArr.Any())
                                    lastArrayValue = systemIndexArr.OrderByDescending(e => e).FirstOrDefault();
                                item.SystemIndex = lastArrayValue + 1;
                            }
                            // TH2: Tìm STT bị khuyết trong mảng
                            else item.SystemIndex = missingNumber;
                        }
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Tạo lịch sử phiếu khám bệnh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task CreateExaminationHistory(ExaminationForms item, string createdBy)
        {
            int action = 0;
            switch (item.Status)
            {
                case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                    action = (int)CatalogueUtilities.ExaminationAction.Cancel;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                    {
                        action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                        // Tạo lịch sử thanh toán
                        await CreatePaymentHistories(item, createdBy);
                    }
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                    {
                        action = (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination;
                        // Tạo lịch sử thanh toán
                        await CreatePaymentHistories(item, createdBy);
                    }
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.WaitRefund:
                    action = (int)CatalogueUtilities.ExaminationAction.Cancel;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.RefundSuccess:
                    {
                        action = (int)CatalogueUtilities.ExaminationAction.Refund;
                        // Tạo lịch sử thanh toán
                        await CreatePaymentHistories(item, createdBy, 1);
                    }
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.PaymentFailed:
                    {
                        action = (int)CatalogueUtilities.ExaminationAction.Return;
                        // Tạo lịch sử thanh toán
                        await CreatePaymentHistories(item, createdBy, 2);
                    }
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.New:
                    action = (int)CatalogueUtilities.ExaminationAction.Create;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                    action = (int)CatalogueUtilities.ExaminationAction.Create;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                    action = (int)CatalogueUtilities.ExaminationAction.Update;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                    action = (int)CatalogueUtilities.ExaminationAction.FinishExamination;
                    break;
                default:
                    break;
            }

            // Tạo lịch sử tạo phiếu khám bệnh
            ExaminationHistories examinationHistories = new ExaminationHistories()
            {
                Created = DateTime.Now,
                CreatedBy = createdBy,
                Active = true,
                Deleted = false,
                ExaminationFormId = item.Id,
                Status = item.Status,
                Action = action,
                Comment = item.Comment,
                Note = item.Note,
                ExaminationDate = item.ExaminationDate,
                ReExaminationDate = item.ReExaminationDate,
                ExaminationIndex = item.ExaminationIndex,
                ExaminationPaymentIndex = item.ExaminationPaymentIndex,
                RoomExaminationId = item.RoomExaminationId,
                ExaminationScheduleDetailId = item.ExaminationScheduleDetailId,
                DoctorId = item.DoctorId,
                Id = 0
            };
            await unitOfWork.Repository<ExaminationHistories>().CreateAsync(examinationHistories);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Lưu thông tin lịch sử thanh toán + cập nhật số thứ tự cho người dùng
        /// </summary>
        /// <param name="examinationForms"></param>
        /// <returns></returns>
        private async Task CreatePaymentHistories(ExaminationForms examinationForms, string createdBy, int status = 0)
        {
            if (examinationForms.PaymentMethodId.HasValue && examinationForms.PaymentMethodId.Value > 0)
            {
                var paymentMethodInfo = await unitOfWork.Repository<PaymentMethods>().GetQueryable().Where(e => !e.Deleted && e.Active
                && e.Id == examinationForms.PaymentMethodId).FirstOrDefaultAsync();
                if (paymentMethodInfo != null)
                {
                    // Lưu lịch sử thanh toán
                    PaymentHistories paymentHistories = new PaymentHistories();
                    //TH1: Thanh toán COD => Lấy STT tại phòng khám
                    //=> Cập nhật lại STT tại phòng khám cho mẫu phiếu khám bệnh
                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString()
                        || paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.TRANSFER.ToString()
                        )
                    {
                        // Lưu lịch sử thanh toán
                        paymentHistories = new PaymentHistories()
                        {
                            Created = DateTime.Now,
                            Active = true,
                            Deleted = false,
                            CreatedBy = createdBy,
                            ExaminationFee = examinationForms.Price,
                            ServiceFee = examinationForms.FeeExamination,
                            ExaminationFormId = examinationForms.Id,
                            PaymentMethodId = examinationForms.PaymentMethodId.Value,
                            PaymentMethodName = paymentMethodInfo.Name,
                            HospitalId = examinationForms.HospitalId,
                            Status = status,
                            Id = 0
                        };
                        unitOfWork.Repository<PaymentHistories>().Create(paymentHistories);
                        await unitOfWork.SaveAsync();
                    }
                    //TH2: Thanh toán qua App => Lấy STT đóng tiền khám
                    //=> Cập nhật lại STT đóng tiền cho mẫu phiếu khám bệnh
                    else
                    {

                        BankInfos bankInfos = null;
                        if (examinationForms.BankInfoId.HasValue && examinationForms.BankInfoId.Value > 0)
                        {
                            bankInfos = await unitOfWork.Repository<BankInfos>().GetQueryable().Where(e => !e.Deleted && e.Active && e.Id == examinationForms.BankInfoId.Value).FirstOrDefaultAsync();
                        }

                        // Lưu lịch sử thanh toán
                        paymentHistories = new PaymentHistories()
                        {
                            Id = 0,
                            HospitalId = examinationForms.HospitalId,
                            Created = DateTime.Now,
                            Active = true,
                            Deleted = false,
                            CreatedBy = createdBy,
                            ExaminationFee = examinationForms.Price,
                            ServiceFee = examinationForms.FeeExamination,
                            ExaminationFormId = examinationForms.Id,
                            PaymentMethodId = examinationForms.PaymentMethodId.Value,
                            PaymentMethodName = paymentMethodInfo.Name,
                            BankInfoId = examinationForms.BankInfoId,
                            Status = status,
                            BankInfo = bankInfos != null ? string.Format("STK: {0} - {1}", bankInfos.BankNo, bankInfos.BankBranch) : string.Empty
                        };
                        unitOfWork.Repository<PaymentHistories>().Create(paymentHistories);
                        await unitOfWork.SaveAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Kiểm tra trùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(ExaminationForms item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;

            //bool isExistDetailSchedule = await Queryable.AnyAsync(x => !x.Deleted && x.Active
            //&& x.Id != item.Id
            //&& (((!item.ExaminationScheduleDetailId.HasValue || item.ExaminationScheduleDetailId.Value <= 0)
            //    //&& x.ServiceTypeId == item.ServiceTypeId
            //    && (x.ExaminationDate.Date == item.ExaminationDate.Date))
            //|| (item.ExaminationScheduleDetailId.HasValue && item.ExaminationScheduleDetailId.Value > 0 && x.ExaminationScheduleDetailId == item.ExaminationScheduleDetailId
            //&& x.RecordId == item.RecordId))
            //&& x.Status != (int)CatalogueUtilities.ExaminationStatus.Canceled
            //&& x.Status != (int)CatalogueUtilities.ExaminationStatus.New
            //&& x.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
            //&& x.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed
            //);
            //if (isExistDetailSchedule)
            //    messages.Add("Đã tồn tại phiếu khám với thời gian này!");

            if (item.ExaminationScheduleDetailId.HasValue && item.ExaminationScheduleDetailId.Value > 0)
            {
                var examinationScheduleDetailInfo = await this.unitOfWork.Repository<ExaminationScheduleDetails>().GetQueryable().Where(e => e.Id == item.ExaminationScheduleDetailId).FirstOrDefaultAsync();
                // Đếm số lượng phiếu của user đã khám theo khung giờ đã chọn
                int totalRegisterExamination = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active
                    && e.ExaminationScheduleDetailId == item.ExaminationScheduleDetailId
                    && e.Status != (int)CatalogueUtilities.ExaminationStatus.New
                    && e.Status != (int)CatalogueUtilities.ExaminationStatus.Canceled
                    && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
                    && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed
                    && e.Id != item.Id
                    ).CountAsync();

                if (examinationScheduleDetailInfo != null)
                {
                    // TH1: Nếu có cấu hình số ca khám tối đa theo khung giờ => check theo khung giờ
                    if (examinationScheduleDetailInfo.MaximumExamination.HasValue && examinationScheduleDetailInfo.MaximumExamination.Value > 0)
                    {
                        if (totalRegisterExamination >= examinationScheduleDetailInfo.MaximumExamination.Value)
                            messages.Add("Số ca khám trong khung giờ đã tối đa! Vui lòng chọn khung giờ khác!");

                    }
                    // TH2: Check theo schedule
                    else
                    {
                        var scheduleInfo = await this.unitOfWork.Repository<ExaminationSchedules>().GetQueryable().Where(e => e.Id == examinationScheduleDetailInfo.ScheduleId).FirstOrDefaultAsync();
                        if (scheduleInfo != null)
                        {
                            var sessionInfo = await this.unitOfWork.Repository<SessionTypes>().GetQueryable().Where(e => e.Id == examinationScheduleDetailInfo.SessionTypeId).FirstOrDefaultAsync();
                            if (sessionInfo != null)
                            {
                                // Check theo thông tin buổi sáng
                                if (sessionInfo.Code == CatalogueUtilities.SessionType.BS.ToString())
                                {
                                    if (scheduleInfo.MaximumMorningExamination.HasValue && scheduleInfo.MaximumMorningExamination.Value > 0 && totalRegisterExamination >= scheduleInfo.MaximumMorningExamination.Value)
                                        messages.Add("Số ca khám trong buổi sáng đã tối đa! Vui lòng chọn khung giờ khác!");
                                }
                                else if (sessionInfo.Code == CatalogueUtilities.SessionType.BC.ToString())
                                {
                                    if (scheduleInfo.MaximumAfternoonExamination.HasValue && scheduleInfo.MaximumAfternoonExamination.Value > 0 && totalRegisterExamination >= scheduleInfo.MaximumAfternoonExamination.Value)
                                        messages.Add("Số ca khám trong buổi chiều đã tối đa! Vui lòng chọn khung giờ khác!");
                                }
                                else
                                {
                                    if (scheduleInfo.MaximumOtherExamination.HasValue && scheduleInfo.MaximumOtherExamination.Value > 0 && totalRegisterExamination >= scheduleInfo.MaximumOtherExamination.Value)
                                        messages.Add("Số ca khám trong buổi đã tối đa! Vui lòng chọn khung giờ khác!");
                                }


                            }
                        }
                    }
                }
            }


            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

        /// <summary>
        /// Lấy STT mới nhất của dịch vụ bệnh viện tương ứng
        /// </summary>
        /// <param name="searchExaminationIndex"></param>
        /// <returns></returns>
        public async Task<string> GetExaminationFormIndex(SearchExaminationIndex searchExaminationIndex)
        {
            string indexString = string.Empty;
            int index = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchExaminationIndex.HospitalId),
                new SqlParameter("@ExaminationScheduleDetailId", searchExaminationIndex.ExaminationScheduleDetailId),
                new SqlParameter("@ExaminationDate", searchExaminationIndex.ExaminationDate),
                new SqlParameter("@ExaminationIndex", SqlDbType.Int, 0),

            };
            var obj = await this.unitOfWork.Repository<ExaminationForms>().ExcuteStoreGetValue("Index_GetIExaminationIndex", parameters, "@ExaminationIndex");
            indexString = obj.ToString();
            if (!string.IsNullOrEmpty(indexString))
            {
                int.TryParse(indexString, out index);
                index += 1;
            }
            indexString = index.ToString("D3");
            return indexString;
        }

        /// <summary>
        /// Lấy STT chờ khám
        /// </summary>
        /// <param name="searchExaminationIndex"></param>
        /// <returns></returns>
        public async Task<string> GetExaminationFormPaymentIndex(SearchExaminationIndex searchExaminationIndex)
        {
            string indexString = string.Empty;
            int index = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchExaminationIndex.HospitalId),
                new SqlParameter("@ExaminationScheduleDetailId", searchExaminationIndex.ExaminationScheduleDetailId),
                new SqlParameter("@ExaminationDate", searchExaminationIndex.ExaminationDate),
                new SqlParameter("@ExaminationIndex", SqlDbType.Int, 0),
            };
            var obj = await this.unitOfWork.Repository<ExaminationForms>().ExcuteStoreGetValue("Index_GetIExaminationPaymentIndex", parameters, "@ExaminationIndex");
            indexString = obj.ToString();
            if (!string.IsNullOrEmpty(indexString))
            {
                if (int.TryParse(indexString, out index))
                    index += 1;
            }
            indexString = index.ToString("D3");
            return indexString;
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        private async Task UpdateUserInfo(int recordId, bool isAdmin = false)
        {
            if (isAdmin) return;
            var medicalRecordInfo = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active
                    && e.Id == recordId
                    ).FirstOrDefaultAsync()
                    ;
            // Lấy thông tin account user từ hồ sơ bệnh án
            if (medicalRecordInfo != null)
            {
                var userInfo = await this.unitOfWork.Repository<Users>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active && e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync()
                    ;
                if (userInfo != null)
                {
                    if (!userInfo.TotalViolations.HasValue || userInfo.TotalViolations.Value == 0)
                        userInfo.TotalViolations = 1;
                    else userInfo.TotalViolations += 1;

                    // Cập nhật số lần hủy phiếu của user
                    if (!userInfo.TotalCancelExaminations.HasValue || userInfo.TotalCancelExaminations.Value == 0)
                        userInfo.TotalCancelExaminations = 1;
                    else userInfo.TotalCancelExaminations += 1;

                    if (userInfo.TotalViolations == 3)
                    {
                        userInfo.IsLocked = true;
                        userInfo.TotalViolations = 0;
                        userInfo.LockedDate = DateTime.Now.AddDays(30);
                    }
                    //else
                    //{
                    //    userInfo.IsLocked = false;
                    //    userInfo.LockedDate = null;
                    //}
                    Expression<Func<Users, object>>[] includeUserProperties = new Expression<Func<Users, object>>[]
                    {
                                        e => e.TotalViolations,
                                        e => e.IsLocked,
                                        e => e.LockedDate,
                                        e => e.TotalCancelExaminations
                    };
                    await this.unitOfWork.Repository<Users>().UpdateFieldsSaveAsync(userInfo, includeUserProperties);
                    await unitOfWork.SaveAsync();
                    await this.medicalDbContext.SaveChangesAsync();
                    medicalDbContext.Entry(userInfo).State = EntityState.Detached;
                }
            }
        }

        /// <summary>
        /// Cập nhật số lần chỉnh sửa của user
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        private async Task UpdateUserEditExamination(int recordId)
        {
            var medicalRecordInfo = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active
                    && e.Id == recordId
                    ).FirstOrDefaultAsync()
                    ;
            // Lấy thông tin account user từ hồ sơ bệnh án
            if (medicalRecordInfo != null)
            {
                var userInfo = await this.unitOfWork.Repository<Users>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active && e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync()
                    ;
                if (userInfo != null)
                {
                    if (!userInfo.TotalEditExaminations.HasValue || userInfo.TotalEditExaminations.Value == 0)
                        userInfo.TotalEditExaminations = 1;
                    else userInfo.TotalEditExaminations += 1;
                }

                Expression<Func<Users, object>>[] includeUserProperties = new Expression<Func<Users, object>>[]
                {
                                        e => e.TotalEditExaminations,
                };
                await this.unitOfWork.Repository<Users>().UpdateFieldsSaveAsync(userInfo, includeUserProperties);
                await unitOfWork.SaveAsync();
                await this.medicalDbContext.SaveChangesAsync();
                medicalDbContext.Entry(userInfo).State = EntityState.Detached;
            }
        }

        /// <summary>
        /// Lấy ra thông báo lỗi kiểm tra trạng thái cập nhật của phiếu khám
        /// </summary>
        /// <param name="examinationFormId"></param>
        /// <param name="statusCheck"></param>
        /// <returns></returns>
        public async Task<string> GetCheckStatusMessage(int examinationFormId, int statusCheck)
        {
            string result = string.Empty;
            bool isError = false;
            var examinationFormInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                .Where(e => e.Id == examinationFormId)
                .FirstOrDefaultAsync();
            if (examinationFormInfo != null)
            {
                switch (statusCheck)
                {
                    case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                        {
                            if (examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.New
                                && examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                        {
                            if (examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.WaitConfirm
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                    case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                        {
                            if (examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.Confirmed
                                && examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                        {
                            if (examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.WaitReExamination
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentFailed:
                        {
                            if (examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.WaitConfirm
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed:
                        {
                            if (examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.WaitReExamination
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.ExaminationStatus.New:
                        {
                            if (examinationFormInfo.Status != (int)CatalogueUtilities.ExaminationStatus.New
                                )
                                isError = true;
                        }
                        break;
                    default:
                        break;
                }
                if (isError) return "Trạng thái phiếu không hợp lệ! Không thể cập nhật";
                return string.Empty;
            }
            else return "Không tìm thấy thông tin phiếu khám";
        }

        #region CRON JOBS

        /// <summary>
        /// Job cập nhật trạng thái phiếu
        /// </summary>
        /// <returns></returns>
        public async Task UpdateCurrentExaminationJob()
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0);
            DateTime dateCheck = DateTime.Now.Date + ts;

            var examinationFormChecks = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                .Where(e => !e.Deleted && e.Active
                && ((!e.ReExaminationDate.HasValue && e.ExaminationDate < dateCheck) || (e.ReExaminationDate.Value < dateCheck))
                && (e.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
                || e.Status == (int)CatalogueUtilities.ExaminationStatus.WaitReExamination)
                ).ToListAsync();
            if (examinationFormChecks == null || !examinationFormChecks.Any())
                return;
            foreach (var examinationFormCheck in examinationFormChecks)
            {
                // Cập nhật trạng thái phiếu thành Đã hủy
                examinationFormCheck.Status = (int)CatalogueUtilities.ExaminationStatus.Canceled;
                examinationFormCheck.Updated = DateTime.Now;
                examinationFormCheck.UpdatedBy = "Job";
                Expression<Func<ExaminationForms, object>>[] includeExaminationProperties = new Expression<Func<ExaminationForms, object>>[]
                {
                    e => e.Status,
                    e => e.Updated,
                    e => e.UpdatedBy
                };
                this.unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(examinationFormCheck, includeExaminationProperties);
                // Cập nhật lại số lần vi phạm của user
                await UpdateUserInfo(examinationFormCheck.RecordId);
            }
        }

        /// <summary>
        /// Job chạy cập nhật hủy phiếu
        /// </summary>
        /// <param name="examinationFormId"></param>
        /// <returns></returns>
        public async Task UpdateCancelExaminationJob(int examinationFormId)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0);
            DateTime dateCheck = DateTime.Now.Date + ts;
            var examinationFormCheck = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                .Where(e => !e.Deleted && e.Active
                && e.Id == examinationFormId
                ).FirstOrDefaultAsync();
            if (examinationFormCheck != null
                && examinationFormCheck.Status == (int)CatalogueUtilities.ExaminationStatus.Canceled)
            {
                // Cập nhật lại số lần vi phạm của user
                await UpdateUserInfo(examinationFormCheck.RecordId);
            }
        }

        /// <summary>
        /// TẠO THÔNG BÁO CHO USER NHẮC NHỞ SẮP ĐÊN GIỜ KHÁM (10/15/30 phút)
        /// </summary>
        /// <param name="totalMinute">Tổng số phút gửi</param>
        /// <returns></returns>
        public async Task RemindUserExamination(int totalMinute)
        {
            DateTime currentDate = DateTime.Now;
            int currentTotalMinute = currentDate.Hour * 60 + currentDate.Minute;
            // LẤY RA NHỮNG PHIẾU SẮP ĐẾN GIỚ KHÁM (< 30 PHÚT)
            var examinationForms = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
               .Where(e => !e.Deleted && e.Active
               && e.ExaminationDate.Date == DateTime.Now.Date
               && e.FromTimeExamination - currentTotalMinute <= totalMinute + 1
               && e.FromTimeExamination - currentTotalMinute > totalMinute - 1
               && e.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed
               ).ToListAsync();

            // KIỂM TRA NHỮNG PHIẾU ĐÓ ĐÃ CÓ THÔNG BÁO CHO USER CHƯA 
            if (examinationForms == null || !examinationForms.Any()) return;

            // Lấy ra thông tin template thông báo nhắc lịch hẹn (10/15/30p);
            var notificationTemplateCheck = await this.unitOfWork.Repository<NotificationTemplates>().GetQueryable()
                .Where(e => !e.Deleted && e.Code == CoreContants.TEMPLATE_NEXT_EXAMINATION_NOTIFY).FirstOrDefaultAsync();

            // Danh sách mã phiếu có trong thông báo
            List<int> notificationExaminationFormIds = null;

            // Lấy ra danh sách mã phiếu để kiểm tra
            var examinationFormIds = examinationForms.Select(e => e.Id).ToList();

            // Lấy ra thông báo nhắc nhở của phiếu đã lưu với template
            var currentNotifications = await this.unitOfWork.Repository<Notifications>()
                .GetQueryable()
                .Where(e => !e.Deleted
                && e.NotificationTemplateId == notificationTemplateCheck.Id
                && (!string.IsNullOrEmpty(e.ExaminationFormIds))
                && e.Created.Date == DateTime.Now.Date).ToListAsync();
            if (currentNotifications != null && currentNotifications.Any())
                notificationExaminationFormIds = currentNotifications.SelectMany(e => e.ExaminationFormSplitIds).ToList();

            // List danh sách phiếu cần tạo thông báo
            List<int> selectedExaminationFormIds = new List<int>();
            // LẤY RA DANH SÁCH PHIẾU CHƯA CÓ NOTIFY
            if (notificationExaminationFormIds != null && notificationExaminationFormIds.Any())
                selectedExaminationFormIds = examinationFormIds.Except(notificationExaminationFormIds).ToList();
            else selectedExaminationFormIds = examinationFormIds;

            if (selectedExaminationFormIds == null || !selectedExaminationFormIds.Any()) return;

            // Lấy ra danh sách bệnh viện có thông tin phiếu
            var hospitalIds = examinationForms.Select(e => e.HospitalId.Value).Distinct().ToList();
            var hospitalInfos = await this.unitOfWork.Repository<Hospitals>().GetQueryable()
                .Where(e => hospitalIds.Contains(e.Id)).ToListAsync();

            // Lấy ra danh sách bác sĩ trong thông tin phiếu
            var doctorIds = examinationForms.Where(e => e.DoctorId.HasValue).Select(e => e.DoctorId.Value).ToList();
            List<Doctors> doctorInfos = new List<Doctors>();
            if (doctorIds != null && doctorIds.Any())
            {
                doctorInfos = await this.unitOfWork.Repository<Doctors>().GetQueryable()
                    .Where(e => doctorIds.Contains(e.Id))
                    .Select(e => new Doctors()
                    {
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        UserId = e.UserId,
                        Id = e.Id
                    }).ToListAsync();
            }

            // Lấy ra danh sách phòng khám
            var roomExaminationIds = examinationForms.Where(e => e.RoomExaminationId.HasValue).Select(e => e.RoomExaminationId.Value).ToList();
            List<RoomExaminations> roomExaminationInfos = new List<RoomExaminations>();
            if (roomExaminationIds != null && roomExaminationIds.Any())
            {
                roomExaminationInfos = await this.unitOfWork.Repository<RoomExaminations>().GetQueryable()
                    .Where(e => roomExaminationIds.Contains(e.Id))
                    .Select(e => new RoomExaminations()
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Code = e.Code,
                    }).ToListAsync();
            }

            // List danh sách user
            List<int> userIds = new List<int>();
            // Mã hồ sơ
            var recordIds = examinationForms.Select(e => e.RecordId).ToList();
            var medicalRecordInfos = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable()
                .Where(e => recordIds.Contains(e.Id)).ToListAsync();
            if (medicalRecordInfos != null && medicalRecordInfos.Any()) userIds = medicalRecordInfos.Select(e => e.UserId).ToList();

            if (notificationTemplateCheck == null) return;


            // Lấy thông tin loại user
            var notificationUserTypeInfo = await this.unitOfWork.Repository<NotificationTypes>().GetQueryable()
                .Where(e => e.Code == CatalogueUtilities.NotificationType.USER.ToString()).FirstOrDefaultAsync();

            // Lấy thông tin bảng thông báo cho user (NotificationApplicationUser)
            var notificationApplicationUserTable = this.SetDataTable(NOTIFICATION_APPLICATION_USER);


            // TẠO NOTIFY VỚI DANH SÁCH PHIẾU CHƯA CÓ NOTIFY
            using (var contextTransaction = await medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Notifications notifications = new Notifications()
                    {
                        Deleted = false,
                        Active = true,
                        Created = DateTime.Now,
                        CreatedBy = "Job",
                        NotificationTemplateId = notificationTemplateCheck.Id,
                        IsSendNotify = false,
                        IsRead = false,
                        NotificationTypeId = notificationUserTypeInfo.Id,
                        WebUrl = "examination/examination-form/{0}",
                        TypeId = (int)CatalogueUtilities.NotificationCatalogueType.NextExamination,
                        //UserIds = userIds,
                        ExaminationFormIds = string.Join(";", selectedExaminationFormIds),
                        AppUrl = string.Empty,
                        Content = notificationTemplateCheck.Content,
                        Title = notificationTemplateCheck.Title,
                    };
                    // Lưu thông tin thông báo
                    await this.unitOfWork.Repository<Notifications>().CreateAsync(notifications);
                    await this.unitOfWork.SaveAsync();

                    // Tạo nội dung thông báo cho user
                    foreach (var selectedExaminationFormId in selectedExaminationFormIds)
                    {
                        var examinationFormInfo = examinationForms.Where(e => e.Id == selectedExaminationFormId).FirstOrDefault();
                        if (examinationFormInfo == null) continue;
                        var userId = medicalRecordInfos.Where(e => e.Id == examinationFormInfo.RecordId).Select(e => e.UserId).FirstOrDefault();
                        var hospitalInfo = hospitalInfos.Where(e => e.Id == examinationFormInfo.HospitalId.Value).FirstOrDefault();
                        var doctorInfo = doctorInfos.Where(e => e.Id == examinationFormInfo.DoctorId.Value).FirstOrDefault();
                        var roomExaminationInfo = roomExaminationInfos.Where(e => e.Id == examinationFormInfo.RoomExaminationId).FirstOrDefault();

                        NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                        {
                            Deleted = false,
                            Active = true,
                            Created = DateTime.Now,
                            CreatedBy = "Job",
                            HospitalId = hospitalInfo.Id,
                            ToUserId = userId,
                            IsRead = false,
                            NotificationId = notifications.Id,
                            // Gấn giá trị notify cho từng user
                            NotificationContent = string.Format(notificationTemplateCheck.Content
                            , totalMinute.ToString()
                            , hospitalInfo != null ? hospitalInfo.Name : string.Empty
                            , doctorInfo != null ? (doctorInfo.FirstName + " " + doctorInfo.LastName) : string.Empty
                            , roomExaminationInfo != null ? roomExaminationInfo.Name : string.Empty
                            ),
                        };
                        notificationApplicationUserTable.Rows.Add(notificationApplicationUser.Id, notificationApplicationUser.NotificationId, notificationApplicationUser.ToUserId, notificationApplicationUser.IsRead, notificationApplicationUser.Created, notificationApplicationUser.CreatedBy, notificationApplicationUser.Updated, notificationApplicationUser.UpdatedBy, notificationApplicationUser.Deleted, notificationApplicationUser.Active, notificationApplicationUser.HospitalId, notificationApplicationUser.UserGroupId, notificationApplicationUser.NotificationContent);
                    }
                    // KIỂM TRA DỮ LIỆU THÔNG BÁO CỦA USER => BULK INSERT DỮ LIỆU THÔNG BÁO CHO USER
                    if (notificationApplicationUserTable != null && notificationApplicationUserTable.Rows.Count > 0)
                    {
                        var connectionString = string.Format(configuration.GetConnectionString("MedicalDbContext"));
                        using (SqlBulkCopy bc = new SqlBulkCopy(connectionString))
                        {
                            bc.DestinationTableName = NOTIFICATION_APPLICATION_USER;
                            bc.BulkCopyTimeout = 4000;
                            await bc.WriteToServerAsync(notificationApplicationUserTable);
                        }
                    }
                    contextTransaction.Commit();

                    // Thêm job nhắc nhở 15p
                    BackgroundJob.Schedule(() => SendNotifiChild(15, notifications.Id), TimeSpan.FromMinutes(15));

                    // ONE SIGNAL PUSH NOTIFY TO DESKTOP


                    // ONE SIGNAL PUSH NOTIFY TO MOBILE


                    // SIGNALR PUSH TO FE


                }
                catch (Exception)
                {
                    contextTransaction.Rollback();
                    return;
                }

            }
        }

        /// <summary>
        /// Tạo thông báo gửi 10/15p
        /// </summary>
        /// <param name="totalMinute"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task SendNotifiChild(int totalMinute, int notificationId)
        {
            DateTime currentDate = DateTime.Now;
            int currentTotalMinute = currentDate.Hour * 60 + currentDate.Minute;

            // Lấy ra thông tin thông báo cho user
            var notificationInfo = await this.unitOfWork.Repository<Notifications>()
                .GetQueryable().Where(e => e.Id == notificationId).FirstOrDefaultAsync();
            if (notificationInfo == null) return;

            // Lấy ra thông tin phiếu được chọn để gửi thông báo
            var selectedExaminationFormIds = notificationInfo.ExaminationFormSplitIds;
            if (selectedExaminationFormIds == null || !selectedExaminationFormIds.Any()) return;
            var examinationFormInfos = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                .Where(e => selectedExaminationFormIds.Contains(e.Id))
                .Select(e => new ExaminationForms()
                {
                    Id = e.Id,
                    HospitalId = e.HospitalId,
                    DoctorId = e.DoctorId,
                    RoomExaminationId = e.RoomExaminationId,
                    RecordId = e.RecordId
                })
                .ToListAsync();
            if (examinationFormInfos == null || !examinationFormInfos.Any()) return;

            // Lấy ra thông tin user được gửi thông báo
            List<int> userSelectedIds = new List<int>();
            var medicalRecordSelectedIds = examinationFormInfos.Select(e => e.RecordId).ToList();
            var medicalRecordInfos = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable()
                .Where(e => medicalRecordSelectedIds.Contains(e.Id)).ToListAsync();
            if (medicalRecordInfos == null || !medicalRecordInfos.Any()) return;
            userSelectedIds = medicalRecordInfos.Select(e => e.UserId).ToList();

            // Lấy ra thông tin template thông báo nhắc lịch hẹn (10/15/30p);
            var notificationTemplateCheck = await this.unitOfWork.Repository<NotificationTemplates>().GetQueryable()
                .Where(e => !e.Deleted && e.Code == CoreContants.TEMPLATE_NEXT_EXAMINATION_NOTIFY).FirstOrDefaultAsync();
            if (notificationTemplateCheck == null) return;


            // Lấy ra thông tin bệnh viện
            List<Hospitals> hospitalInfos = new List<Hospitals>();
            var hospitalSelectedIds = examinationFormInfos.Select(e => e.HospitalId.Value).ToList();
            if (hospitalSelectedIds != null && hospitalSelectedIds.Any())
            {
                hospitalInfos = await this.unitOfWork.Repository<Hospitals>().GetQueryable()
                    .Where(e => hospitalSelectedIds.Contains(e.Id)).ToListAsync();
            }

            // Lấy ra thông tin phòng khám được gửi thông báo
            List<RoomExaminations> roomExaminationInfos = new List<RoomExaminations>();
            var roomExaminationSelectedIds = examinationFormInfos.Select(e => e.RoomExaminationId.Value).ToList();
            if (roomExaminationSelectedIds != null && roomExaminationSelectedIds.Any())
            {
                roomExaminationInfos = await this.unitOfWork.Repository<RoomExaminations>().GetQueryable()
                    .Where(e => roomExaminationSelectedIds.Contains(e.Id)).ToListAsync();
            }

            // Lấy ra thông tin bác sĩ được chọn
            List<Doctors> doctorInfos = new List<Doctors>();
            var doctorSelectedIds = examinationFormInfos.Select(e => e.DoctorId.Value).ToList();
            if (doctorSelectedIds != null && doctorSelectedIds.Any())
            {
                doctorInfos = await this.unitOfWork.Repository<Doctors>().GetQueryable()
                    .Where(e => doctorSelectedIds.Contains(e.Id)).ToListAsync();
            }


            // Khởi tạo bảng thông báo cho user (NotificationApplicationUser)
            var notificationApplicationUserTable = SetDataTable(NOTIFICATION_APPLICATION_USER);

            foreach (var examinationFormInfo in examinationFormInfos)
            {
                var medicalRecordInfo = medicalRecordInfos.Where(e => e.Id == examinationFormInfo.RecordId).FirstOrDefault();
                var hospitalInfo = hospitalInfos.Where(e => e.Id == examinationFormInfo.HospitalId).FirstOrDefault();
                var doctorInfo = doctorInfos.Where(e => e.Id == examinationFormInfo.DoctorId).FirstOrDefault();
                var roomExaminationInfo = roomExaminationInfos.Where(e => e.Id == examinationFormInfo.RoomExaminationId).FirstOrDefault();
                if (medicalRecordInfo == null) continue;

                NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                {
                    Deleted = false,
                    Active = true,
                    Created = DateTime.Now,
                    CreatedBy = "Job",
                    HospitalId = examinationFormInfo.HospitalId,
                    ToUserId = medicalRecordInfo.UserId,
                    IsRead = false,
                    NotificationId = notificationId,
                    // Gán giá trị notify cho từng user
                    NotificationContent = string.Format(notificationTemplateCheck.Content
                            , totalMinute.ToString()
                            , hospitalInfo != null ? hospitalInfo.Name : string.Empty
                            , doctorInfo != null ? (doctorInfo.FirstName + " " + doctorInfo.LastName) : string.Empty
                            , roomExaminationInfo != null ? roomExaminationInfo.Name : string.Empty
                            ),
                };
                notificationApplicationUserTable.Rows.Add(notificationApplicationUser.Id, notificationApplicationUser.NotificationId, notificationApplicationUser.ToUserId, notificationApplicationUser.IsRead, notificationApplicationUser.Created, notificationApplicationUser.CreatedBy, notificationApplicationUser.Updated, notificationApplicationUser.UpdatedBy, notificationApplicationUser.Deleted, notificationApplicationUser.Active, notificationApplicationUser.HospitalId, notificationApplicationUser.UserGroupId, notificationApplicationUser.NotificationContent);
            }

            // KIỂM TRA DỮ LIỆU THÔNG BÁO CỦA USER => BULK INSERT DỮ LIỆU THÔNG BÁO CHO USER
            if (notificationApplicationUserTable != null && notificationApplicationUserTable.Rows.Count > 0)
            {
                var connectionString = string.Format(configuration.GetConnectionString("MedicalDbContext"));
                using (SqlBulkCopy bc = new SqlBulkCopy(connectionString))
                {
                    bc.DestinationTableName = NOTIFICATION_APPLICATION_USER;
                    bc.BulkCopyTimeout = 4000;
                    await bc.WriteToServerAsync(notificationApplicationUserTable);
                }
            }

            // KIỂM TRA NẾU SỐ PHÚT GỬI THÔNG BÁO KHÔNG PHẢI 15 => RETURN
            if (totalMinute != 15) return;
            // TẠO THÔNG BÁO CÒN 10P KHÁM BỆNH CHO USER
            BackgroundJob.Schedule(() => SendNotifiChild(10, notificationId), TimeSpan.FromMinutes(5));

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Lấy thông tin cột của bảng thông qua tên bảng
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

        #endregion

        #region Contants

        public const string NOTIFICATION_APPLICATION_USER = "NotificationApplicationUser";

        #endregion

    }
}
