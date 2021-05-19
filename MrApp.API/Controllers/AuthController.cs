using AutoMapper;
using Medical.Core.App.Controllers;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Đăng nhập/đăng ký/quên mật khẩu/ đổi mật khẩu")]
    public class AuthController : AuthCoreController
    {
        public AuthController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, ILogger<AuthCoreController> logger) : base(serviceProvider, configuration, mapper, logger)
        {
        }

    }
}
