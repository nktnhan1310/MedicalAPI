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
    [Route("api/diagnotic-type")]
    [ApiController]
    [Description("Quản lý danh mục chuẩn đoán")]
    [Authorize]
    public class DiagnoticTypeController : CatalogueCoreHospitalController<DiagnoticTypes, DiagnoticTypeModel, BaseHospitalSearch>
    {
        public DiagnoticTypeController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<DiagnoticTypes, DiagnoticTypeModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IDiagnoticTypeService>();
        }
    }
}
