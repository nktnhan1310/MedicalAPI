using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Core.App.Controllers
{
    [Route("api/v1/authenticate")]
    [ApiController]
    public abstract class AuthCoreController : ControllerBase
    {
        protected readonly ILogger<AuthCoreController> logger;
        private readonly IUserService userService;
        private IConfiguration configuration;
        private IMapper mapper;
        private IEmailConfigurationService emailConfigurationService;
        private readonly ITokenManagerService tokenManagerService;
        public AuthCoreController(IServiceProvider serviceProvider
            , IConfiguration configuration
            , IMapper mapper, ILogger<AuthCoreController> logger
            )
        {
            this.logger = logger;
            this.configuration = configuration;
            this.mapper = mapper;

            userService = serviceProvider.GetRequiredService<IUserService>();
            emailConfigurationService = serviceProvider.GetRequiredService<IEmailConfigurationService>();
            tokenManagerService = serviceProvider.GetRequiredService<ITokenManagerService>();
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public virtual async Task<AppDomainResult> LoginAsync([FromBody] LoginModel loginModel)
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
                        var token = await GenerateJwtToken(userModel);
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
        public virtual async Task<AppDomainResult> Register([FromBody] RegisterModel register)
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
                };
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
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("changePassword/{userId}")]
        public virtual async Task<AppDomainResult> ChangePassword(int userId, [FromBody] ChangePasswordModel changePasswordModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                // Check current user
                if (LoginContext.Instance.CurrentUser.UserId != userId)
                    throw new AppException("Không phải người dùng hiện tại");
                // Check old Password + new Password
                string messageCheckPassword = await this.userService.CheckCurrentUserPassword(userId, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
                if (!string.IsNullOrEmpty(messageCheckPassword))
                    throw new AppException(messageCheckPassword);

                var userInfo = await this.userService.GetByIdAsync(userId);
                userInfo.Password = SecurityUtils.HashSHA1(changePasswordModel.NewPassword);
                appDomainResult.Success = await userService.UpdateAsync(userInfo);
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }

        /// <summary>
        /// Quên mật khẩu
        /// <para>Gửi mật khẩu mới qua Email nếu username là email</para>
        /// <para>Gửi mật khẩu mới qua SMS nếu username là phone</para>
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("forgot-password/{userName}")]
        public virtual async Task<AppDomainResult> ForgotPassword(string userName)
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
            // Cấp mật khẩu mới
            var newPasswordRandom = RandomUtilities.RandomString(8);
            userInfo.Password = SecurityUtils.HashSHA1(newPasswordRandom);
            bool success = await this.userService.UpdateAsync(userInfo);
            if (success)
            {
                // Gửi mã qua Email
                if (isValidEmail)
                {
                    string emailBody = string.Format("<p>Your new password: {0}</p>", newPasswordRandom);
                    emailConfigurationService.Send("Change Password", new string[] { userInfo.Email }, null, null, new EmailContent()
                    {
                        Content = emailBody,
                        IsHtml = true,
                    });
                }
                // Gửi SMS
                else if (isValidPhone)
                {

                }
            }
            return appDomainResult;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        public virtual async Task<AppDomainResult> Logout()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            await this.tokenManagerService.DeactivateCurrentAsync();
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }

        #region Private methods

        private async Task<string> GenerateJwtToken(UserModel user)
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
            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly[] assems = currentDomain.GetAssemblies();
            var controllers = new List<ControllerModel>();
            var roles = new List<RoleModel>();
            foreach (Assembly assem in assems)
            {
                var controller = assem.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type) && !type.IsAbstract)
              .Select(e => new ControllerModel()
              {
                  Id = e.Name.Replace("Controller", string.Empty),
                  Name = string.Format("{0}", ReflectionUtils.GetClassDescription(e)).Replace("Controller", string.Empty)
              }).OrderBy(e => e.Name)
                  .Distinct();
                controllers.AddRange(controller);
            }
            if (controllers.Any())
            {
                foreach (var controller in controllers)
                {
                    roles.Add(new RoleModel()
                    {
                        RoleName = controller.Id,
                        IsView = await this.userService.HasPermission(userLoginModel.UserId, controller.Id, new string[] { CoreContants.View })
                    });
                }
            }
            userLoginModel.Roles = roles;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userLoginModel))
                            }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion
    }
}
