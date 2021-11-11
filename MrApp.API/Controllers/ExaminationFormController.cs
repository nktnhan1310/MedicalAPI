using AutoMapper;
using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/examination-form")]
    [ApiController]
    [Authorize]
    [Description("Quản lý phiếu khám bệnh")]
    public class ExaminationFormController : BaseController
    {
        private readonly IExaminationHistoryService examinationHistoryService;
        private readonly IPaymentHistoryService paymentHistoryService;
        private readonly IExaminationFormService examinationFormService;
        private readonly IMedicalRecordService medicalRecordService;
        private readonly IExaminationScheduleService examinationScheduleService;
        private readonly IExaminationScheduleDetailService examinationScheduleDetailService;
        private readonly ISpecialListTypeService specialListTypeService;
        private readonly IPaymentMethodService paymentMethodService;
        private readonly IBankInfoService bankInfoService;
        private readonly IHospitalConfigFeeService hospitalConfigFeeService;
        private readonly IDoctorService doctorService;
        private readonly IMomoPaymentService momoPaymentService;
        private readonly IMomoConfigurationService momoConfigurationService;
        private readonly IDoctorDetailService doctorDetailService;
        private readonly IHospitalFileService hospitalFileService;
        private readonly IMedicalRecordDetailService medicalRecordDetailService;
        private readonly IVaccineTypeService vaccineTypeService;
        private readonly IExaminationFormAdditionServiceMappingService examinationFormAdditionServiceMappingService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IVNPayPaymentHistoryService vNPayPaymentHistoryService;
        private readonly IAdditionServiceType additionServiceType;
        private readonly IRoomExaminationService roomExaminationService;
        private readonly IDegreeTypeService degreeTypeService;
        private readonly IExaminationFormAdditionServiceDetailMappingService examinationFormAdditionServiceDetailMappingService;
        public ExaminationFormController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor
            ) : base(serviceProvider, logger, env, mapper, configuration)
        {
            paymentHistoryService = serviceProvider.GetRequiredService<IPaymentHistoryService>();
            examinationHistoryService = serviceProvider.GetRequiredService<IExaminationHistoryService>();
            examinationFormService = serviceProvider.GetRequiredService<IExaminationFormService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            examinationScheduleService = serviceProvider.GetRequiredService<IExaminationScheduleService>();
            examinationScheduleDetailService = serviceProvider.GetRequiredService<IExaminationScheduleDetailService>();
            specialListTypeService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
            paymentMethodService = serviceProvider.GetRequiredService<IPaymentMethodService>();
            bankInfoService = serviceProvider.GetRequiredService<IBankInfoService>();
            hospitalConfigFeeService = serviceProvider.GetRequiredService<IHospitalConfigFeeService>();
            doctorService = serviceProvider.GetRequiredService<IDoctorService>();
            momoPaymentService = serviceProvider.GetRequiredService<IMomoPaymentService>();
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
            doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
            hospitalFileService = serviceProvider.GetRequiredService<IHospitalFileService>();
            medicalRecordDetailService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            vaccineTypeService = serviceProvider.GetRequiredService<IVaccineTypeService>();
            examinationFormAdditionServiceMappingService = serviceProvider.GetRequiredService<IExaminationFormAdditionServiceMappingService>();
            vNPayPaymentHistoryService = serviceProvider.GetRequiredService<IVNPayPaymentHistoryService>();
            additionServiceType = serviceProvider.GetRequiredService<IAdditionServiceType>();
            roomExaminationService = serviceProvider.GetRequiredService<IRoomExaminationService>();
            degreeTypeService = serviceProvider.GetRequiredService<IDegreeTypeService>();
            examinationFormAdditionServiceDetailMappingService = serviceProvider.GetRequiredService<IExaminationFormAdditionServiceDetailMappingService>();

            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Lấy thông tin danh sách bác sĩ theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="typeId"></param>
        /// <param name="specialistTypeId"></param>
        /// <returns></returns>
        [HttpGet("get-list-doctor-by-hospital/{hospitalId}/specialistTypeId")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetDoctorByHospital(int hospitalId, int? specialistTypeId, int? typeId)
        {
            if (hospitalId <= 0) throw new AppException("Vui lòng chọn bệnh viện");
            var doctors = await this.doctorService.GetAsync(e => !e.Deleted && e.Active 
            && e.HospitalId == hospitalId
            && (!typeId.HasValue || (typeId.Value == 0 && e.TypeId == 0) || e.TypeId != 0)
            );
            if (doctors != null && doctors.Any())
            {
                var doctorIds = doctors.Select(e => e.Id).ToList();
                var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted
                && doctorIds.Contains(e.DoctorId)
                && (!specialistTypeId.HasValue || e.SpecialistTypeId == specialistTypeId.Value)
                );


                var doctorIdsBySpecialistTypes = doctorDetails.Select(e => e.DoctorId).ToList();
                if (doctorIdsBySpecialistTypes != null && doctorIdsBySpecialistTypes.Any())
                    doctors = await this.doctorService.GetAsync(e => !e.Deleted
                    && e.Active
                    && e.HospitalId == hospitalId
                    && doctorIdsBySpecialistTypes.Contains(e.Id));
                else doctors = null;
            }

            return new AppDomainResult()
            {
                Data = mapper.Map<IList<DoctorModel>>(doctors),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy ra danh sách bác sĩ khám bệnh
        /// </summary>
        /// <param name="searchExaminationScheduleDetailV2"></param>
        /// <returns></returns>
        [HttpGet("get-doctor-examination")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetDoctorExamination([FromQuery] SearchExaminationScheduleDetailV2 searchExaminationScheduleDetailV2)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (!searchExaminationScheduleDetailV2.HospitalId.HasValue || searchExaminationScheduleDetailV2.HospitalId.Value <= 0)
                throw new AppException("Vui lòng chọn bệnh viện");
            var doctorExaminations = await this.doctorService.GetListDoctorExaminations(searchExaminationScheduleDetailV2);
            var doctorExaminationModels = mapper.Map<PagedList<DoctorDetailModel>>(doctorExaminations);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = doctorExaminationModels
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy tất cả thông tin chuyên khoa khám bệnh
        /// </summary>
        /// <param name="searchExaminationScheduleDetailV2"></param>
        /// <returns></returns>
        [HttpGet("get-all-examination-schedules")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetAllExaminationSchedules([FromQuery] SearchExaminationScheduleDetailV2 searchExaminationScheduleDetailV2)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (!searchExaminationScheduleDetailV2.HospitalId.HasValue || searchExaminationScheduleDetailV2.HospitalId.Value <= 0)
                throw new AppException("Vui lòng chọn bệnh viện");
            var examinationSchedules = await this.examinationScheduleService.GetAllExaminationSchedules(searchExaminationScheduleDetailV2);
            var examinationScheduleModels = mapper.Map<PagedList<ExaminationScheduleModel>>(examinationSchedules);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = examinationScheduleModels
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin danh sách ca khám theo bệnh viện và chuyên khoa được chọn
        /// </summary>
        /// <param name="searchExaminationScheduleForm"></param>
        /// <returns></returns>
        [HttpGet("get-config-time-examination-by-specialist-type")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetConfigTimeExaminationBySpecialistType([FromQuery] SearchExaminationScheduleForm searchExaminationScheduleForm)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (searchExaminationScheduleForm.HospitalId <= 0)
                throw new AppException("Vui lòng chọn bệnh viện");
            var examinationSchedules = await this.examinationScheduleService.GetExaminationSchedules(searchExaminationScheduleForm);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = examinationSchedules
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách ngày khám theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="searchExaminationDate"></param>
        /// <returns></returns>
        [HttpGet("get-list-examination-date-by-hospital/{hospitalId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetListDateExaminationByHospital(int hospitalId, [FromQuery] SearchExaminationDate searchExaminationDate)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (hospitalId <= 0)
                throw new AppException("Vui lòng chọn bệnh viện");
            var schedules = await this.examinationScheduleService.GetAsync(e => !e.Deleted && e.Active
            && e.HospitalId == hospitalId
            && e.ExaminationDate.Date >= DateTime.Now.Date
            && (!searchExaminationDate.DoctorId.HasValue || e.DoctorId == searchExaminationDate.DoctorId.Value)
            && (!searchExaminationDate.SpecialistTypeId.HasValue || e.SpecialistTypeId == searchExaminationDate.SpecialistTypeId.Value)
            );
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<ExaminationScheduleModel>>(schedules)
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin danh sách chuyên khoa theo ngày khám được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="examinationDate"></param>
        /// <returns></returns>
        [HttpGet("get-specialist-type-by-date/{hospitalId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetSpecialistTypeByExaminationDate(int hospitalId, [FromQuery] DateTime? examinationDate)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (hospitalId <= 0)
                throw new AppException("Vui lòng chọn bệnh viện");
            var specialistTypeByExaminationDates = await this.examinationScheduleService.GetAsync(e => !e.Deleted && e.Active
            && e.HospitalId == hospitalId
            && (!examinationDate.HasValue || e.ExaminationDate.Date == examinationDate.Value.Date)
            , e => new ExaminationSchedules()
            {
                SpecialistTypeId = e.SpecialistTypeId
            });
            var specialistTypeIds = specialistTypeByExaminationDates.Select(e => e.SpecialistTypeId).ToList();
            var specialistTypes = await this.specialListTypeService.GetAsync(e => !e.Deleted && e.Active && specialistTypeIds.Contains(e.Id));
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<SpecialistTypeModel>>(specialistTypes),
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin lịch sử phiếu khám (lịch hen)
        /// </summary>
        /// <param name="searchExaminationFormHistory"></param>
        /// <returns></returns>
        [HttpGet("get-examination-form-history")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetExaminationHistory([FromQuery] SearchExaminationFormHistory searchExaminationFormHistory)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            PagedList<ExaminationHistoryModel> pagedListModel = new PagedList<ExaminationHistoryModel>();
            searchExaminationFormHistory.UserId = LoginContext.Instance.CurrentUser.UserId;

            var pagedList = await this.examinationHistoryService.GetPagedListData(searchExaminationFormHistory);
            if (pagedList != null && pagedList.Items.Any())
                pagedListModel = mapper.Map<PagedList<ExaminationHistoryModel>>(pagedList);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = pagedListModel
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin lịch sử thanh toán
        /// </summary>
        /// <param name="examinationFormId"></param>
        /// <returns></returns>
        [HttpGet("get-payment-history/{examinationFormId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetPaymentHistory(int examinationFormId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var paymentHistories = await this.paymentHistoryService.GetAsync(x => !x.Deleted && x.ExaminationFormId == examinationFormId);
            var paymentHistoryModels = mapper.Map<IList<PaymentHistoryModel>>(paymentHistories);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = paymentHistoryModels
            };

            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu khám
        /// </summary>
        /// <param name="updateExaminationStatusModel"></param>
        /// <returns></returns>
        [HttpPost("update-examination-status")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateExaminationStatus([FromBody] UpdateExaminationStatusModel updateExaminationStatusModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            MomoResponseModel momoResponseModel = null;
            bool isSuccess = false;
            string paymentUrl = string.Empty;
            if (ModelState.IsValid)
            {
                // KIỂM TRA TRẠNG THÁI HIỆN TẠI VỚI TRẠNG THÁI CẬP NHẬT PHIẾU
                string checkMessage = await this.examinationFormService.GetCheckStatusMessage(updateExaminationStatusModel.ExaminationFormId, updateExaminationStatusModel.Status ?? 0);
                if (!string.IsNullOrEmpty(checkMessage)) throw new AppException(checkMessage);

                // KIỂM TRA CÓ THANH TOÁN HAY KHÔNG?
                if (updateExaminationStatusModel.PaymentMethodId.HasValue && updateExaminationStatusModel.PaymentMethodId.Value > 0 && updateExaminationStatusModel.Status == (int)CatalogueUtilities.ExaminationStatus.WaitConfirm)
                {
                    var paymentMethodInfos = await this.paymentMethodService.GetAsync(e => !e.Deleted && e.Active && e.Id == updateExaminationStatusModel.PaymentMethodId.Value);
                    if (paymentMethodInfos != null && paymentMethodInfos.Any())
                    {
                        var paymentMethodInfo = paymentMethodInfos.FirstOrDefault();
                        // THANH TOÁN QUA MOMO => Trạng thái chờ xác nhận => Chờ thanh toán thành công => Cập nhật trạng thái
                        if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.MOMO.ToString())
                        {
                            momoResponseModel = await GetResponseMomoPayment(updateExaminationStatusModel);
                        }
                        // THANH TOÁN QUA MOMO => Trạng thái chờ xác nhận => Chờ thanh toán thành công => Cập nhật trạng thái
                        if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.VNPAY.ToString())
                        {
                            paymentUrl = await GetVNPayPaymentUrl(updateExaminationStatusModel);
                        }
                    }
                }
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                if (updateExaminationStatusModel.MedicalRecordDetailFiles != null && updateExaminationStatusModel.MedicalRecordDetailFiles.Any())
                {
                    foreach (var file in updateExaminationStatusModel.MedicalRecordDetailFiles)
                    {
                        string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);
                        // ------- START GET URL FOR FILE
                        string folderUploadPath = string.Empty;
                        var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                        folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                        string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
                        // Kiểm tra có tồn tại file trong temp chưa?
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                        {

                            FileUtils.CreateDirectory(folderUploadPath);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                            folderUploadPaths.Add(fileUploadPath);
                            string fileUrl = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
                            // ------- END GET URL FOR FILE
                            filePaths.Add(filePath);
                            file.Created = DateTime.Now;
                            file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                            file.Active = true;
                            file.Deleted = false;
                            file.FileName = Path.GetFileName(filePath);
                            file.FileExtension = Path.GetExtension(filePath);
                            file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                            file.FileUrl = fileUrl;
                        }
                        else
                        {
                            file.Updated = DateTime.Now;
                            file.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                        }
                    }
                }

                var updateExaminationStatus = mapper.Map<UpdateExaminationStatus>(updateExaminationStatusModel);
                isSuccess = await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus);
                if (isSuccess)
                {
                    appDomainResult.Success = isSuccess;
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                }
                else
                    throw new Exception("Lỗi trong quá trình xử lý");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return new AppDomainResult()
            {
                Data = new
                {
                    MoMoResponse = momoResponseModel,
                    VNPayPaymentUrl = paymentUrl
                },
                Success = isSuccess,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Trả lại thông tin phản hồi từ momo
        /// </summary>
        /// <param name="updateExaminationStatusModel"></param>
        /// <returns></returns>
        private async Task<MomoResponseModel> GetResponseMomoPayment(UpdateExaminationStatusModel updateExaminationStatusModel)
        {
            MomoResponseModel momoResponseModel = null;
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            MomoConfigurations momoConfiguration = await this.momoConfigurationService.GetSingleAsync(e => !e.Deleted && e.Active);
            if (momoConfiguration != null)
            {
                var examinationInfo = await this.examinationFormService.GetSingleAsync(e => e.Id == updateExaminationStatusModel.ExaminationFormId);

                string orderInfo = "test";
                string amount = (examinationInfo != null && examinationInfo.Price.HasValue) ? (examinationInfo.Price.Value.ToString()) : "0";
                string orderid = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                momoConfiguration.PartnerCode + "&accessKey=" +
                momoConfiguration.AccessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                momoConfiguration.ReturnUrlWebApp + "&notifyUrl=" +
                momoConfiguration.NotifyUrl + "&extraData=" +
                extraData;

                MomoUtilities crypto = new MomoUtilities();
                //sign signature SHA256
                string signature = crypto.SignSHA256(rawHash, momoConfiguration.SecretKey);

                //build body json request
                JObject message = new JObject
                {
                    { "partnerCode", momoConfiguration.PartnerCode },
                    { "accessKey", momoConfiguration.AccessKey },
                    { "requestId", requestId },
                    { "amount", amount },
                    { "orderId", orderid },
                    { "orderInfo", orderInfo },
                    { "returnUrl", momoConfiguration.ReturnUrlWebApp },
                    { "notifyUrl", momoConfiguration.NotifyUrl },
                    { "extraData", extraData },
                    { "requestType", "captureMoMoWallet" },
                    { "signature", signature }

                };
                string responseFromMomo = crypto.SendPaymentRequest(endpoint, message.ToString());
                momoResponseModel = JsonConvert.DeserializeObject<MomoResponseModel>(responseFromMomo);
                if (momoResponseModel != null && momoResponseModel.errorCode == 0)
                {
                    MomoPayments momoPayments = new MomoPayments()
                    {
                        Created = DateTime.Now,
                        CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                        Active = true,
                        Deleted = false,
                        Amount = Convert.ToInt64(amount),
                        RequestId = requestId,
                        OrderId = orderid,
                        OrderInfo = orderInfo,
                        Signature = signature,
                        ExaminationFormId = updateExaminationStatusModel.ExaminationFormId
                    };
                    bool success = await this.momoPaymentService.CreateAsync(momoPayments);
                    if (!success) throw new AppException("Không lưu được thông tin thanh toán");
                }
            }
            else throw new AppException("Không lấy được cấu hình thanh toán momo");
            return momoResponseModel;
        }

        /// <summary>
        /// Lấy url thanh toán của vnpay
        /// </summary>
        /// <param name="updateExaminationStatusModel"></param>
        /// <returns></returns>
        private async Task<string> GetVNPayPaymentUrl(UpdateExaminationStatusModel updateExaminationStatusModel)
        {
            string result = string.Empty;

            string vnp_Returnurl = configuration.GetSection("MySettings:vnp_Returnurl").Value.ToString();
            string vnp_Url = configuration.GetSection("MySettings:vnp_Url").Value.ToString(); //URL thanh toan cua VNPAY 
            string vnp_TmnCode = configuration.GetSection("MySettings:vnp_TmnCode").Value.ToString(); //Ma website
            string vnp_HashSecret = configuration.GetSection("MySettings:vnp_HashSecret").Value.ToString(); //Chuoi bi mat
            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                throw new AppException("Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong appsetting");
            long orderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            long amount = 0; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND

            // LẤY THÔNG TIN TỔNG GIÁ TRỊ CỦA PHIẾU CẦN THANH TOÁN
            var examinationFormInfo = await this.examinationFormService.GetSingleAsync(e => e.Id == updateExaminationStatusModel.ExaminationFormId);
            if (examinationFormInfo != null && examinationFormInfo.Price.HasValue)
                amount = Convert.ToInt64(examinationFormInfo.Price.Value);

            string status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
            string orderDescription = "test";
            DateTime createdDate = DateTime.Now;
            VNPayUtilities vnpay = new VNPayUtilities();
            vnpay.AddRequestData("vnp_Version", VNPayUtilities.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "NCB");
            vnpay.AddRequestData("vnp_CreateDate", createdDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", VNPayUtilities.GetIpAddress(httpContextAccessor));
            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", createdDate.AddMinutes(30).ToString("yyyyMMddHHmmss"));
            result = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            // NẾU LẤY ĐƯỢC THÔNG TIN THANH TOÁN CỦA VNPAY
            // LƯU THÔNG TIN THANH TOÁN VN PAY VÀO BẢNG LỊCH SỬ THANH TOÁN CỦA VNPAY
            if (!string.IsNullOrEmpty(result))
            {
                VNPayPaymentHistories vNPayPaymentHistories = new VNPayPaymentHistories()
                {
                    Created = DateTime.Now,
                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                    Active = true,
                    Deleted = false,
                    Amount = amount,
                    ExaminationFormId = updateExaminationStatusModel.ExaminationFormId,
                    OrderId = orderId,
                    UserId = LoginContext.Instance.CurrentUser.UserId,
                    Status = status,
                };
                await this.vNPayPaymentHistoryService.CreateAsync(vNPayPaymentHistories);
            }
            return result;
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public virtual async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            SearchExaminationForm searchExaminationForm = new SearchExaminationForm()
            {
                PageIndex = 1,
                PageSize = 1,
                ExaminationFormId = id,
                OrderBy = "Id desc"
            };
            ExaminationForms item = null;
            var pagedItems = await this.examinationFormService.GetPagedListData(searchExaminationForm);
            if (pagedItems != null && pagedItems.Items.Any())
            {
                item = pagedItems.Items.FirstOrDefault();
            }

            //var item = await this.examinationFormService.GetByIdAsync(id);

            if (item != null)
            {
                var itemModel = mapper.Map<ExaminationFormModel>(item);
                // Lấy thông tin chi tiết lịch nếu có
                if (item.ExaminationScheduleDetailId.HasValue && item.ExaminationScheduleDetailId.Value > 0)
                {
                    SearchExaminationScheduleForm searchExaminationScheduleForm = new SearchExaminationScheduleForm()
                    {
                        HospitalId = item.HospitalId ?? 0,
                        DoctorId = item.DoctorId,
                        ExaminationScheduleDetailId = item.ExaminationScheduleDetailId.Value,
                        ExaminationDate = item.ExaminationDate,
                        SpecialistTypeId = item.SpecialistTypeId ?? 0
                    };
                    var examinationScheduleDetails = await this.examinationScheduleService.GetExaminationScheduleDetails(searchExaminationScheduleForm);
                    if (examinationScheduleDetails != null && examinationScheduleDetails.Any())
                    {
                        itemModel.ExaminationScheduleDetail = mapper.Map<ExaminationScheduleDetailModel>(examinationScheduleDetails.FirstOrDefault());
                    }
                }

                // Lấy thông tin dịch vụ phát sinh của phiếu khám (nếu có)
                var additionServiceInfos = await this.examinationFormAdditionServiceMappingService
                    .GetAsync(e => !e.Deleted
                && e.ExaminationFormId == item.Id);
                if (additionServiceInfos != null && additionServiceInfos.Any())
                {
                    itemModel.AdditionServiceIds = additionServiceInfos.Select(e => e.AdditionServiceId).ToList();
                    itemModel.ExaminationFormServiceMappings = mapper.Map<IList<ExaminationFormAdditionServiceMappingModel>>(additionServiceInfos);
                    var additionServiceTypeInfos = await this.additionServiceType.GetAsync(e => itemModel.AdditionServiceIds.Contains(e.Id));
                    if (additionServiceTypeInfos.Any(x => x.IsVaccineSelected)) itemModel.IsVaccineSelected = true;
                    foreach (var examinationFormServiceMapping in itemModel.ExaminationFormServiceMappings)
                    {
                        var additionServiceTypeInfo = additionServiceTypeInfos.Where(e => e.Id == examinationFormServiceMapping.AdditionServiceId).FirstOrDefault();
                        if (additionServiceTypeInfo != null) examinationFormServiceMapping.AdditionServiceName = additionServiceTypeInfo.Name;
                    }
                }

                // Lấy ra thông tin chi tiết của dịch vụ (nếu có)
                var additionServiceDetailInfos = await this.examinationFormAdditionServiceDetailMappingService
                    .GetAsync(e => !e.Deleted && e.ExaminationFormId == item.Id);
                if (additionServiceDetailInfos != null && additionServiceDetailInfos.Any())
                {
                    itemModel.AdditionServiceDetailIds = additionServiceDetailInfos.Select(e => e.AdditionServiceDetailId).ToList();
                    itemModel.ExaminationFormAdditionServiceDetailMappings = mapper.Map<IList<ExaminationFormAdditionServiceDetailMappingModel>>(additionServiceDetailInfos);
                }

                // Lây thông tin lịch sử
                //var examinationHistories = await this.examinationHistoryService.GetAsync(e => !e.Deleted && e.ExaminationFormId == item.Id);
                //if (examinationHistories != null && examinationHistories.Any())
                //    itemModel.ExaminationHistories = mapper.Map<IList<ExaminationHistoryModel>>(examinationHistories.OrderByDescending(e => e.Id));
                // Lây thông tin lịch sử thanh toán
                //var payentHistories = await this.paymentHistoryService.GetAsync(e => !e.Deleted && e.ExaminationFormId == item.Id);
                //if (payentHistories != null && payentHistories.Any())
                //    itemModel.PaymentHistories = mapper.Map<IList<PaymentHistoryModel>>(payentHistories.OrderByDescending(e => e.Id));
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin lịch khám gần nhất user đặt
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-latest-user-examination")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetLatestUserExamination()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            SearchExaminationForm searchExaminationForm = new SearchExaminationForm()
            {
                PageIndex = 1,
                PageSize = 20,
                UserId = LoginContext.Instance.CurrentUser.UserId,
                Status = (int)CatalogueUtilities.ExaminationStatus.FinishExamination,
                OrderBy = "Created desc"
            };

            ExaminationForms item = null;
            var pagedItems = await this.examinationFormService.GetPagedListData(searchExaminationForm);
            if (pagedItems != null && pagedItems.Items.Any())
            {
                item = pagedItems.Items.FirstOrDefault();
                var itemModel = mapper.Map<ExaminationFormModel>(item);
                // Lấy thông tin dịch vụ phát sinh của phiếu khám (nếu có)
                var additionServiceInfos = await this.examinationFormAdditionServiceMappingService
                    .GetAsync(e => !e.Deleted
                && e.ExaminationFormId == item.Id);
                if (additionServiceInfos != null && additionServiceInfos.Any())
                {
                    itemModel.AdditionServiceIds = additionServiceInfos.Select(e => e.AdditionServiceId).ToList();
                    itemModel.ExaminationFormServiceMappings = mapper.Map<IList<ExaminationFormAdditionServiceMappingModel>>(additionServiceInfos);
                    var additionServiceTypeInfos = await this.additionServiceType.GetAsync(e => itemModel.AdditionServiceIds.Contains(e.Id));
                    if (additionServiceTypeInfos.Any(x => x.IsVaccineSelected)) itemModel.IsVaccineSelected = true;
                    foreach (var examinationFormServiceMapping in itemModel.ExaminationFormServiceMappings)
                    {
                        var additionServiceTypeInfo = additionServiceTypeInfos.Where(e => e.Id == examinationFormServiceMapping.AdditionServiceId).FirstOrDefault();
                        if (additionServiceTypeInfo != null) examinationFormServiceMapping.AdditionServiceName = additionServiceTypeInfo.Name;
                    }
                }
                // Lấy thông tin chi tiết lịch nếu có
                //if (item.ExaminationScheduleDetailId.HasValue && item.ExaminationScheduleDetailId.Value > 0)
                //{

                //    //SearchExaminationScheduleForm searchExaminationScheduleForm = new SearchExaminationScheduleForm()
                //    //{
                //    //    HospitalId = item.HospitalId ?? 0,
                //    //    DoctorId = item.DoctorId,
                //    //    ExaminationScheduleDetailId = item.ExaminationScheduleDetailId.Value,
                //    //    ExaminationDate = item.ExaminationDate,
                //    //    SpecialistTypeId = item.SpecialistTypeId ?? 0
                //    //};
                //    //var examinationScheduleDetails = await this.examinationScheduleService.GetExaminationScheduleDetails(searchExaminationScheduleForm);
                //    //if (examinationScheduleDetails != null && examinationScheduleDetails.Any())
                //    //{
                //    //    itemModel.ExaminationScheduleDetail = mapper.Map<ExaminationScheduleDetailModel>(examinationScheduleDetails.FirstOrDefault());
                //    //}
                //}
                return new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else return new AppDomainResult()
            {
                Success = false,
                Data = null,
                ResultCode = (int)HttpStatusCode.NoContent
            };
        }

        /// <summary>
        /// Đăng kí lịch khám
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddItem([FromBody] ExaminationFormModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (!itemModel.HospitalId.HasValue || itemModel.HospitalId.Value <= 0)
                    throw new AppException("Vui lòng chọn bệnh viện!");
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<ExaminationForms>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.examinationFormService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.examinationFormService.CreateAsync(item);
                    if (success)
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                        appDomainResult.Data = item;
                    }
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.Success = success;
                }
                else
                    throw new AppException(ModelState.GetErrorMessage());
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin lịch
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateItem(int id, [FromBody] ExaminationFormModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (!itemModel.HospitalId.HasValue || itemModel.HospitalId.Value <= 0)
                    throw new AppException("Vui lòng chọn bệnh viện!");
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<ExaminationForms>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.examinationFormService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.examinationFormService.UpdateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = await this.examinationFormService.DeleteAsync(id);
            if (success)
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");

            return appDomainResult;
        }

        /// <summary>
        /// Xóa array item
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-multiples")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteItem([FromBody] List<int> itemIds)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = true;
            if (itemIds != null && itemIds.Any())
            {
                foreach (var itemId in itemIds)
                {
                    success &= await this.examinationFormService.DeleteAsync(itemId);
                }
            }
            if (success)
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");

            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetPagedData([FromQuery] SearchExaminationForm baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                //if (!baseSearch.HospitalId.HasValue || baseSearch.HospitalId.Value <= 0)
                //    throw new AppException("Vui lòng chọn bệnh viện");
                baseSearch.UserId = LoginContext.Instance.CurrentUser.UserId;
                PagedList<ExaminationForms> pagedData = await this.examinationFormService.GetPagedListData(baseSearch);


                PagedList<ExaminationFormModel> pagedDataModel = mapper.Map<PagedList<ExaminationFormModel>>(pagedData);
                if (pagedDataModel != null && pagedDataModel.Items != null && pagedDataModel.Items.Any())
                {
                    foreach (var item in pagedDataModel.Items)
                    {
                        var hospitalFiles = await hospitalFileService.GetAsync(e => !e.Deleted && e.HospitalId == item.HospitalId && e.FileType == (int)CatalogueUtilities.HospitalFileType.Logo);
                        if (hospitalFiles != null && hospitalFiles.Any())
                        {
                            item.HospitalFiles = mapper.Map<IList<HospitalFileModel>>(hospitalFiles);
                        }

                        // KIỂM TRA THỂ HIỆN TRẠNG THÁI CHÍCH NGỪA
                        // NẾU LÀ KHÁC KHÁM THƯỜNG + KO CHỌN VACCINE => KHÔNG CẦN THỂ HIỆN TRẠNG THÁI VACCINE
                        if (item.TypeId != 0 || !item.VaccineTypeId.HasValue || item.VaccineTypeId.Value <= 0) continue;

                        // LẤY RA TẤT CẢ PHIẾU ĐANG ĐĂNG KÍ TIÊM LOẠI VACCINE THEO PHIẾU HIỆN TẠI (BAO GỒM CẢ PHIẾU HIỆN TẠI)
                        var examinationCurrentRecords = await this.examinationFormService.GetAsync(e => !e.Deleted && e.Active
                        && e.VaccineTypeId.HasValue
                        && e.VaccineTypeId == item.VaccineTypeId
                        && e.RecordId == item.RecordId
                        );
                        int totalInjections = 0;
                        if (examinationCurrentRecords != null && examinationCurrentRecords.Any())
                        {
                            // LẤY RA TẤT CẢ TIỂU SỬ KHÁM CỦA USER ĐỂ KIỂM TRA USER ĐÃ TIÊM HAY CHƯA
                            var examinationCurrentRecordIds = examinationCurrentRecords.Select(e => e.Id).ToList();
                            var medicalRecordDetailChecks = await this.medicalRecordDetailService.GetAsync(e => !e.Deleted && e.Active
                            && e.ExaminationFormId.HasValue
                            && e.VaccineTypeId == item.VaccineTypeId
                            //&& e.ServiceTypeId == item.ServiceTypeId
                            && examinationCurrentRecordIds.Contains(e.ExaminationFormId.Value));
                            if (medicalRecordDetailChecks != null && medicalRecordDetailChecks.Any())
                                totalInjections = medicalRecordDetailChecks.Count();
                        }

                        //if (item.NumberOfDoses.HasValue && item.NumberOfDoses.Value > 0 && totalInjections < item.NumberOfDoses.Value)
                        //{
                        //    item.TotalRemainInject = item.NumberOfDoses.Value - totalInjections;
                        //    // Lấy thông tin loại vaccine => tính toán ra ngày tiêm tiếp theo
                        //    var vaccineTypeInfo = await this.vaccineTypeService.GetByIdAsync(item.VaccineTypeId.Value);
                        //    if (vaccineTypeInfo != null && vaccineTypeInfo.DateTypeId.HasValue)
                        //    {
                        //        switch (vaccineTypeInfo.DateTypeId.Value)
                        //        {
                        //            // Ngày
                        //            case 0:
                        //                {
                        //                    item.NextInjectionDate = item.ExaminationDate.AddDays(vaccineTypeInfo.NumberOfDateTypeValue ?? 0);
                        //                }
                        //                break;
                        //            // Tuần
                        //            case 1:
                        //                {
                        //                    item.NextInjectionDate = item.ExaminationDate.AddDays((vaccineTypeInfo.NumberOfDateTypeValue ?? 0) * 7);
                        //                }
                        //                break;
                        //            // Tháng
                        //            case 2:
                        //                {
                        //                    item.NextInjectionDate = item.ExaminationDate.AddMonths(vaccineTypeInfo.NumberOfDateTypeValue ?? 0);
                        //                }
                        //                break;
                        //            // Năm
                        //            case 3:
                        //                {
                        //                    item.NextInjectionDate = item.ExaminationDate.AddYears(vaccineTypeInfo.NumberOfDateTypeValue ?? 0);
                        //                }
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //    }

                        //    if (item.NextInjectionDate.HasValue)
                        //    {
                        //        item.NextInjectionDateDisplay = string.Format("Tiêm mũi {0} ngày {1}", item.TotalCurrentInjections += 1, item.NextInjectionDate.Value.ToString("dd/MM/yyyy"));
                        //    }
                        //}
                        //else
                        //    item.NextInjectionDateDisplay = string.Format("Đã tiêm ngày {0}", item.ExaminationDate.ToString("dd/MM/yyyy"));
                    }
                }
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin random ca trực
        /// </summary>
        /// <param name="searchExaminationScheduleDetailAddition"></param>
        /// <returns></returns>
        [HttpGet("get-random-examination-schedule-detail")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetRandomExaminationScheduleDetailInfo([FromQuery] SearchExaminationScheduleDetailAddition searchExaminationScheduleDetailAddition)
        {
            int? roomExaminationId = null;
            int? examinationScheduleDetailId = null;
            int? doctorId = null;
            int? specialistTypeId = null;
            string roomExaminationName = string.Empty;
            string doctorName = string.Empty;
            string doctorDegreeTypeName = string.Empty;

            var scheduleInfos = await this.examinationScheduleService.GetAsync(e => !e.Deleted && e.ExaminationDate.Date == searchExaminationScheduleDetailAddition.ExaminationDate.Date 
            && e.SpecialistTypeId == searchExaminationScheduleDetailAddition.SpecialistTypeId
            && e.HospitalId == searchExaminationScheduleDetailAddition.HospitalId
            );
            if (scheduleInfos == null || !scheduleInfos.Any()) throw new AppException("Không tồn tại lịch khám với ngày khám và chuyên khoa này");
            var importScheduleIds = scheduleInfos.Where(e => e.ImportScheduleId.HasValue).Select(e => e.ImportScheduleId.Value).ToList();
            // LẤY THÔNG TIN CHI TIẾT CA TRỰC
            var examinationScheduleDetails = await this.examinationScheduleDetailService.GetAsync(e => e.ImportScheduleId.HasValue
            && importScheduleIds.Contains(e.ImportScheduleId.Value)
            && e.FromTime == searchExaminationScheduleDetailAddition.FromTime && e.ToTime == searchExaminationScheduleDetailAddition.ToTime);
            if (examinationScheduleDetails == null || !examinationScheduleDetails.Any()) throw new AppException("Không tồn tại chi tiết ca trực phù hợp");
            // TÍNH TOÁN LẤY RA CA TRỰC CÒN CÓ THỂ ĐĂNG KÍ ĐƯỢC
            List<ExaminationScheduleDetails> examinationScheduleDetailResults = new List<ExaminationScheduleDetails>();
            foreach (var examinationScheduleDetail in examinationScheduleDetails)
            {
                int totalUserExamination = await this.examinationFormService.CountAsync(e => !e.Deleted && e.Active
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.New
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.Canceled
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentFailed
                                && e.Status != (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed
                                && e.HospitalId == searchExaminationScheduleDetailAddition.HospitalId
                                && e.ExaminationScheduleDetailId == examinationScheduleDetail.Id
                                && (!searchExaminationScheduleDetailAddition.ExaminationFormId.HasValue || searchExaminationScheduleDetailAddition.ExaminationFormId.Value <= 0 || e.Id != searchExaminationScheduleDetailAddition.ExaminationFormId)
                                );
                if (examinationScheduleDetail.MaximumExamination.HasValue && totalUserExamination >= examinationScheduleDetail.MaximumExamination.Value)
                    continue;
                examinationScheduleDetailResults.Add(examinationScheduleDetail);
            }
            // RANDOM CA TRỰC CÓ THỂ ĐƯỢC ĐĂNG KÍ => LẤY RA THÔNG TIN BÁC SĨ/PHÒNG KHÁM/CHI TIẾT CA TRỰC
            Random random = new Random();
            ExaminationScheduleDetails randomDetail = null;
            if (examinationScheduleDetailResults != null && examinationScheduleDetailResults.Any())
            {
                int indexRandom = random.Next(examinationScheduleDetailResults.Count());
                randomDetail = examinationScheduleDetailResults[indexRandom];
                if (randomDetail != null)
                {
                    roomExaminationId = randomDetail.RoomExaminationId;
                    if (roomExaminationId.HasValue && roomExaminationId.Value > 0)
                    {
                        var roomExaminationFormInfo = await this.roomExaminationService.GetSingleAsync(e => !e.Deleted && e.Id == roomExaminationId.Value
                        && e.HospitalId == searchExaminationScheduleDetailAddition.HospitalId
                        );
                        if (roomExaminationFormInfo != null) roomExaminationName = roomExaminationFormInfo.Name;
                    }
                    examinationScheduleDetailId = randomDetail.Id;
                    specialistTypeId = searchExaminationScheduleDetailAddition.SpecialistTypeId;
                    // LẤY RA LỊCH ĐƯỢC RANDOM TỪ CA TRỰC => LẤY THÔNG TIN BÁC SĨ
                    var sheduleInfoRandomSelected = scheduleInfos.Where(e => e.ImportScheduleId == randomDetail.ImportScheduleId).FirstOrDefault();
                    if (sheduleInfoRandomSelected != null)
                    {
                        doctorId = sheduleInfoRandomSelected.DoctorId;
                        if (doctorId.HasValue && doctorId.Value > 0)
                        {
                            var doctorInfo = await this.doctorService.GetSingleAsync(e => e.Id == doctorId.Value 
                            && e.HospitalId == searchExaminationScheduleDetailAddition.HospitalId);
                            if (doctorInfo != null)
                            {
                                doctorName = doctorInfo.FirstName + " " + doctorInfo.LastName;
                                var degreeTypeInfo = await this.degreeTypeService.GetSingleAsync(e => e.Id == doctorInfo.DegreeId
                                );
                                if (degreeTypeInfo != null) doctorDegreeTypeName = degreeTypeInfo.Name;
                            }
                        }
                    }
                }
            }
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = new
                {
                    RoomExaminationId = roomExaminationId,
                    RoomExaminationName = roomExaminationName,
                    DoctorId = doctorId,
                    DoctorName = doctorName,
                    DoctorDegreeTypeName = doctorDegreeTypeName,
                    ExaminationScheduleDetailId = examinationScheduleDetailId,
                    SpecialistTypeId = specialistTypeId
                }
            };
        }

        #region FEE EXAMINATION (PHÍ KHÁM BỆNH)

        /// <summary>
        /// Lấy thông tin chi phí khám bệnh
        /// </summary>
        /// <param name="feeCaculateExaminationRequest"></param>
        /// <returns></returns>
        [HttpGet("get-caculate-fee-response")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetFeeResponseExamination([FromQuery] FeeCaculateExaminationRequest feeCaculateExaminationRequest)
        {
            if (feeCaculateExaminationRequest.HospitalId <= 0)
                throw new AppException("Vui lòng chọn bệnh viện!");
            var feeCaculateExaminationResponse = await this.hospitalConfigFeeService.GetFeeExamination(feeCaculateExaminationRequest);
            return new AppDomainResult()
            {
                Success = true,
                Data = feeCaculateExaminationResponse
            };
        }

        #endregion

        #region Catalogue

        /// <summary>
        /// Lấy danh sách phương thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-list-payment-method")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetPaymentMethods()
        {
            var paymentMethods = await this.paymentMethodService.GetAsync(e => !e.Deleted);
            return new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<PaymentMethodModel>>(paymentMethods)
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách ngân hàng theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-hospital-bank-info/{hospitalId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetHospitalBankInfos(int hospitalId)
        {
            if (hospitalId <= 0)
                throw new AppException("Vui lòng chọn bệnh viện!");
            var bankInfos = await this.bankInfoService.GetAsync(e => !e.Deleted && e.HospitalId == hospitalId);
            return new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<BankInfoModel>>(bankInfos)
            };
        }

        #endregion

        public const string MEDICAL_RECORD_FOLDER_NAME = "medicalrecord";
    }
}
