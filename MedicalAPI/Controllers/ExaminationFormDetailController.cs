using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    [Route("api/examination-form-detail")]
    [ApiController]
    [Authorize]
    [Description("Quản lý dịch vụ phát sinh khám bệnh")]
    public class ExaminationFormDetailController : CoreHospitalController<ExaminationFormDetails, ExaminationFormDetailModel, SearchExaminationFormDetail>
    {
        private readonly IExaminationFormDetailService examinationFormDetailService;
        public ExaminationFormDetailController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<ExaminationFormDetails, ExaminationFormDetailModel, SearchExaminationFormDetail>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            examinationFormDetailService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán cho dịch vụ phát sinh
        /// </summary>
        /// <returns></returns>
        [HttpPost("update-examination-form-detail-status")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateExaminationFormDetailStatus([FromBody] UpdateExaminationFormDetailStatus updateExaminationFormDetailStatus)
        {
            updateExaminationFormDetailStatus.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            bool success = await this.examinationFormDetailService.UpdateExaminationFormDetailStatus(updateExaminationFormDetailStatus);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
