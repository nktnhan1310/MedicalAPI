using AutoMapper;
using Medical.Extensions;
using Medical.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IMapper mapper;
        protected IWebHostEnvironment env;
        protected IConfiguration configuration;
        protected BaseController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.env = env;
            this.configuration = configuration;
        }


    }
}
