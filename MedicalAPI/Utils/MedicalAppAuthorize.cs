using Medical.Interface.Services;
using MedicalAPI.Context;
using MedicalAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MedicalAppAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] Permissions;

        public MedicalAppAuthorize(string[] permission)
        {
            Permissions = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserLoginModel)context.HttpContext.Items["User"];//.User;
            string controllerName = string.Empty;
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                controllerName = descriptor.ControllerName;
            }

            if (user == null)
            {
                context.Result = new JsonResult(new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.Unauthorized,
                    ResultMessage = "Unauthorized"
                });
                return;
            }

            IUserService userService = (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService));
            IConfiguration configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
            var hasPermit = false;
#if DEBUG
            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            if (appSettings != null && appSettings.GrantPermissionDebug)
            {
                hasPermit = true;
            }
            else
            {
                var userCheckResult = userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, controllerName, Permissions);
                hasPermit = userCheckResult.Result;
            }
#else
                var userCheckResult = userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, controllerName, Permissions);
                hasPermit = userCheckResult.Result;
#endif

            if (!hasPermit)
            {
                context.Result = new JsonResult(new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.Unauthorized,
                    ResultMessage = "Unauthorized"
                });
                //new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                return;
            }

        }
    }
}
