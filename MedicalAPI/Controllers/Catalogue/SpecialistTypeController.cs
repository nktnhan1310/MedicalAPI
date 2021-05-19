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
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Chuyên khoa khám bệnh")]
    public class SpecialistTypeController : CatalogueController<SpecialistTypes, SpecialistTypeModel, SearchSpecialListType>
    {
        public SpecialistTypeController(IServiceProvider serviceProvider, ILogger<BaseController<SpecialistTypes, SpecialistTypeModel, SearchSpecialListType>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ISpecialListTypeService>();
        }
    }
}
