using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Route("api/sms-email-template")]
    [ApiController]
    [Description("Chuyên khoa khám bệnh")]
    [Authorize]
    public class SMSEmailTemplateController : CatalogueCoreHospitalController<SMSEmailTemplates, SMSEmailTemplateModel, BaseHospitalSearch>
    {
        public SMSEmailTemplateController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<SMSEmailTemplates, SMSEmailTemplateModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ISMSEmailTemplateService>();
        }
    }
}
