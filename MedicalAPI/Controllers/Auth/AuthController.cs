using Medical.Entities;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Medical.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Http;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : AuthCoreController
    {
        public AuthController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, ILogger<AuthCoreController> logger) : base(serviceProvider, configuration, mapper, logger)
        {
        }
    }
}
