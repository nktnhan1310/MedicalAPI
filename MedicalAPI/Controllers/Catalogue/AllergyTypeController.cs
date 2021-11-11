using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface;
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
    [Route("api/allergy-type")]
    [ApiController]
    [Description("Quản lý loại dị ứng")]
    [Authorize]
    public class AllergyTypeController : CatalogueController<AllergyTypes, AllergyTypeModel, BaseSearch>
    {
        public AllergyTypeController(IServiceProvider serviceProvider, ILogger<BaseController<AllergyTypes, AllergyTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IAllergyTypeService>();
        }
    }
}
