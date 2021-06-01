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

namespace MedicalAPI.Controllers
{
    [Route("api/examination-form")]
    [ApiController]
    [Description("Quản lý phiếu khám bệnh")]
    [Authorize]
    public class ExaminationFormController : CoreHospitalController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>
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
        public ExaminationFormController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
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
        }
        
        /// <summary>
        /// Lấy tất cả thông tin chuyên khoa khám bệnh
        /// </summary>
        /// <param name="searchExaminationScheduleDetailV2"></param>
        /// <returns></returns>
        [HttpGet("get-all-examination-schedules")]
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
        /// Lấy thông tin danh sách ca khám theo bệnh viện và chuyên khoa được chọn
        /// </summary>
        /// <param name="searchExaminationScheduleForm"></param>
        /// <returns></returns>
        [HttpGet("get-config-time-examination-by-specialist-type")]
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
        /// Lấy danh sách hồ sơ bệnh án theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-medical-record-by-hospital/hospitalId")]
        public async Task<AppDomainResult> GetMedicalRecordByHospital(int? hospitalId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hospitalId = LoginContext.Instance.CurrentUser.HospitalId;
            var medicalRecords = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value),
                e => new MedicalRecords()
                {
                    Id = e.Id,
                    Code = e.Code,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Address = e.Address,
                    Email = e.Email,
                    Phone = e.Phone,
                    CertificateNo = e.CertificateNo,
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
        /// Cập nhật trạng thái phiếu khám sức khỏe
        /// </summary>
        /// <param name="updateExaminationStatusModel"></param>
        /// <returns></returns>
        [HttpPost("update-examination-status")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateExaminationStatus([FromBody] UpdateExaminationStatusModel updateExaminationStatusModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                var updateExaminationStatus = mapper.Map<UpdateExaminationStatus>(updateExaminationStatusModel);
                bool isSuccess = await this.examinationFormService.UpdateExaminationStatus(updateExaminationStatus);
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

            return appDomainResult;
        }

        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override Task<AppDomainResult> AddItem([FromBody] ExaminationFormModel itemModel)
        {
            return base.AddItem(itemModel);
        }

        #region FEE EXAMINATION (PHÍ KHÁM BỆNH)

        /// <summary>
        /// Lấy thông tin chi phí khám bệnh
        /// </summary>
        /// <param name="feeCaculateExaminationRequest"></param>
        /// <returns></returns>
        [HttpGet("get-caculate-fee-response")]
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
