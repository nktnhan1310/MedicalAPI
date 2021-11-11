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
    [Route("api/app-partner")]
    [ApiController]
    [Description("Quản lý thông tin đối tác của app")]
    [Authorize]
    public class AppPartnerController : CatalogueController<AppPartners, AppPartnerModel, BaseSearch>
    {
        public AppPartnerController(IServiceProvider serviceProvider, ILogger<BaseController<AppPartners, AppPartnerModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IAppPartnerService>();
        }
    }
}
