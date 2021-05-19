using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Quản lý thông tin người dùng")]
    [Authorize]
    public class UserController : BaseController<Users, UserModel, SearchUser>
    {
        private readonly IUserService userService;
        public UserController(IServiceProvider serviceProvider, ILogger<UserController> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
        }

        /// <summary>
        /// Cập nhật 1 phần user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> PatchItem(int id, [FromBody] UserModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            var item = mapper.Map<Users>(itemModel);
            if (item != null)
            {
                Expression<Func<Users, object>>[] includeProperties = new Expression<Func<Users, object>>[]
                {
                        x => x.Active,
                        x => x.Status
                };
                success = await this.domainService.UpdateFieldAsync(item, includeProperties);
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                appDomainResult.Success = false;
            }

            return appDomainResult;
        }

    }
}
