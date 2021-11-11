using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using Medical.Interface;

namespace MedicalAPI.Controllers
{
    [Route("api/examination-form")]
    [ApiController]
    [Description("Quản lý phiếu khám bệnh")]
    [Authorize]
    public class ExaminationFormController : BaseController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>
    {
        private readonly IExaminationHistoryService examinationHistoryService;
        private readonly IPaymentHistoryService paymentHistoryService;
        private readonly IExaminationFormService examinationFormService;
        private readonly IMedicalRecordService medicalRecordService;
        private readonly IMedicalRecordDetailService medicalRecordDetailService;
        private readonly IExaminationScheduleService examinationScheduleService;
        private readonly IExaminationScheduleDetailService examinationScheduleDetailService;
        private readonly ISpecialListTypeService specialListTypeService;
        private readonly IPaymentMethodService paymentMethodService;
        private readonly IBankInfoService bankInfoService;
        private readonly IHospitalConfigFeeService hospitalConfigFeeService;
        private readonly IExaminationFormDetailService examinationFormDetailService;
        private readonly IMomoConfigurationService momoConfigurationService;
        private readonly IMomoPaymentService momoPaymentService;
        private readonly IDoctorService doctorService;
        private readonly IDoctorDetailService doctorDetailService;
        private readonly INotificationService notificationService;
        private readonly IHospitalFileService hospitalFileService;
        private readonly IVaccineTypeService vaccineTypeService;
        private readonly IExaminationFormAdditionServiceMappingService examinationFormAdditionServiceMappingService;
        private readonly IAdditionServiceType additionServiceType;
        private readonly IRoomExaminationService roomExaminationService;
        private readonly IDegreeTypeService degreeTypeService;

        private IHubContext<NotificationHub> hubContext;
        private IHubContext<NotificationAppHub> appHubContext;


        private IConfiguration configuration;
        public ExaminationFormController(IServiceProvider serviceProvider, ILogger<BaseController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>> logger
            , IWebHostEnvironment env
            , IHubContext<NotificationHub> hubContext
            , IHubContext<NotificationAppHub> appHubContext
            , IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.configuration = configuration;
            this.domainService = serviceProvider.GetRequiredService<IExaminationFormService>();
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
            examinationFormDetailService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
            momoPaymentService = serviceProvider.GetRequiredService<IMomoPaymentService>();
            doctorService = serviceProvider.GetRequiredService<IDoctorService>();
            doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
            notificationService = serviceProvider.GetRequiredService<INotificationService>();
            medicalRecordDetailService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            hospitalFileService = serviceProvider.GetRequiredService<IHospitalFileService>();
            vaccineTypeService = serviceProvider.GetRequiredService<IVaccineTypeService>();
            examinationFormAdditionServiceMappingService = serviceProvider.GetRequiredService<IExaminationFormAdditionServiceMappingService>();
            additionServiceType = serviceProvider.GetRequiredService<IAdditionServiceType>();
            roomExaminationService = serviceProvider.GetRequiredService<IRoomExaminationService>();
            degreeTypeService = serviceProvider.GetRequiredService<IDegreeTypeService>();

            this.hubContext = hubContext;
            this.appHubContext = appHubContext;
        }

