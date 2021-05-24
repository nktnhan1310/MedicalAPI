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
using Medical.Core.App.Controllers;

namespace MedicalAPI.Controllers
{
    [Route("api/config-time-examination")]
    [ApiController]
    [Description("Cấu hình ca khám")]
    public class ConfigTimeExaminationController : CatalogueController<ConfigTimeExaminations, ConfigTimeExamniationModel, BaseSearch>
    {
        public ConfigTimeExaminationController(IServiceProvider serviceProvider, Microsoft.Extensions.Logging.ILogger<CatalogueController<ConfigTimeExaminations, ConfigTimeExamniationModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IConfigTimeExaminationService>();
        }
    }
}
