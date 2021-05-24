using Medical.Core.App.Controllers;
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
    [Route("api/nation")]
    [ApiController]
    [Description("Quản lý thông tin dân tộc")]
    public class NationController : CatalogueController<Nations, NationModel, BaseSearch>
    {
        public NationController(IServiceProvider serviceProvider, ILogger<CatalogueController<Nations, NationModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<INationService>();
        }
    }
}
