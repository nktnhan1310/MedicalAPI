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
    [Route("api/hospital-type")]
    [ApiController]
    [Description("Quản lý danh mục loại bệnh viện")]
    [Authorize]
    public class HospitalTypeController : CatalogueController<HospitalTypes, HospitalTypeModel, BaseSearch>
    {
        public HospitalTypeController(IServiceProvider serviceProvider, ILogger<BaseController<HospitalTypes, HospitalTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IHospitalTypeService>();
        }
    }
}
