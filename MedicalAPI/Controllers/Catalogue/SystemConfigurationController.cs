using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
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
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/system-configuration")]
    [ApiController]
    [Description("Bảng cấu hình hệ thống")]
    [Authorize]
    public class SystemConfigurationController : CatalogueController<SystemConfiguartions, SystemConfiguartionModel, BaseSearch>
    {
        public SystemConfigurationController(IServiceProvider serviceProvider, ILogger<BaseController<SystemConfiguartions, SystemConfiguartionModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ISystemConfigurationService>();
        }

    }
}
