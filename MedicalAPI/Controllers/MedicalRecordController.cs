using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAPI.Controllers
{
    [Route("api/medical-record")]
    [ApiController]
    [Description("Quản lý hồ sơ khám bệnh")]
    [Authorize]
    public class MedicalRecordController : BaseController<MedicalRecords, MedicalRecordModel, SearchMedicalRecord>
    {
        private readonly IMedicalRecordAdditionService medicalRecordAdditionService;
        public MedicalRecordController(IServiceProvider serviceProvider, ILogger<BaseController<MedicalRecords, MedicalRecordModel, SearchMedicalRecord>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            medicalRecordAdditionService = serviceProvider.GetRequiredService<IMedicalRecordAdditionService>();
        }

        /// <summary>
        /// Lấy thông tin thông tin người thân của hồ sơ
        /// </summary>
        /// <param name="medicalRecordId"></param>
        /// <returns></returns>
        [HttpGet("get-medical-addition-info/{medicalRecordId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetMedicalAdditionInfos(int medicalRecordId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var medicalRecordAdditionInfos = await this.medicalRecordAdditionService.GetAsync(e => !e.Deleted && e.MedicalRecordId == medicalRecordId);
            var medicalRecordAdditionInfoModels = mapper.Map<IList<MedicalRecordAdditionModel>>(medicalRecordAdditionInfos);
            appDomainResult = new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = true,
                Data = medicalRecordAdditionInfoModels,
            };
            return appDomainResult;
        }


    }
}
