using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using MedicalAPI.Controllers.BaseHospital;
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

namespace MedicalAPI.Controllers.Catalogue
{
    [Route("api/notification-type")]
    [ApiController]
    [Description("Loại thông báo")]
    public class NotificationTypeController : CatalogueController<NotificationTypes, NotificationTypeModel, BaseSearch>
    {
        public NotificationTypeController(IServiceProvider serviceProvider, ILogger<BaseController<NotificationTypes, NotificationTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<INotificationTypeService>();
        }
    }
}