        /// <summary>
        /// Lấy thông tin danh sách bác sĩ theo bệnh viện
        /// </summary>
        /// <param name="specialistTypeId"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpGet("get-list-doctor-by-hospital/specialistTypeId")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetDoctorByHospital(int? specialistTypeId, int? typeId)
        {
            var doctors = await this.doctorService.GetAsync(e => !e.Deleted && e.Active
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
            && (!typeId.HasValue || (typeId.Value == 0 && e.TypeId == 0) || e.TypeId != 0)
            );
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
                        && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
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
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                searchExaminationScheduleDetailV2.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
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
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                searchExaminationScheduleDetailV2.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
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
                    foreach (var examinationFormServiceMapping in itemModel.ExaminationFormServiceMappings)
                    {
                        var additionServiceTypeInfo = additionServiceTypeInfos.Where(e => e.Id == examinationFormServiceMapping.AdditionServiceId).FirstOrDefault();
                        if (additionServiceTypeInfo != null) examinationFormServiceMapping.AdditionServiceName = additionServiceTypeInfo.Name;
                    }
                }
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
        /// Lấy thông tin danh sách ca khám theo bệnh viện và chuyên khoa được chọn
        /// </summary>
        /// <param name="searchExaminationScheduleForm"></param>
        /// <returns></returns>
        [HttpGet("get-config-time-examination-by-specialist-type")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetConfigTimeExaminationBySpecialistType([FromQuery] SearchExaminationScheduleForm searchExaminationScheduleForm)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                searchExaminationScheduleForm.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
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
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
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
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
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
        /// Lấy danh sách hồ sơ bệnh án 
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-medical-record")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetMedicalRecordCatalogue()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            //if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
            //    hospitalId = LoginContext.Instance.CurrentUser.HospitalId;
            var medicalRecords = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active
            //&& (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            ,
                e => new MedicalRecords()
                {
                    Id = e.Id,
                    Code = e.Code,
                    UserFullName = e.UserFullName,
                    Address = e.Address,
                    Email = e.Email,
                    Phone = e.Phone,
                    CertificateNo = e.CertificateNo,
                    UserId = e.UserId,
                });
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<MedicalRecordModel>>(medicalRecords)
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin lịch sử phiếu khám (lịch hen)
        /// </summary>
        /// <param name="searchExaminationFormHistory"></param>
        /// <returns></returns>
        [HttpGet("get-examination-form-history/{examinationFormId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetExaminationHistory([FromQuery] SearchExaminationFormHistory searchExaminationFormHistory)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            PagedList<ExaminationHistoryModel> pagedListModel = new PagedList<ExaminationHistoryModel>();

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
            string paymentCode = string.Empty;
            if (ModelState.IsValid)
            {
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();

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
                            paymentCode = paymentMethodInfo.Code;
                        }
                    }
                }
                // THANH TOÁN QUA APP HOẶC COD => Cập nhật trạng thái phiếu => Chờ admin xác nhận
                if (updateExaminationStatusModel.MedicalRecordDetailFiles != null && updateExaminationStatusModel.MedicalRecordDetailFiles.Any())
                {
                    foreach (var file in updateExaminationStatusModel.MedicalRecordDetailFiles)
                    {
                        // ------- START GET URL FOR FILE
                        string folderUploadPath = string.Empty;
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                        string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);

                        string folderUplocadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                        string fileUploadPath = Path.Combine(folderUplocadUrl, Path.GetFileName(filePath));
                        // Kiểm tra có tồn tại file trong temp chưa?
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                        {

                            FileUtils.CreateDirectory(folderUplocadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                            folderUploadPaths.Add(fileUploadPath);
                            string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
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
                bool success = await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus, true);
                if (success)
                {

                    var examinationFormInfo = await this.examinationFormService.GetByIdAsync(updateExaminationStatusModel.ExaminationFormId);
                    if (examinationFormInfo != null)
                    {
                        var medicalRecordInfos = await medicalRecordService.GetAsync(e => e.Id == examinationFormInfo.RecordId, e => new MedicalRecords()
                        {
                            UserId = e.UserId
                        });
                        switch (updateExaminationStatus.Status)
                        {
                            // Đã xác nhận => gửi thông báo cho bác sĩ + user
                            case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                                {
                                    if (medicalRecordInfos != null && medicalRecordInfos.Any())
                                    {
                                        await notificationService.CreateCustomNotificationUser(null, LoginContext.Instance.CurrentUser.HospitalId
                                            , new List<int>() { medicalRecordInfos.FirstOrDefault().UserId }
                                            , string.Format("/medical/examination/{0}", examinationFormInfo.Id)
                                            , string.Empty
                                            , LoginContext.Instance.CurrentUser.UserName
                                            , examinationFormInfo.Id
                                            , false
                                            , "USER"
                                            , CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_CREATE
                                            );
                                        await appHubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                                    }

                                    if (examinationFormInfo.DoctorId.HasValue && examinationFormInfo.DoctorId.Value > 0)
                                    {
                                        var doctorInfo = await this.doctorService.GetByIdAsync(examinationFormInfo.DoctorId.Value);
                                        if (doctorInfo != null && doctorInfo.UserId.HasValue && doctorInfo.UserId > 0)
                                        {
                                            await notificationService.CreateCustomNotificationUser(null, LoginContext.Instance.CurrentUser.HospitalId
                                            , new List<int>() { doctorInfo.UserId.Value }
                                            , string.Format("/medical/examination/{0}", examinationFormInfo.Id)
                                            , string.Empty
                                            , LoginContext.Instance.CurrentUser.UserName
                                            , examinationFormInfo.Id
                                            , false
                                            , "USER"
                                            , CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_DOCTOR_CREATE
                                            );
                                            await hubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                                        }
                                    }
                                }
                                break;
                            // Chờ xác nhận tái khám => thông báo cho user
                            case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                                {
                                    if (medicalRecordInfos != null && medicalRecordInfos.Any())
                                    {
                                        await notificationService.CreateCustomNotificationUser(null, LoginContext.Instance.CurrentUser.HospitalId
                                            , new List<int>() { medicalRecordInfos.FirstOrDefault().UserId }
                                            , string.Format("/medical/examination/{0}", examinationFormInfo.Id)
                                            , string.Empty
                                            , LoginContext.Instance.CurrentUser.UserName
                                            , examinationFormInfo.Id
                                            , false
                                            , "USER"
                                            , CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_USER_UPDATE
                                            );
                                        await appHubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                                    }
                                }
                                break;
                            case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                                {
                                    if (examinationFormInfo.DoctorId.HasValue && examinationFormInfo.DoctorId.Value > 0)
                                    {
                                        var doctorInfo = await this.doctorService.GetByIdAsync(examinationFormInfo.DoctorId.Value);
                                        if (doctorInfo != null && doctorInfo.UserId.HasValue && doctorInfo.UserId > 0)
                                        {
                                            await notificationService.CreateCustomNotificationUser(null, LoginContext.Instance.CurrentUser.HospitalId
                                            , new List<int>() { doctorInfo.UserId.Value }
                                            , string.Format("/medical/examination/{0}", examinationFormInfo.Id)
                                            , string.Empty
                                            , LoginContext.Instance.CurrentUser.UserName
                                            , examinationFormInfo.Id
                                            , false
                                            , "USER"
                                            , CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_DOCTOR_UPDATE
                                            );
                                            await hubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                    }


                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    // Remove file trong thư mục temp
                    if (filePaths.Any())
                    {
                        foreach (var filePath in filePaths)
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
                else
                {
                    if (folderUploadPaths.Any())
                    {
                        foreach (var folderUploadPath in folderUploadPaths)
                        {
                            System.IO.File.Delete(folderUploadPath);
                        }
                    }
                }

                appDomainResult.Success = success;
                appDomainResult.Data = momoResponseModel;
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
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
                string amount = updateExaminationStatusModel.TotalPrice.HasValue ? (updateExaminationStatusModel.TotalPrice.Value.ToString()) : "0";
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
        /// Thêm mới phiếu khám
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override Task<AppDomainResult> AddItem([FromBody] ExaminationFormModel itemModel)
        {
            return base.AddItem(itemModel);
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> GetPagedData([FromQuery] SearchExaminationForm baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
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

                    if (item.NumberOfDoses.HasValue && item.NumberOfDoses.Value > 0 && totalInjections < item.NumberOfDoses.Value)
                    {
                        item.TotalRemainInject = item.NumberOfDoses.Value - totalInjections;
                        // Lấy thông tin loại vaccine => tính toán ra ngày tiêm tiếp theo
                        var vaccineTypeInfo = await this.vaccineTypeService.GetByIdAsync(item.VaccineTypeId.Value);
                        if (vaccineTypeInfo != null && vaccineTypeInfo.DateTypeId.HasValue)
                        {
                            switch (vaccineTypeInfo.DateTypeId.Value)
                            {
                                // Ngày
                                case 0:
                                    {
                                        item.NextInjectionDate = item.ExaminationDate.AddDays(vaccineTypeInfo.NumberOfDateTypeValue ?? 0);
                                    }
                                    break;
                                // Tuần
                                case 1:
                                    {
                                        item.NextInjectionDate = item.ExaminationDate.AddDays((vaccineTypeInfo.NumberOfDateTypeValue ?? 0) * 7);
                                    }
                                    break;
                                // Tháng
                                case 2:
                                    {
                                        item.NextInjectionDate = item.ExaminationDate.AddMonths(vaccineTypeInfo.NumberOfDateTypeValue ?? 0);
                                    }
                                    break;
                                // Năm
                                case 3:
                                    {
                                        item.NextInjectionDate = item.ExaminationDate.AddYears(vaccineTypeInfo.NumberOfDateTypeValue ?? 0);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (item.NextInjectionDate.HasValue)
                        {
                            item.NextInjectionDateDisplay = string.Format("Tiêm mũi {0} ngày {1}", item.TotalCurrentInjections += 1, item.NextInjectionDate.Value.ToString("dd/MM/yyyy"));
                        }
                    }
                    else
                        item.NextInjectionDateDisplay = string.Format("Đã tiêm ngày {0}", item.ExaminationDate.ToString("dd/MM/yyyy"));
                }
            }
            appDomainResult = new AppDomainResult
            {
                Data = pagedDataModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };


            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin phiếu khám theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
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
            var pagedItems = await this.domainService.GetPagedListData(searchExaminationForm);
            if (pagedItems != null && pagedItems.Items.Any())
            {
                item = pagedItems.Items.FirstOrDefault();
            }
            //var item = await this.domainService.GetByIdAsync(id);
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
                    foreach (var examinationFormServiceMapping in itemModel.ExaminationFormServiceMappings)
                    {
                        var additionServiceTypeInfo = additionServiceTypeInfos.Where(e => e.Id == examinationFormServiceMapping.AdditionServiceId).FirstOrDefault();
                        if (additionServiceTypeInfo != null) examinationFormServiceMapping.AdditionServiceName = additionServiceTypeInfo.Name;
                    }
                }

                //// Lây thông tin lịch sử
                //var examinationHistories = await this.examinationHistoryService.GetAsync(e => !e.Deleted && e.ExaminationFormId == item.Id);
                //if(examinationHistories != null && examinationHistories.Any())
                //    itemModel.ExaminationHistories = mapper.Map<IList<ExaminationHistoryModel>>(examinationHistories.OrderByDescending(e => e.Id));
                //// Lây thông tin lịch sử thanh toán
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
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                feeCaculateExaminationRequest.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
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
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var bankInfos = await this.bankInfoService.GetAsync(e => !e.Deleted && e.HospitalId == hospitalId);
            return new AppDomainResult()
            {
                Success = true,
                Data = mapper.Map<IList<BankInfoModel>>(bankInfos)
            };
        }

        #endregion

    }
}
