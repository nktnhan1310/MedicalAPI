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

namespace MedicalAPI.Controllers
{
    [Route("api/service-type")]
    [ApiController]
    [Description("Dịch vụ khám bệnh")]
    public class ServiceTypeController : CatalogueController<ServiceTypes, ServiceTypeModel, BaseSearch>
    {
        public ServiceTypeController(IServiceProvider serviceProvider, ILogger<CatalogueController<ServiceTypes, ServiceTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IServiceTypeService>();
        }
    }
}
