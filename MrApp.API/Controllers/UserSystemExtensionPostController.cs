using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Route("api/user-system-extension-post")]
    [ApiController]
    [Description("Quản lý bài viết của hệ thống theo từng đối tượng")]
    [Authorize]
    public class UserSystemExtensionPostController : BaseController
    {
        private readonly IUserSystemExtensionPostService userSystemExtensionPostService;
        public UserSystemExtensionPostController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            userSystemExtensionPostService = serviceProvider.GetRequiredService<IUserSystemExtensionPostService>();
        }

        /// <summary>
        /// Xem danh sách bài viết của hệ thống
        /// </summary>
        /// <param name="searchUserSystemExtensionPost">Bộ lọc tìm kiếm bài viết</param>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> Get([FromQuery] SearchUserSystemExtensionPost searchUserSystemExtensionPost)
        {
            var pagedListModel = new PagedList<UserSystemExtensionPostModel>();
            var pagedList = await this.userSystemExtensionPostService.GetPagedListData(searchUserSystemExtensionPost);
            if (pagedList != null) pagedListModel = mapper.Map<PagedList<UserSystemExtensionPostModel>>(pagedList);
            return new AppDomainResult()
            {
                 Success = true,
                 Data = pagedListModel,
                 ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xem danh sách bài viết của hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetById(int id)
        {
            var itemModel = new UserSystemExtensionPostModel();
            var itemInfo = await this.userSystemExtensionPostService.GetByIdAsync(id);
            if (itemInfo != null) itemModel = mapper.Map<UserSystemExtensionPostModel>(itemInfo);
            return new AppDomainResult()
            {
                Data = itemModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
