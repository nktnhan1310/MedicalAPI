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
    [Route("api/job")]
    [ApiController]
    [Description("Quản lý chức danh/nghề nghiệp")]
    public class JobController : CatalogueController<Jobs, JobModel, BaseSearch>
    {
        public JobController(IServiceProvider serviceProvider, ILogger<CatalogueController<Jobs, JobModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IJobService>();
        }
    }
}
