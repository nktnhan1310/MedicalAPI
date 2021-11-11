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
    [Route("api/hospital-holiday-config")]
    [ApiController]
    [Description("Cấu hình ngày nghỉ theo từng bệnh viện")]
    [Authorize]
    public class HospitalHolidayConfigController : CoreHospitalController<HospitalHolidayConfigs, HospitalHolidayConfigModel, BaseHospitalSearch>
    {
        public HospitalHolidayConfigController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<HospitalHolidayConfigs, HospitalHolidayConfigModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IHospitalHolidayConfigService>();
        }
    }
}
