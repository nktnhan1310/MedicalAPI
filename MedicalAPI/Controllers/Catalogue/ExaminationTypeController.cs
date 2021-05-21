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
using Medical.Core.App.Controllers;

namespace MedicalAPI.Controllers
{
    [Route("api/examination-type")]
    [ApiController]
    [Description("Kênh đăng ký khám bệnh")]
    public class ExaminationTypeController : CatalogueController<ExaminationTypes, ExaminationTypeModel, BaseSearch>
    {
        public ExaminationTypeController(IServiceProvider serviceProvider, ILogger<BaseController<ExaminationTypes, ExaminationTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IExaminationTypeService>();
        }
    }
}
