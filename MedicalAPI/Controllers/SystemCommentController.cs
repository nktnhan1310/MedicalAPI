using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/system-comment")]
    [ApiController]
    [Description("Thông tin liên hệ")]
    [Authorize]
    public class SystemCommentController : BaseController<SystemComments, SystemCommentModel, SearchSystemComment>
    {
        public SystemCommentController(IServiceProvider serviceProvider, ILogger<BaseController<SystemComments, SystemCommentModel, SearchSystemComment>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<ISystemCommentService>();
        }
    }
}
