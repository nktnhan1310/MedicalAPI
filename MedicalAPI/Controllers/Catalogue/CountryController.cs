﻿using Medical.Core.App.Controllers;
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
    [Route("api/country")]
    [ApiController]
    [Description("Quản lý quốc gia")]
    public class CountryController : CatalogueController<Countries, CountryModel, BaseSearch>
    {
        public CountryController(IServiceProvider serviceProvider, ILogger<BaseController<Countries, CountryModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ICountryService>();
        }
    }
}
