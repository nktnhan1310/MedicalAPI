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

namespace Medical.Service
{
    public class ExaminationFormService : DomainService<ExaminationForms, SearchExaminationForm>, IExaminationFormService
    {
        private readonly ISMSConfigurationService sMSConfigurationService;
        private IHttpContextAccessor httpContextAccessor;
        private IConfiguration configuration;
        private IMedicalDbContext medicalDbContext;
        private IHospitalConfigFeeService hospitalConfigFeeService;
        public ExaminationFormService(IMedicalUnitOfWork unitOfWork, IMapper mapper, ISMSConfigurationService sMSConfigurationService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IMedicalDbContext medicalDbContext, IHospitalConfigFeeService hospitalConfigFeeService) : base(unitOfWork, mapper)
        {
            this.sMSConfigurationService = sMSConfigurationService;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
            this.medicalDbContext = medicalDbContext;
            this.hospitalConfigFeeService = hospitalConfigFeeService;

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
                new SqlParameter("@ServiceTypeId", baseSearch.ServiceTypeId),
                new SqlParameter("@StatusIds", statusIds),


                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
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
            bool result = false;
            int? action = null;
            var existExaminationFormInfo = await Queryable.Where(e => e.Id == updateExaminationStatus.ExaminationFormId).AsNoTracking().FirstOrDefaultAsync();
            this.medicalDbContext.Entry<ExaminationForms>(existExaminationFormInfo).State = EntityState.Detached;
            Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[] { };
            if (existExaminationFormInfo != null)
            {
                if (updateExaminationStatus.Status.HasValue)
                {
                    existExaminationFormInfo.Status = updateExaminationStatus.Status.Value;
                    existExaminationFormInfo.Updated = DateTime.Now;
                    existExaminationFormInfo.UpdatedBy = updateExaminationStatus.CreatedBy;
                    if (updateExaminationStatus.PaymentMethodId.HasValue)
                        existExaminationFormInfo.PaymentMethodId = updateExaminationStatus.PaymentMethodId;
                    if (updateExaminationStatus.BankInfoId.HasValue)
                        existExaminationFormInfo.BankInfoId = updateExaminationStatus.BankInfoId;
                    if (updateExaminationStatus.ReExaminationDate.HasValue)
                    {
                        if (updateExaminationStatus.ReExaminationDate.Value.Date < existExaminationFormInfo.ExaminationDate.Date)
                            throw new Exception("Ngày tái khám phải lớn hơn ngày khám hiện tại");
                        existExaminationFormInfo.ReExaminationDate = updateExaminationStatus.ReExaminationDate;
                    }
                    switch (updateExaminationStatus.Status)
                    {
                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                            {
                                if (!updateExaminationStatus.PaymentMethodId.HasValue)
                                    throw new Exception("Vui lòng chọn phương thức thanh toán");
                                FeeCaculateExaminationRequest feeCaculateExaminationRequest = new FeeCaculateExaminationRequest()
                                {
                                    HospitalId = existExaminationFormInfo.HospitalId ?? 0,
                                    PaymentMethodId = existExaminationFormInfo.PaymentMethodId.Value,
                                    ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                                    SpecialistTypeId = existExaminationFormInfo.SpecialistTypeId
                                };
                                var hospitalConfigFee = await this.hospitalConfigFeeService.GetFeeExamination(feeCaculateExaminationRequest);
                                existExaminationFormInfo.Price = hospitalConfigFee.TotalPayment;
                                action = (int)CatalogueUtilities.ExaminationAction.Update;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.PaymentMethodId,
                                    x => x.Price
                                };
                            }
                            break;

                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                                existExaminationFormInfo = await this.GetExaminationIndex(existExaminationFormInfo);
                                existExaminationFormInfo.Code = RandomUtilities.RandomString(6);
                                //if (updateExaminationStatus.ExaminationScheduleDetailId.HasValue && updateExaminationStatus.ExaminationScheduleDetailId.Value > 0)
                                //    existExaminationFormInfo.ExaminationScheduleDetailId = updateExaminationStatus.ExaminationScheduleDetailId.Value;
                                //else throw new Exception("Vui lòng chọn thông tin ca khám");
                                //if (updateExaminationStatus.RoomExaminationId.HasValue && updateExaminationStatus.RoomExaminationId.Value > 0)
                                //    existExaminationFormInfo.RoomExaminationId = updateExaminationStatus.RoomExaminationId.Value;
                                //else throw new Exception("Vui lòng chọn thông tin phòng khám");
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Code,
                                    x => x.ExaminationIndex,
                                    x => x.ExaminationPaymentIndex,
                                    x => x.PaymentMethodId,
                                    //x => x.ExaminationScheduleDetailId,
                                    //x => x.RoomExaminationId
                                };
                            }
                            break;
                        // Nếu trạng thái: đã hủy => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Cancel;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                };
                            }
                            break;
                        // Nếu trạng thái: Chờ xác nhận tái khám => lưu lại ngày tái khám trong phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Update;

