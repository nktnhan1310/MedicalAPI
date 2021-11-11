using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Entities.Search;
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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/user-folder")]
    [ApiController]
    [Description("Quản lý folder của user")]
    [Authorize]
    public class UserFolderController : BaseController<UserFolders, UserFolderModel, SearchUserFolder>
    {
        protected IUserFolderService userFolderService;
        protected IUserFileService userFileService;
        protected IMedicalRecordDetailFileService medicalRecordDetailFileService;
        protected IMedicalRecordFileService medicalRecordFileService;
        protected IConfiguration configuration;

        public UserFolderController(IServiceProvider serviceProvider, ILogger<BaseController<UserFolders, UserFolderModel, SearchUserFolder>> logger, IWebHostEnvironment env, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.configuration = configuration;

            this.domainService = serviceProvider.GetRequiredService<IUserFolderService>();
            userFolderService = serviceProvider.GetRequiredService<IUserFolderService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
            medicalRecordDetailFileService = serviceProvider.GetRequiredService<IMedicalRecordDetailFileService>();
            medicalRecordFileService = serviceProvider.GetRequiredService<IMedicalRecordFileService>();
        }

        /// <summary>
        /// Lấy danh sách file
        /// </summary>
        /// <param name="searchUserFile"></param>
        /// <returns></returns>
        [HttpGet("get-user-file")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserFile([FromQuery] SearchUserFile searchUserFile)
        {
            PagedList<UserFileModel> pagedList = new PagedList<UserFileModel>();
            //searchUserFile.UserId = LoginContext.Instance.CurrentUser.UserId;
            // File của tiểu sử hồ sơ bệnh nhân
            var userFiles = await this.userFileService.GetPagedListData(searchUserFile);
            if (userFiles != null && userFiles.Items.Any())
                pagedList = mapper.Map<PagedList<UserFileModel>>(userFiles);
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                Success = true,
                Data = pagedList
            };
        }

        /// <summary>
        /// Lấy thông tin file theo ngày/tháng
        /// </summary>
        /// <param name="searchUserFile"></param>
        /// <returns></returns>
        [HttpGet("get-user-file-extension")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserFileExtension([FromQuery] SearchUserFile searchUserFile)
        {
            PagedList<UserFileExtensionModel> pagedListModel = new PagedList<UserFileExtensionModel>();
            //searchUserFile.UserId = LoginContext.Instance.CurrentUser.UserId;
            var pagedList = await this.userFileService.GetPagedListExtension(searchUserFile);
            if (pagedList != null && pagedList.Items.Any())
                pagedListModel = mapper.Map<PagedList<UserFileExtensionModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = pagedListModel
            };
        }

        /// <summary>
        /// Lấy thông tin folder theo ngày/tháng
        /// </summary>
        /// <param name="searchUserFolder"></param>
        /// <returns></returns>
        [HttpGet("get-user-folder-extension")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserFolderExtension([FromQuery] SearchUserFolder searchUserFolder)
        {
            PagedList<UserFolderExtensionModel> pagedListModel = new PagedList<UserFolderExtensionModel>();
            //searchUserFolder.UserId = LoginContext.Instance.CurrentUser.UserId;
            var pagedList = await this.userFolderService.GetPagedListExtension(searchUserFolder);
            if (pagedList != null && pagedList.Items.Any())
                pagedListModel = mapper.Map<PagedList<UserFolderExtensionModel>>(pagedList);
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = pagedListModel
            };
        }
    }
}
