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
    [Route("api/cities")]
    [ApiController]
    [Description("Quản lý thành phố")]
    [Authorize]
    public class CityController : CatalogueController<Cities, CityModel, SearchBaseLocation>
    {
        public CityController(IServiceProvider serviceProvider, ILogger<CatalogueController<Cities, CityModel, SearchBaseLocation>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ICityService>();
        }
    }
}
