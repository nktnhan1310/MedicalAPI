using AutoMapper;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/system-configuration")]
    [ApiController]
    [Description("Bảng cấu hình hệ thống")]
    [Authorize]
    public class SystemConfigurationController : BaseController
    {
        private readonly ISystemConfigurationService systemconfigurationservice;
        public SystemConfigurationController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            systemconfigurationservice = serviceProvider.GetRequiredService<ISystemConfigurationService>();
        }

        /// <summary>
        /// Lấy thông tin cấu hình hệ thông theo code
        /// </summary>
        /// <param name="configurationCode"></param>
        /// <returns></returns>
        [HttpGet("get-configuration/{configurationCode}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetConfiguration(string configurationCode)
        {
            SystemConfiguartionModel systemConfiguartionModel = null;
            var configurationInfos = await this.systemconfigurationservice.GetAsync(e => !e.Deleted && e.Active && e.Code == configurationCode);
            if (configurationInfos != null && configurationInfos.Any())
                systemConfiguartionModel = mapper.Map<SystemConfiguartionModel>(configurationInfos.FirstOrDefault());
            return new AppDomainResult()
            {
                Data = systemConfiguartionModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
