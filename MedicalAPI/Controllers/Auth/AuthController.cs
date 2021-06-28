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
    [Route("api/authenticate")]
    [ApiController]
    [Description("Authenticate")]
    public class AuthController : AuthCoreController
    {
        public AuthController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, ILogger<AuthCoreController> logger) : base(serviceProvider, configuration, mapper, logger)
        {
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public override async Task<AppDomainResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                success = await this.userService.Verify(loginModel.UserName, loginModel.Password);
                if (success)
                {
                    var userInfos = await this.userService.GetAsync(e => !e.Deleted
                    && (e.UserName == loginModel.UserName
                    || e.Phone == loginModel.UserName
                    || e.Email == loginModel.UserName
                    ));
                    if (userInfos != null && userInfos.Any())
                    {
                        var userModel = mapper.Map<UserModel>(userInfos.FirstOrDefault());
                        var token = await GenerateJwtToken(userModel);
                        // Lưu giá trị token
                        await this.userService.UpdateUserToken(userModel.Id, token, true);
                        appDomainResult = new AppDomainResult()
                        {
                            Success = true,
                            Data = new
                            {
                                token = token,
                            },
                            ResultCode = (int)HttpStatusCode.OK
                        };

                    }
                }
                else
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không chính xác");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }

    }
}
