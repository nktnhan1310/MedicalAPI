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
    [Route("api/system-advertisement")]
    [ApiController]
    [Description("Quản lý quảng cáo của hệ thống/bệnh viện")]
    [Authorize]
    public class SystemAdvertisementController : CoreHospitalController<SystemAdvertisements, SystemAdvertisementModel, BaseHospitalSearch>
    {
        public SystemAdvertisementController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<SystemAdvertisements, SystemAdvertisementModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<ISystemAdvertisementService>();
        }
    }
}
