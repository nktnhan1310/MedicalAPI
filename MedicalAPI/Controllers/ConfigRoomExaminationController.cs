using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Quản lý cấu hình số lượng bệnh nhân khám theo phòng")]
    [Authorize]
    public class ConfigRoomExaminationController : BaseController<ConfigRoomExaminations, ConfigRoomExaminationModel, SearchConfigRoomExamination>
    {
        public ConfigRoomExaminationController(IServiceProvider serviceProvider, ILogger<BaseController<ConfigRoomExaminations, ConfigRoomExaminationModel, SearchConfigRoomExamination>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IConfigRoomExaminationService>();
        }
    }
}
