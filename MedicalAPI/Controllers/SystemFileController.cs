using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
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
    [Route("api/system-file")]
    [ApiController]
    [Description("Quản lý file của hệ thống")]
    [Authorize]
    public class SystemFileController : BaseController<SystemFiles, SystemFileModel, SearchSystemFile>
    {
        private IConfiguration configuration;
        public SystemFileController(IServiceProvider serviceProvider, ILogger<BaseController<SystemFiles, SystemFileModel, SearchSystemFile>> logger, IWebHostEnvironment env, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<ISystemFileService>();

            this.configuration = configuration;
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> GetPagedData([FromQuery] SearchSystemFile baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                    baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;

                PagedList<SystemFiles> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<SystemFileModel> pagedDataModel = mapper.Map<PagedList<SystemFileModel>>(pagedData);
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] SystemFileModel itemModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var itemUpdate = mapper.Map<SystemFiles>(itemModel);
            itemUpdate.Active = true;
            itemUpdate.Created = DateTime.Now;
            itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                itemUpdate.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            // Kiểm tra item có tồn tại chưa?
            var messageUserCheck = await this.domainService.GetExistItemMessage(itemUpdate);
            if (!string.IsNullOrEmpty(messageUserCheck))
                throw new AppException(messageUserCheck);
            string filePath = string.Empty;
            string fileUploadPath = string.Empty;
            string folderUploadPath = string.Empty;
            string folderUploadUrl = string.Empty;
            if (!string.IsNullOrEmpty(itemModel.FileName))
            {
                var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                if (isProduct)
                    folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                else
                    folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.FileName);
                folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER);
                fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                {
                    // ------- START GET URL FOR FILE
                    FileUtils.CreateDirectory(folderUploadUrl);
                    FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                    itemUpdate.FileUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER, Path.GetFileName(filePath));
                }
            }
            bool success = await this.domainService.CreateAsync(itemUpdate);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] SystemFileModel itemModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var itemUpdate = mapper.Map<SystemFiles>(itemModel);
            itemUpdate.Active = true;
            itemUpdate.Updated = DateTime.Now;
            itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                itemUpdate.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;

            // Kiểm tra item có tồn tại chưa?
            var messageUserCheck = await this.domainService.GetExistItemMessage(itemUpdate);
            if (!string.IsNullOrEmpty(messageUserCheck))
                throw new AppException(messageUserCheck);
            string filePath = string.Empty;
            string fileUploadPath = string.Empty;
            string folderUploadPath = string.Empty;
            string folderUploadUrl = string.Empty;
            if (!string.IsNullOrEmpty(itemModel.FileName))
            {
                var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                if (isProduct)
                    folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                else
                    folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.FileName);
                folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER);
                fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                {
                    // ------- START GET URL FOR FILE
                    FileUtils.CreateDirectory(folderUploadUrl);
                    FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                    itemUpdate.FileUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER, Path.GetFileName(filePath));
                }
            }
            bool success = await this.domainService.UpdateAsync(itemUpdate);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
