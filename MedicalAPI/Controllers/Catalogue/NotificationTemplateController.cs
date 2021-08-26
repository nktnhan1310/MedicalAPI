using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
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
    [Route("api/notification-template")]
    [ApiController]
    [Description("Mẫu notifycation")]
    public class NotificationTemplateController : CatalogueCoreHospitalController<NotificationTemplates, NotificationTemplateModel, BaseHospitalSearch>
    {
        public NotificationTemplateController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<NotificationTemplates, NotificationTemplateModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<INotificationTemplateService>();
        }
    }
}
