using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/one-signal")]
    [ApiController]
    [Description("Push notify OneSignal")]
    public class OneSignalController : OneSignalCoreController
    {
        public OneSignalController(IServiceProvider serviceProvider, IConfiguration configuration) : base(serviceProvider, configuration)
        {
        }
    }
}
