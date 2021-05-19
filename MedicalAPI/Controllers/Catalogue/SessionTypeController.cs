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
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Buổi khám bệnh")]
    public class SessionTypeController : CatalogueController<SessionTypes, SessionTypeModel, BaseSearch>
    {
        public SessionTypeController(IServiceProvider serviceProvider, ILogger<CatalogueController<SessionTypes, SessionTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<ISessionTypeService>();
        }
    }
}
