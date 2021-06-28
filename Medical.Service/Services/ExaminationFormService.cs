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
        public ExaminationFormService(IMedicalUnitOfWork unitOfWork, IMapper mapper, ISMSConfigurationService sMSConfigurationService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IMedicalDbContext medicalDbContext) : base(unitOfWork, mapper)
        {
            this.sMSConfigurationService = sMSConfigurationService;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
            this.medicalDbContext = medicalDbContext;
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationForm_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchExaminationForm baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@RecordId", baseSearch.RecordId),
                new SqlParameter("@TypeId", baseSearch.TypeId),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@ReExaminationDate", baseSearch.ReExaminationDate),
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
        public async Task<bool> UpdateExaminationStatus(UpdateExaminationStatus updateExaminationStatus)
        {
            bool result = false;
            int? action = null;
            var existExaminationFormInfo = await Queryable.Where(e => e.Id == updateExaminationStatus.ExaminationFormId).FirstOrDefaultAsync();
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
                                action = (int)CatalogueUtilities.ExaminationAction.Update;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.PaymentMethodId,
                                };

                            }
                            break;

                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                                existExaminationFormInfo = await this.GetExaminationIndex(existExaminationFormInfo);
                                existExaminationFormInfo.Code = RandomUtilities.RandomString(6);
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Code,
                                    x => x.ExaminationIndex,
                                    x => x.ExaminationPaymentIndex,
                                    x => x.PaymentMethodId,
                                };

                            }
                            break;
                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
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
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.ReExaminationDate
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
                                HasMedicalBills = updateExaminationStatus.HasMedicalBill
                            };

                            await unitOfWork.Repository<MedicalRecordDetails>().CreateAsync(medicalRecordDetails);

                            await unitOfWork.SaveAsync();
                            await this.medicalDbContext.SaveChangesAsync();

                            // Lấy thông tin qrcode hình
                            this.medicalDbContext.Entry<MedicalRecordDetails>(medicalRecordDetails).State = EntityState.Detached;
                            var medicalRecordInfo = await this.medicalDbContext.Set<MedicalRecords>()
                                .Where(e => !e.Deleted && e.Id == existExaminationFormInfo.RecordId)
                                .AsNoTracking().FirstOrDefaultAsync();
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
                                    this.unitOfWork.Repository<MedicalBills>().Update(existMedicalBill);
                                }
                                else
                                {
                                    updateExaminationStatus.MedicalBills.Deleted = false;
                                    updateExaminationStatus.MedicalBills.Active = true;
                                    updateExaminationStatus.MedicalBills.Created = DateTime.Now;
                                    updateExaminationStatus.MedicalBills.Status = (int)CatalogueUtilities.MedicalBillStatus.New;
                                    updateExaminationStatus.MedicalBills.MedicalRecordDetailId = medicalRecordDetails.Id;
                                    updateExaminationStatus.MedicalBills.HospitalId = medicalRecordDetails.HospitalId;
                                    updateExaminationStatus.MedicalBills.ExaminationFormId = medicalRecordDetails.ExaminationFormId;
                                    updateExaminationStatus.MedicalBills.CreatedBy = updateExaminationStatus.CreatedBy;
                                    updateExaminationStatus.MedicalBills.MedicalRecordId = existExaminationFormInfo.RecordId;
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
                                            await this.unitOfWork.Repository<Medicines>().CreateAsync(medicine);
                                        }
                                        else
                                        {
                                            medicine.MedicalBillId = updateExaminationStatus.MedicalBills.Id;
                                            medicine.Created = DateTime.Now;
                                            medicine.Active = true;
                                            medicine.Deleted = false;
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
                                    item.MedicalRecordDetailId = medicalRecordDetails.Id;
                                    await unitOfWork.Repository<MedicalRecordDetailFiles>().CreateAsync(item);
                                    await unitOfWork.SaveAsync();
                                }
                            }
                        }
                        break;
                    // Nếu trường hợp hủy => check lại account user
                    case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                        {
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
                ExaminationPaymentIndex = item.ExaminationPaymentIndex
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
                            PaymentMethodName = paymentMethodInfo.Name
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

            bool isExistDetailSchedule = await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.ExaminationScheduleDetailId == item.ExaminationScheduleDetailId);
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
            DateTime dateCheck = DateTime.Now + ts;

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

        private async Task UpdateUserInfo(int recordId)
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
                    if (!userInfo.TotalViolations.HasValue || userInfo.TotalViolations.Value == 0)
                        userInfo.TotalViolations = 1;
                    else userInfo.TotalViolations += 1;
                    if (userInfo.TotalViolations == 3)
                    {
                        userInfo.IsLocked = true;
                        userInfo.TotalViolations = 0;
                        userInfo.LockedDate = DateTime.Now.AddDays(30);
                    }
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

    }
}
