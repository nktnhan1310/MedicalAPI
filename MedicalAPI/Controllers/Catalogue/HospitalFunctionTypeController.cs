using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
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
    [Route("api/hospital-function-type")]
    [ApiController]
    [Description("Quản lý danh mục chức năng của bệnh viện")]
    public class HospitalFunctionTypeController : CatalogueController<HospitalFunctionTypes, HospitalFunctionTypeModel, BaseSearch>
    {
        public HospitalFunctionTypeController(IServiceProvider serviceProvider, ILogger<BaseController<HospitalFunctionTypes, HospitalFunctionTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IHospitalFunctionTypeService>();
        }
    }
}
