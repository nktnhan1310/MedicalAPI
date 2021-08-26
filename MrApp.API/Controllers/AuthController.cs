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
    [Description("Authenticate")]
    public class AuthController : AuthCoreController
    {
        private readonly IUserGroupService userGroupService;
        private readonly IFaceBookAuthService faceBookAuthService;
        private readonly IGoogleAuthService googleAuthService;

        public AuthController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, ILogger<AuthCoreController> logger) : base(serviceProvider, configuration, mapper, logger)
        {
            userService = serviceProvider.GetRequiredService<IUserService>();
            userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
            faceBookAuthService = serviceProvider.GetRequiredService<IFaceBookAuthService>();
            googleAuthService = serviceProvider.GetRequiredService<IGoogleAuthService>();
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
                success = await this.userService.Verify(loginModel.UserName, loginModel.Password, true);
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
                    Phone = register.Phone,
                    Email = register.Email,
                    IsCheckOTP = false,
                    UserGroupIds = new List<int>(),
                };
                // Tạo mặc định trong group User
                var groupUserInfos = await userGroupService.GetAsync(e => e.Code == CoreContants.USER_GROUP);
                if (groupUserInfos != null && groupUserInfos.Any())
                {
                    user.UserGroupIds.Add(groupUserInfos.FirstOrDefault().Id);
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


        /// <summary>
        /// Đăng nhập bằng facebook
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login-facebook")]
        public virtual async Task<AppDomainResult> LoginWithFaceBookAsync(string accessToken)
        {
            var validatedTokenResult = await faceBookAuthService.ValidateTokenAsync(accessToken);
            if (!validatedTokenResult.Data.IsValid)
            {
                throw new AppException("Invalid Facebook token");
            }
            // Lấy thông tin user của facebook
            var faceBookUserInfo = await faceBookAuthService.GetUserInfoAsync(accessToken);
            if (faceBookUserInfo == null)
            {
                throw new AppException("User info is not valid");

            }
            var existUserSystems = await userService.GetAsync(e => !e.Deleted 
            && (e.Email == faceBookUserInfo.Email || e.UserName == faceBookUserInfo.Email));
            if (existUserSystems == null || !existUserSystems.Any())
            {
                Users users = new Users()
                {
                    Deleted = false,
                    Active = true,
                    UserName = faceBookUserInfo.Email,
                    Email = faceBookUserInfo.Email,
                    Phone = string.Empty,
                    Created = DateTime.Now,
                    CreatedBy = "Api",
                    IsLoginFaceBook = true,
                    IsCheckOTP = true,
                    UserGroupIds = new List<int>(),
                };
                // Tạo mặc định trong group User
                var groupUserInfos = await userGroupService.GetAsync(e => e.Code == CoreContants.USER_GROUP);
                if (groupUserInfos != null && groupUserInfos.Any())
                {
                    users.UserGroupIds.Add(groupUserInfos.FirstOrDefault().Id);
                }
                bool success = await userService.CreateAsync(users);
                if (!success)
                {
                    throw new Exception("Something went wrong");
                }
                var userModel = mapper.Map<UserModel>(users);
                var token = await GenerateJwtToken(userModel);
                await this.userService.UpdateUserToken(userModel.Id, token, true);
                return new AppDomainResult()
                {
                    Success = true,
                    Data = new
                    {
                        token = token,
                    },
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                var userModel = mapper.Map<UserModel>(existUserSystems.FirstOrDefault());
                var token = await GenerateJwtToken(userModel);
                await this.userService.UpdateUserToken(userModel.Id, token, true);
                return new AppDomainResult()
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

        /// <summary>
        /// Đăng nhập bằng facebook
        /// </summary>
        /// <param name="googleAuths"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login-google")]
        public virtual async Task<AppDomainResult> LoginWithGoogleAsync([FromBody] GoogleAuths googleAuths)
        {
            var payload = await googleAuthService.VerifyGoogleToken(googleAuths);
            if (payload == null)
            {
                throw new AppException("Invalid google token");
            }
            var existUserSystems = await userService.GetAsync(e => !e.Deleted
            && (e.Email == payload.Email || e.UserName == payload.Email));
            if (existUserSystems == null || !existUserSystems.Any())
            {
                Users users = new Users()
                {
                    Deleted = false,
                    Active = true,
                    UserName = payload.Email,
                    Email = payload.Email,
                    Phone = string.Empty,
                    Created = DateTime.Now,
                    CreatedBy = "Api",
                    IsLoginFaceBook = true,
                    IsCheckOTP = true,
                    UserGroupIds = new List<int>(),
                };
                // Tạo mặc định trong group User
                var groupUserInfos = await userGroupService.GetAsync(e => e.Code == CoreContants.USER_GROUP);
                if (groupUserInfos != null && groupUserInfos.Any())
                {
                    users.UserGroupIds.Add(groupUserInfos.FirstOrDefault().Id);
                }
                bool success = await userService.CreateAsync(users);
                if (!success)
                {
                    throw new Exception("Something went wrong");
                }
                var userModel = mapper.Map<UserModel>(users);
                var token = await GenerateJwtToken(userModel);
                await this.userService.UpdateUserToken(userModel.Id, token, true);
                return new AppDomainResult()
                {
                    Success = true,
                    Data = new
                    {
                        token = token,
                    },
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                var userModel = mapper.Map<UserModel>(existUserSystems.FirstOrDefault());
                var token = await GenerateJwtToken(userModel);
                await this.userService.UpdateUserToken(userModel.Id, token, true);
                return new AppDomainResult()
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

    }
}
