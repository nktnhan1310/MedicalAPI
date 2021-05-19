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
    [Description("Học hàm/Học vị")]
    public class DegreeTypeController : CatalogueController<DegreeTypes, DegreeTypeModel, BaseSearch>
    {
        public DegreeTypeController(IServiceProvider serviceProvider, ILogger<BaseController<DegreeTypes, DegreeTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IDegreeTypeService>();
        }
    }
}