                                if (updateExaminationStatus.ExaminationScheduleDetailId.HasValue && updateExaminationStatus.ExaminationScheduleDetailId.Value > 0)
                                    existExaminationFormInfo.ExaminationScheduleDetailId = updateExaminationStatus.ExaminationScheduleDetailId.Value;
                                else throw new Exception("Vui lòng chọn thông tin ca khám");
                                if (updateExaminationStatus.RoomExaminationId.HasValue && updateExaminationStatus.RoomExaminationId.Value > 0)
                                    existExaminationFormInfo.RoomExaminationId = updateExaminationStatus.RoomExaminationId.Value;
                                else throw new Exception("Vui lòng chọn thông tin phòng khám");
                                if (updateExaminationStatus.DoctorId.HasValue && updateExaminationStatus.DoctorId.Value > 0)
                                    existExaminationFormInfo.DoctorId = updateExaminationStatus.DoctorId.Value;
                                FeeCaculateExaminationRequest feeCaculateExaminationRequest = new FeeCaculateExaminationRequest()
                                {
                                    HospitalId = existExaminationFormInfo.HospitalId ?? 0,
                                    PaymentMethodId = existExaminationFormInfo.PaymentMethodId.Value,
                                    ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                                    SpecialistTypeId = existExaminationFormInfo.SpecialistTypeId
                                };
                                var hospitalConfigFee = await this.hospitalConfigFeeService.GetFeeExamination(feeCaculateExaminationRequest);
                                existExaminationFormInfo.Price = hospitalConfigFee.TotalPayment;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.ReExaminationDate,
                                    x => x.ExaminationScheduleDetailId,
                                    x => x.RoomExaminationId,
                                    x => x.DoctorId,
                                    x => x.Price
                                };
                            }
                            break;
                        // Nếu trạng thái: đã xác nhận tái khám => lưu lại ngày tái khám trong phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination;
                                existExaminationFormInfo = await this.GetExaminationIndex(existExaminationFormInfo);
                                if (existExaminationFormInfo.ReExaminationDate.HasValue)
                                {
                                    existExaminationFormInfo.ExaminationDate = existExaminationFormInfo.ReExaminationDate.Value;
                                    existExaminationFormInfo.ReExaminationDate = null;
                                }
                                else throw new Exception("Vui lòng chọn ngày tái khám");


                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.ExaminationDate,
                                    x => x.ReExaminationDate,
                                };
                            }
                            break;
                        // Nếu trạng thái: Hoàn thành khám => lưu lại ngày tái khám trong phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.FinishExamination;
                                existExaminationFormInfo.ReExaminationDate = null;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.ReExaminationDate
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
                    // Nếu trạng thái là chờ xác nhận tái khám hoặc hoàn thành khám bệnh => lưu lại thông tin vào chi tiết hồ sơ bệnh án của bệnh nhân
                    case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                    case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                        {
                            string examinationIndex = string.Empty;
                            string examinationPaymentIndex = string.Empty;

                            if (existExaminationFormInfo.PaymentMethodId.HasValue)
                            {
                                if (existExaminationFormInfo.PaymentMethodId == (int)CatalogueUtilities.PaymentMethod.COD)
                                {
                                    examinationIndex = existExaminationFormInfo.ExaminationIndex;
                                }
                                else
                                {
                                    examinationPaymentIndex = existExaminationFormInfo.ExaminationPaymentIndex;
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
                                ServiceTypeId = existExaminationFormInfo.ServiceTypeId,
                                SpecialistTypeId = existExaminationFormInfo.SpecialistTypeId,
                                HasMedicalBills = updateExaminationStatus.HasMedicalBill,
                                ExaminationScheduleDetailId = existExaminationFormInfo.ExaminationScheduleDetailId,
                                DoctorComment = updateExaminationStatus.DoctorComment,
                                ExaminationIndex = examinationIndex,
                                ExaminationPaymentIndex = examinationPaymentIndex,
                                PaymentMethodId = existExaminationFormInfo.PaymentMethodId,
                                TypeId = existExaminationFormInfo.TypeId,
                                DoctorId = existExaminationFormInfo.DoctorId,
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

                            QRCodeUtils qRCodeUtils = new QRCodeUtils(configuration, httpContextAccessor);
                            if (medicalRecordInfo != null)
                            {
                                var filePathUrl = qRCodeUtils.GetQrImagePath(medicalRecordInfo.UserId, medicalRecordDetails.Id);
                                var medicalRecordDetailInfo = await this.unitOfWork.Repository<MedicalRecordDetails>()
                                    .GetQueryable()
                                    .Where(e => e.Id == medicalRecordDetails.Id)
                                    .AsNoTracking().FirstOrDefaultAsync();
                                if (medicalRecordDetailInfo != null)
                                {
                                    medicalRecordDetailInfo.QrCodeUrlFile = filePathUrl;
                                    Expression<Func<MedicalRecordDetails, object>>[] expressions = new Expression<Func<MedicalRecordDetails, object>>[]
                                    {
                                e => e.QrCodeUrlFile
                                    };
                                    await this.unitOfWork.Repository<MedicalRecordDetails>().UpdateFieldsSaveAsync(medicalRecordDetailInfo, expressions);
                                }
                            }

                            // Thêm hóa đơn toa thuốc
                            if (updateExaminationStatus.HasMedicalBill && updateExaminationStatus.MedicalBills != null)
                            {
                                var existMedicalBill = await this.unitOfWork.Repository<MedicalBills>().GetQueryable()
                                    .Where(e => e.Id == updateExaminationStatus.MedicalBills.Id).FirstOrDefaultAsync();
                                if (existMedicalBill != null)
                                {
                                    existMedicalBill = mapper.Map<MedicalBills>(updateExaminationStatus.MedicalBills);
                                    existMedicalBill.Updated = DateTime.Now;
                                    existMedicalBill.UpdatedBy = updateExaminationStatus.CreatedBy;
                                    if (updateExaminationStatus.MedicalBills.Medicines != null
                                        && updateExaminationStatus.MedicalBills.Medicines.Any()
                                        )
                                        existMedicalBill.TotalPrice = updateExaminationStatus.MedicalBills.Medicines
                                            .Sum(e => (e.Price ?? 0) * (e.TotalAmount ?? 0));

                                    this.unitOfWork.Repository<MedicalBills>().Update(existMedicalBill);
                                }
                                else
                                {
                                    if (updateExaminationStatus.MedicalBills.Medicines != null
                                        && updateExaminationStatus.MedicalBills.Medicines.Any()
                                        )
                                        updateExaminationStatus.MedicalBills.TotalPrice = updateExaminationStatus.MedicalBills.Medicines
                                            .Sum(e => (e.Price ?? 0) * (e.TotalAmount ?? 0));


                                    updateExaminationStatus.MedicalBills.Deleted = false;
                                    updateExaminationStatus.MedicalBills.Active = true;
                                    updateExaminationStatus.MedicalBills.Created = DateTime.Now;
                                    updateExaminationStatus.MedicalBills.Status = (int)CatalogueUtilities.MedicalBillStatus.New;
                                    updateExaminationStatus.MedicalBills.MedicalRecordDetailId = medicalRecordDetails.Id;
                                    updateExaminationStatus.MedicalBills.HospitalId = medicalRecordDetails.HospitalId;
                                    updateExaminationStatus.MedicalBills.ExaminationFormId = medicalRecordDetails.ExaminationFormId;
                                    updateExaminationStatus.MedicalBills.CreatedBy = updateExaminationStatus.CreatedBy;
                                    updateExaminationStatus.MedicalBills.MedicalRecordId = existExaminationFormInfo.RecordId;
                                    updateExaminationStatus.MedicalBills.Id = 0;
                                    // TH1: Tạo mã toa thuốc tự động
                                    updateExaminationStatus.MedicalBills.Code = RandomUtilities.RandomString(8);
                                    // Th2: Lấy mã toa thuốc từ API bệnh viện
                                    //..............................
                                    await this.unitOfWork.Repository<MedicalBills>().CreateAsync(updateExaminationStatus.MedicalBills);
                                }
                                await this.unitOfWork.SaveAsync();
                                // Thêm thông tin chi tiết toa thuốc
                                if (updateExaminationStatus.MedicalBills.Medicines != null && updateExaminationStatus.MedicalBills.Medicines.Any())
                                {
                                    foreach (var medicine in updateExaminationStatus.MedicalBills.Medicines)
                                    {
                                        var existMedicine = await this.unitOfWork.Repository<Medicines>().GetQueryable().Where(e => e.Id == medicine.Id).FirstOrDefaultAsync();
                                        if (existMedicine != null)
                                        {
                                            medicine.MedicalBillId = updateExaminationStatus.MedicalBills.Id;
                                            existMedicine = mapper.Map<Medicines>(medicine);
                                            existMedicine.Updated = DateTime.Now;
                                            existMedicine.UpdatedBy = updateExaminationStatus.CreatedBy;
                                            this.unitOfWork.Repository<Medicines>().Update(medicine);
                                        }
                                        else
                                        {
                                            medicine.MedicalBillId = updateExaminationStatus.MedicalBills.Id;
                                            medicine.Created = DateTime.Now;
                                            medicine.Active = true;
                                            medicine.Deleted = false;
                                            medicine.Id = 0;
                                            medicine.CreatedBy = updateExaminationStatus.CreatedBy;
                                            await this.unitOfWork.Repository<Medicines>().CreateAsync(medicine);
                                        }
                                    }
                                    await this.unitOfWork.SaveAsync();
                                }

                            }

                            // Lưu thông tin file toa thuốc/ xét nghiệm/ siêu âm/ ....
                            if (updateExaminationStatus.MedicalRecordDetailFiles != null && updateExaminationStatus.MedicalRecordDetailFiles.Any())
                            {
                                foreach (var item in updateExaminationStatus.MedicalRecordDetailFiles)
                                {
                                    item.Created = DateTime.Now;
                                    item.CreatedBy = updateExaminationStatus.CreatedBy;
                                    item.Id = 0;
                                    item.MedicalRecordDetailId = medicalRecordDetails.Id;
                                    await unitOfWork.Repository<MedicalRecordDetailFiles>().CreateAsync(item);
                                }
                                await unitOfWork.SaveAsync();
                            }
                        }
                        break;
                    // Nếu trường hợp hủy => check lại account user
                    case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                        {
                            if (!isAdmin)
                                await UpdateUserInfo(existExaminationFormInfo.RecordId);
                        }
                        break;
                    //--------------------------- GỬI SMS STT CHO USER
                    //--------------------------- idnex string
                    //......................................................
                    case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                    case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
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
                        }
                        break;
                    //--------------------------- GỬI SMS THÔNG BÁO CHO USER THÔNG TIN THANH TOÁN KHÔNG HỢP LỆ
                    //--------------------------- idnex string
                    //......................................................
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentFailed:
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed:
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
                result = true;
            }


            //if (updateExaminationStatus.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed || updateExaminationStatus.Status == (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination)
            //{
            //    var medicalRecordInfo = await unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == existExaminationFormInfo.RecordId).FirstOrDefaultAsync();
            //    if (medicalRecordInfo != null)
            //    {
            //        var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync();
            //        if (userInfo != null)
            //        {
            //            if (!string.IsNullOrEmpty(existExaminationFormInfo.ExaminationIndex))
            //                await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", existExaminationFormInfo.ExaminationIndex));
            //            else if (!string.IsNullOrEmpty(existExaminationFormInfo.ExaminationPaymentIndex))
            //                await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", existExaminationFormInfo.ExaminationPaymentIndex));
            //        }
            //    }
            //}

            return result;
        }

        /// <summary>
        /// Tạo mới phiếu khám bệnh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ExaminationForms item)
        {
            bool result = false;
            if (item != null)
            {
                item.Id = 0;
                if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
                    item = await this.GetExaminationIndex(item);

                await unitOfWork.Repository<ExaminationForms>().CreateAsync(item);
                await unitOfWork.SaveAsync();

                // Tạo lịch sử tạo phiếu khám bệnh
                await CreateExaminationHistory(item, item.CreatedBy);

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
                                await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationIndex));
                            else if (!string.IsNullOrEmpty(item.ExaminationPaymentIndex))
                                await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationPaymentIndex));
                        }
                    }

                }
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin phiếu khám bệnh (lịch khám)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(ExaminationForms item)
        {
            bool result = false;
            if (item != null)
            {
                var existItem = await Queryable.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                if (existItem != null)
                {
                    item.Updated = DateTime.Now;
                    if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
                        item = await this.GetExaminationIndex(item);

                    existItem = mapper.Map<ExaminationForms>(item);
                    unitOfWork.Repository<ExaminationForms>().Update(existItem);
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
                                    await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationIndex));
                                else if (!string.IsNullOrEmpty(item.ExaminationPaymentIndex))
                                    await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", item.ExaminationPaymentIndex));
                            }
                        }
                    }
                    result = true;
                }
            }

            return result;
        }

        public async Task<ExaminationForms> GetExaminationIndex(ExaminationForms item)
        {
            if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value > 0 && item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
            {
                var paymentMethodInfo = await unitOfWork.Repository<PaymentMethods>().GetQueryable().Where(e => !e.Deleted && e.Active
                && e.Id == item.PaymentMethodId).FirstOrDefaultAsync();
                if (paymentMethodInfo != null)
                {
                    string indexString = string.Empty;
                    // Lưu lịch sử thanh toán
                    PaymentHistories paymentHistories = new PaymentHistories();
                    //TH1: Thanh toán COD => Lấy STT tại phòng khám
                    //=> Cập nhật lại STT tại phòng khám cho mẫu phiếu khám bệnh
                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString())
                    {
                        // Lấy STT khám tại BV
                        SearchExaminationIndex searchExaminationIndex = new SearchExaminationIndex()
                        {
                            HospitalId = item.HospitalId ?? 0,
                            ServiceTypeId = item.ServiceTypeId,
                            ExaminationDate = item.ExaminationDate
                        };
                        if (string.IsNullOrEmpty(item.ExaminationIndex))
                        {
                            item.ExaminationIndex = await this.GetExaminationFormIndex(searchExaminationIndex);
                            item.ExaminationPaymentIndex = string.Empty;
                            indexString = item.ExaminationIndex;
                        }

                    }
                    //TH2: Thanh toán qua App => Lấy STT đóng tiền khám
                    //=> Cập nhật lại STT đóng tiền cho mẫu phiếu khám bệnh
                    else
                    {
                        // Lấy STT đóng tiền tại BV
                        SearchExaminationIndex searchExaminationIndex = new SearchExaminationIndex()
                        {
                            HospitalId = item.HospitalId ?? 0,
                            ServiceTypeId = item.ServiceTypeId,
                            ExaminationDate = item.ExaminationDate
                        };
                        if (string.IsNullOrEmpty(item.ExaminationPaymentIndex))
                        {
                            item.ExaminationPaymentIndex = await this.GetExaminationFormPaymentIndex(searchExaminationIndex);
                            item.ExaminationIndex = string.Empty;
                            indexString = item.ExaminationPaymentIndex;
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
                        // Lưu lại thông tin số thứ tự
                        await CreatePaymentHistories(item, createdBy);
                    }
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                    {
                        action = (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination;
                        // Tạo lịch sử thanh toán
                        // Lưu lại thông tin số thứ tự
                        await CreatePaymentHistories(item, createdBy);
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
        private async Task CreatePaymentHistories(ExaminationForms examinationForms, string createdBy)
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
                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString())
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
                            Id = 0
                        };
                        await unitOfWork.Repository<PaymentHistories>().CreateAsync(paymentHistories);
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
                            BankInfo = bankInfos != null ? string.Format("STK: {0} - {1}", bankInfos.BankNo, bankInfos.BankBranch) : string.Empty
                        };
                        await unitOfWork.Repository<PaymentHistories>().CreateAsync(paymentHistories);
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

            bool isExistDetailSchedule = await Queryable.AnyAsync(x => !x.Deleted
            && x.Id != item.Id
            && (((!item.ExaminationScheduleDetailId.HasValue || item.ExaminationScheduleDetailId.Value <= 0) && x.ServiceTypeId == item.ServiceTypeId
                && (x.ExaminationDate.Year == item.ExaminationDate.Year
                && x.ExaminationDate.Month == item.ExaminationDate.Month && x.ExaminationDate.Date == item.ExaminationDate.Date))
            || (item.ExaminationScheduleDetailId.HasValue && item.ExaminationScheduleDetailId.Value > 0 && x.ExaminationScheduleDetailId == item.ExaminationScheduleDetailId
            && x.RecordId == item.RecordId))
            && x.Status != (int)CatalogueUtilities.ExaminationStatus.Canceled
            );
            if (isExistDetailSchedule)
                messages.Add("Đã tồn tại phiếu khám với thời gian này!");

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
                new SqlParameter("@ServiceTypeId", searchExaminationIndex.ServiceTypeId),
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
                new SqlParameter("@ServiceTypeId", searchExaminationIndex.ServiceTypeId),
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
                await this.unitOfWork.Repository<ExaminationForms>().UpdateFieldsSaveAsync(examinationFormCheck, includeExaminationProperties);
                // Cập nhật lại số lần vi phạm của user
                await UpdateUserInfo(examinationFormCheck.RecordId);
            }
        }

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
                                        e => e.LockedDate
                    };
                    await this.unitOfWork.Repository<Users>().UpdateFieldsSaveAsync(userInfo, includeUserProperties);
                    await unitOfWork.SaveAsync();
                    await this.medicalDbContext.SaveChangesAsync();
                    medicalDbContext.Entry(userInfo).State = EntityState.Detached;
                }
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

    }
}
