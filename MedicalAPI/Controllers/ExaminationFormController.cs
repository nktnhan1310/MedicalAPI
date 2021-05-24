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
    public class ExaminationFormController : BaseController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>
    {
        private readonly IExaminationHistoryService examinationHistoryService;
        private readonly IPaymentHistoryService paymentHistoryService;
        private readonly IExaminationFormService examinationFormService;
        private readonly IMedicalRecordService medicalRecordService;
        private readonly IExaminationScheduleService examinationScheduleService;
        private readonly IExaminationScheduleDetailService examinationScheduleDetailService;
        private readonly ISpecialListTypeService specialListTypeService;
        public ExaminationFormController(IServiceProvider serviceProvider, ILogger<BaseController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IExaminationFormService>();
            paymentHistoryService = serviceProvider.GetRequiredService<IPaymentHistoryService>();
            examinationHistoryService = serviceProvider.GetRequiredService<IExaminationHistoryService>();
            examinationFormService = serviceProvider.GetRequiredService<IExaminationFormService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            examinationScheduleService = serviceProvider.GetRequiredService<IExaminationScheduleService>();
            examinationScheduleDetailService = serviceProvider.GetRequiredService<IExaminationScheduleDetailService>();
            specialListTypeService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
        }

        /// <summary>
        /// Lấy thông tin danh sách ca khám theo bệnh viện và chuyên khoa được chọn
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="specialistTypeId"></param>
        /// <returns></returns>
        [HttpGet("get-config-time-examination-by-specialist-type/{hospitalId}/{specialistTypeId}")]
        public async Task<AppDomainResult> GetConfigTimeExaminationBySpecialistType(int hospitalId, int specialistTypeId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var scheduleSelecteds = await this.examinationScheduleService.GetAsync(e => !e.Deleted && e.Active && e.HospitalId == hospitalId && e.SpecialistTypeId == specialistTypeId);
            if (scheduleSelecteds != null && scheduleSelecteds.Any())
            {
                var scheduleIds = scheduleSelecteds.Select(e => e.Id).ToList();
                var scheduleDetails = await this.examinationScheduleDetailService.GetAsync(e => !e.Deleted && e.Active && scheduleIds.Contains(e.ScheduleId));

                var scheduleDetailModels = mapper.Map<IList<ExaminationScheduleDetailModel>>(scheduleDetails);


                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = scheduleDetailModels
                };

            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách ngày khám theo bệnh viện
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpGet("get-list-examination-date-by-hospital/{hospitalId}")]
        public async Task<AppDomainResult> GetListDateExaminationByHospital(int hospitalId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var schedules = await this.examinationScheduleService.GetAsync(e => !e.Deleted && e.Active
            && e.HospitalId == hospitalId
            && e.ExaminationDate.Date >= DateTime.Now.Date
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

    }
}
