using AutoMapper;
using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    [Description("Đăng nhập/đăng ký/quên mật khẩu/ đổi mật khẩu")]
    public class AuthController : AuthCoreController
    {
        private readonly IUserService userService;
        private readonly IUserGroupService userGroupService;
        public AuthController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, ILogger<AuthCoreController> logger) : base(serviceProvider, configuration, mapper, logger)
        {
            userService = serviceProvider.GetRequiredService<IUserService>();
            userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public override async Task<AppDomainResult> Register([FromBody] RegisterModel register)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                // Kiểm tra định dạng user name
                bool isValidUser = ValidateUserName.IsValidUserName(register.UserName);
                if (!isValidUser)
                    throw new AppException("Vui lòng nhập số điện thoại hoặc email!");

                var user = new Users()
                {
                    UserName = register.UserName,
                    Password = register.Password,
                    Created = DateTime.Now,
                    CreatedBy = register.UserName,
                    Active = true,
                    Phone = ValidateUserName.IsPhoneNumber(register.UserName) ? register.UserName : string.Empty,
                    Email = ValidateUserName.IsEmail(register.UserName) ? register.UserName : string.Empty,
                    UserInGroups = new List<UserInGroups>(),
                };
                // Tạo mặc định trong group User
                var groupUserInfos = await userGroupService.GetAsync(e => e.Code == CoreContants.USER_GROUP);
                if (groupUserInfos != null && groupUserInfos.Any())
                {
                    UserInGroups userInGroups = new UserInGroups()
                    {
                        UserGroupId = groupUserInfos.FirstOrDefault().Id,
                        Created = DateTime.Now,
                        CreatedBy = register.UserName,
                        Deleted = false,
                        Active = true,
                    };
                    user.UserInGroups.Add(userInGroups);
                }

                // Kiểm tra item có tồn tại chưa?
                var messageUserCheck = await this.userService.GetExistItemMessage(user);
                if (!string.IsNullOrEmpty(messageUserCheck))
                    throw new AppException(messageUserCheck);
                user.Password = SecurityUtils.HashSHA1(register.Password);
                appDomainResult.Success = await userService.CreateAsync(user);
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            }
            else
            {
                var resultMessage = ModelState.GetErrorMessage();
                throw new AppException(resultMessage);
            }
            return appDomainResult;
        }
    }
}
