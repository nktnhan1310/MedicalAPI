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

namespace MedicalAPI.Controllers.Catalogue
{
    [Route("api/addition-service")]
    [ApiController]
    [Description("Dịch vụ phát sinh")]
    [Authorize]
    public class AdditionServiceController : CatalogueCoreHospitalController<AdditionServices, AdditionServiceModel, BaseHospitalSearch>
    {
        public AdditionServiceController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<AdditionServices, AdditionServiceModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IAdditionServiceType>();
        }
    }
}
