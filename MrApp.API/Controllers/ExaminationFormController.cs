using AutoMapper;
using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Extensions;
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

        public ExaminationFormController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
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
        }

        /// <summary>
        /// Lấy thông tin danh sách bác sĩ theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="specialistTypeId"></param>
        /// <returns></returns>
        [HttpGet("get-list-doctor-by-hospital/{hospitalId}/specialistTypeId")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetDoctorByHospital(int hospitalId, int? specialistTypeId)
        {
            if (hospitalId <= 0) throw new AppException("Vui lòng chọn bệnh viện");
            var doctors = await this.doctorService.GetAsync(e => !e.Deleted && e.Active && e.HospitalId == hospitalId);
            if (doctors != null && doctors.Any())
            {
                if (specialistTypeId.HasValue && specialistTypeId.Value > 0)
                {
                    var doctorIds = doctors.Select(e => e.Id).ToList();
                    var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted
                    && doctorIds.Contains(e.DoctorId)
                    && e.SpecialistTypeId == specialistTypeId.Value);
                    var doctorIdsBySpecialistTypes = doctorDetails.Select(e => e.DoctorId).ToList();
                    if (doctorIdsBySpecialistTypes != null && doctorIdsBySpecialistTypes.Any())
                        doctors = await this.doctorService.GetAsync(e => !e.Deleted
                        && e.Active
                        && e.HospitalId == hospitalId
                        && doctorIdsBySpecialistTypes.Contains(e.Id));
                    else doctors = null;
                }
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
        /// <param name="examinationFormId"></param>
        /// <returns></returns>
        [HttpGet("get-examination-form-history/{examinationFormId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetExaminationHistory(int examinationFormId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var examinationForms = await this.examinationHistoryService.GetAsync(x => !x.Deleted && x.ExaminationFormId == examinationFormId);
            var examinationFormModels = mapper.Map<IList<ExaminationHistoryModel>>(examinationForms);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = examinationFormModels
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
            if (ModelState.IsValid)
            {
                // KIỂM TRA TRẠNG THÁI HIỆN TẠI VỚI TRẠNG THÁI CẬP NHẬT PHIẾU
                string checkMessage = await this.examinationFormService.GetCheckStatusMessage(updateExaminationStatusModel.ExaminationFormId, updateExaminationStatusModel.Status ?? 0);
                if (!string.IsNullOrEmpty(checkMessage)) throw new AppException(checkMessage);

                // KIỂM TRA CÓ THANH TOÁN HAY KHÔNG?
                if (updateExaminationStatusModel.PaymentMethodId.HasValue && updateExaminationStatusModel.PaymentMethodId.Value > 0)
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
                Data = momoResponseModel,
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
            MomoConfigurations momoConfiguration = new MomoConfigurations();
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            var momoConfigurationInfos = await this.momoConfigurationService.GetAsync(e => !e.Deleted && e.Active);
            if (momoConfigurationInfos != null && momoConfigurationInfos.Any())
            {
                momoConfiguration = momoConfigurationInfos.FirstOrDefault();
                string orderInfo = "test";
                string amount = updateExaminationStatusModel.TotalPrice.HasValue ? updateExaminationStatusModel.TotalPrice.Value.ToString() : "0";
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
                        Amount = amount,
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
                itemModel.Active = true;
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
                    throw new AppException("Item không tồn tại");
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
                        if(hospitalFiles != null && hospitalFiles.Any())
                        {
                            item.HospitalFiles = mapper.Map<IList<HospitalFileModel>>(hospitalFiles);
                        }
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
