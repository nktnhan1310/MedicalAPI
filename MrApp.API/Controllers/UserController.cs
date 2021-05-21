using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Description("Quản lý thông tin người dùng")]
    [MedicalAppAuthorize(new string[] { CoreContants.View, CoreContants.Update })]
    public class UserController : BaseController<Users, UserModel, SearchUser>
    {
        private readonly IUserService userService;
        public UserController(IServiceProvider serviceProvider, ILogger<UserController> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
        }

        /// <summary>
        /// Lấy thông tin user theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.UserId != id)
                throw new UnauthorizedAccessException("Không có quyền truy cập");
            if (id == 0)
                throw new KeyNotFoundException("id không tồn tại");
            var item = await this.domainService.GetByIdAsync(id, e => new Users()
            {
                Id = e.Id,
                Deleted = e.Deleted,
                Active = e.Active,
                Created = e.Created,
                CreatedBy = e.CreatedBy,
                Address = e.Address,
                Age = e.Age,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                UserName = e.UserName,
                Updated = e.Updated,
                UpdatedBy = e.UpdatedBy,
            });
            if (item != null)
            {
                var itemModel = mapper.Map<UserModel>(item);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new KeyNotFoundException("Item không tồn tại");
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin Profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] UserModel itemModel)
        {
            if (LoginContext.Instance.CurrentUser.UserId != id)
                throw new UnauthorizedAccessException("Không có quyền truy cập");
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<Users>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    item.Updated = DateTime.Now;
                    item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    Expression<Func<Users, object>>[] includeProperties = new Expression<Func<Users, object>>[]
                    {
                    x => x.FirstName,
                    x => x.LastName,
                    x => x.Email,
                    x => x.Phone,
                    x => x.Updated,
                    x => x.UpdatedBy,
                    x => x.UserName,
                    x => x.Age,
                    x => x.Address,
                    };
                    success = await this.domainService.UpdateFieldAsync(item, includeProperties);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
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
            if (LoginContext.Instance.CurrentUser.UserId != id)
                throw new UnauthorizedAccessException("Không có quyền truy cập");
            bool success = false;
            var item = mapper.Map<Users>(itemModel);
            if (item != null)
            {
                item.Updated = DateTime.Now;
                item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                Expression<Func<Users, object>>[] includeProperties = new Expression<Func<Users, object>>[]
                {
                    x => x.FirstName,
                    x => x.LastName,
                    x => x.Email,
                    x => x.Phone,
                    x => x.Updated,
                    x => x.UpdatedBy,
                    x => x.UserName,
                    x => x.Age,
                    x => x.Address,
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
