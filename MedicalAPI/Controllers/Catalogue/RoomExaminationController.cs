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
    [Route("api/room-examination")]
    [ApiController]
    [Description("Phòng khám bệnh")]
    public class RoomExaminationController : CatalogueController<RoomExaminations, RoomExaminationModel, SearchHopitalExtension>
    {
        public RoomExaminationController(IServiceProvider serviceProvider, ILogger<CatalogueController<RoomExaminations, RoomExaminationModel, SearchHopitalExtension>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IRoomExaminationService>();
        }
    }
}
