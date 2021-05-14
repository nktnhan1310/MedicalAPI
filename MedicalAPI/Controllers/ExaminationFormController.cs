﻿using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Interface.Services;
using MedicalAPI.Model;
using MedicalAPI.Utils;
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

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Quản lý phiếu khám bệnh")]
    public class ExaminationFormController : BaseController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>
    {
        private readonly IExaminationHistoryService examinationHistoryService;
        private readonly IPaymentHistoryService paymentHistoryService;
        private readonly IExaminationFormService examinationFormService;


        public ExaminationFormController(IServiceProvider serviceProvider, ILogger<BaseController<ExaminationForms, ExaminationFormModel, SearchExaminationForm>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IExaminationFormService>();
            paymentHistoryService = serviceProvider.GetRequiredService<IPaymentHistoryService>();
            examinationHistoryService = serviceProvider.GetRequiredService<IExaminationHistoryService>();
            examinationFormService = serviceProvider.GetRequiredService<IExaminationFormService>();
        }

        /// <summary>
        /// Lấy thông tin lịch sử phiếu khám (lịch hen)
        /// </summary>
        /// <param name="examinationFormId"></param>
        /// <returns></returns>
        [HttpGet("{examinationFormId}")]
        public async Task<AppDomainResult> GetExaminationHistory(int examinationFormId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var examinationForms = await this.examinationHistoryService.GetAsync(x => !x.Deleted && x.ExaminationFormId == examinationFormId);
                var examinationFormModels = mapper.Map<IList<ExaminationHistoryModel>>(examinationForms);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK,
                    Data = examinationFormModels
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "GetExaminationHistory", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin lịch sử thanh toán
        /// </summary>
        /// <param name="examinationFormId"></param>
        /// <returns></returns>
        [HttpGet("{examinationFormId}")]
        public async Task<AppDomainResult> GetPaymentHistory(int examinationFormId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var paymentHistories = await this.paymentHistoryService.GetAsync(x => !x.Deleted && x.ExaminationFormId == examinationFormId);
                var paymentHistoryModels = mapper.Map<IList<PaymentHistoryModel>>(paymentHistories);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK,
                    Data = paymentHistoryModels
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "GetExaminationHistory", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu khám sức khỏe
        /// </summary>
        /// <param name="updateExaminationStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AppDomainResult> UpdateExaminationStatus([FromBody] UpdateExaminationStatusModel updateExaminationStatusModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
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
                    {
                        appDomainResult.Success = false;
                        appDomainResult.ResultMessage = ModelState.GetErrorMessage();
                        appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
                else
                {
                    appDomainResult.Success = false;
                    appDomainResult.ResultMessage = ModelState.GetErrorMessage();
                    appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "AddItem", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

    }
}
