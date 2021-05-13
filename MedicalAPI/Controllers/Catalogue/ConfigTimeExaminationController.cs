﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using MedicalAPI.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Cấu hình ca khám")]
    public class ConfigTimeExaminationController : CatalogueController<ConfigTimeExaminations, ConfigTimeExamniationModel, BaseSearch>
    {
        public ConfigTimeExaminationController(IServiceProvider serviceProvider, Microsoft.Extensions.Logging.ILogger<BaseController<ConfigTimeExaminations, ConfigTimeExamniationModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IConfigTimeExaminationService>();
        }
    }
}
