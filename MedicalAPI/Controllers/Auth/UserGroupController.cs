using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAPI.Controllers
{
    [Route("api/user-group")]
    [ApiController]
    [Description("Quản lý nhóm người dùng")]
    [Authorize]
    public class UserGroupController : CatalogueCoreHospitalController<UserGroups, UserGroupModel, BaseHospitalSearch>
    {
        private readonly IUserInGroupService userInGroupService;
        private readonly IPermissionService permissionService;
        private readonly IPermitObjectPermissionService permitObjectPermissionService;

        public UserGroupController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<UserGroups, UserGroupModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IUserGroupService>();
            userInGroupService = serviceProvider.GetRequiredService<IUserInGroupService>();
            permissionService = serviceProvider.GetRequiredService<IPermissionService>();
            permitObjectPermissionService = serviceProvider.GetRequiredService<IPermitObjectPermissionService>();
        }

        /// <summary>
        /// Lấy thông tin nhóm theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            var item = await this.catalogueService.GetByIdAsync(id);
            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<UserGroupModel>(item);
                    var userInGroups = await this.userInGroupService.GetAsync(e => !e.Deleted && e.UserGroupId == id);
                    if (userInGroups != null)
                        itemModel.UserInGroups = mapper.Map<IList<UserInGroupModel>>(userInGroups);

                    var permitObjectPermissions = await this.permitObjectPermissionService.GetAsync(e => !e.Deleted && e.UserGroupId == id);
                    if (permitObjectPermissions != null)
                        itemModel.PermitObjectPermissions = mapper.Map<IList<PermitObjectPermissionModel>>(permitObjectPermissions);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new KeyNotFoundException("Item không tồn tại");

            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-permissions")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetPermissionCatalogue()
        {
            var listPermissions = await this.permissionService.GetAsync(e => !e.Deleted);
            var listPermissionModels = mapper.Map<List<PermissionModel>>(listPermissions);
            return new AppDomainResult()
            {
                Data = listPermissionModels,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            }; ;
        }

        /// <summary>
        /// Lấy danh sách phân trang người dùng thuộc nhóm
        /// </summary>
        /// <param name="searchUserInGroup"></param>
        /// <returns></returns>
        [HttpGet("get-user-in-groups")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserInGroups([FromQuery] SearchUserInGroup searchUserInGroup)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var pagedList = await this.userInGroupService.GetPagedListData(searchUserInGroup);
            var pagedListModel = mapper.Map<PagedList<UserInGroupModel>>(pagedList);
            appDomainResult = new AppDomainResult()
            {
                Data = pagedListModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }
    }
}
