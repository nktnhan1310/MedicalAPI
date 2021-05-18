using Medical.Entities;
using Medical.Interface.Services;
using MedicalAPI.Model;
using MedicalAPI.Utils;
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

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        protected readonly ILogger<AuthController> logger;
        private readonly IUserService userService;
        private IConfiguration configuration;
        private IMapper mapper;
        public AuthController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, ILogger<AuthController> logger)
        {
            this.logger = logger;
            userService = serviceProvider.GetRequiredService<IUserService>();
            this.configuration = configuration;
            this.mapper = mapper;
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<AppDomainResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                success = await this.userService.Verify(loginModel.UserName, loginModel.Password);
                if (success)
                {
                    var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.UserName == loginModel.UserName);
                    if (userInfos != null && userInfos.Any())
                    {
                        var userModel = mapper.Map<UserModel>(userInfos.FirstOrDefault());
                        var token = GenerateJwtToken(userModel);

                        appDomainResult = new AppDomainResult()
                        {
                            Success = true,
                            Data = new
                            {
                                token = token
                            },
                            ResultCode = (int)HttpStatusCode.OK
                        };

                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("User name or password is incorrect");
                }
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Register")]
        public async Task<AppDomainResult> Register([FromBody] RegisterModel register)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                // Kiểm tra định dạng user name
                bool isValidUser = ValidateUserName.IsValidUserName(register.UserName);
                if (!isValidUser)
                {
                    throw new AppException("Vui lòng nhập số điện thoại hoặc email!");
                }

                var user = new Users()
                {
                    UserName = register.UserName,
                    Password = register.Password,
                    Created = DateTime.Now,
                    Active = true,
                };
                // Kiểm tra item có tồn tại chưa?
                var messageUserCheck = await this.userService.GetExistItemMessage(user);
                if (!string.IsNullOrEmpty(messageUserCheck))
                {
                    throw new AppException(messageUserCheck);
                }
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
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{userId}")]
        public async Task<AppDomainResult> ChangePassword(int userId, [FromBody] ChangePasswordModel changePasswordModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                var userInfo = await this.userService.GetByIdAsync(userId);
                userInfo.Password = SecurityUtils.HashSHA1(changePasswordModel.NewPassword);
                appDomainResult.Success = await userService.UpdateAsync(userInfo);
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }

            return appDomainResult;
        }

        /// <summary>
        /// Quên mật khẩu
        /// Gửi mật khẩu mới qua Email nếu username là email
        /// Gửi mật khẩu mới qua SMS nếu username là phone
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("userName")]
        public async Task<AppDomainResult> ForgotPassword(string userName)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool isValidEmail = ValidateUserName.IsEmail(userName);
            bool isValidPhone = ValidateUserName.IsPhoneNumber(userName);
            // Kiểm tra đúng định dạng email và số điện thoại chưa
            if (!isValidEmail && !isValidPhone)
                throw new AppException("Vui lòng nhập email hoặc số điện thoại!");
            // Tạo mật khẩu mới
            // Kiểm tra email/phone đã tồn tại chưa?
            var userInfos = await this.userService.GetAsync(e => !e.Deleted
            && (e.UserName == userName
            || e.Email == userName
            || e.Phone == userName
            )
            );
            Users userInfo = null;
            if (userInfos != null && userInfos.Any())
                userInfo = userInfos.FirstOrDefault();
            if (userInfo == null)
                throw new AppException("Số điện thoại hoặc email không tồn tại");
            // Cập nhật mật khẩu mới
            userInfo.Password = SecurityUtils.HashSHA1(RandomUtilities.RandomString(8));
            bool success = await this.userService.UpdateAsync(userInfo);
            if (success)
            {
                // Gửi mã qua Email
                if (isValidEmail)
                {

                }
                // Gửi SMS
                else if (isValidPhone)
                {

                }
            }
            return appDomainResult;
        }


        #region Private methods

        private string GenerateJwtToken(UserModel user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var appSettingsSection = configuration.GetSection("AppSettings");
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var userLoginModel = new UserLoginModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userLoginModel))
                            }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion

    }
}
