using Medical.Core.App.Controllers;
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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/user-system-extension-post")]
    [ApiController]
    [Description("Quản lý bài viết của hệ thống theo đối tượng")]
    [Authorize]
    public class UserSystemExtensionPostController : BaseController<UserSystemExtensionPosts, UserSystemExtensionPostModel, SearchUserSystemExtensionPost>
    {
        private readonly IConfiguration configuration;
        public UserSystemExtensionPostController(IServiceProvider serviceProvider, ILogger<BaseController<UserSystemExtensionPosts, UserSystemExtensionPostModel, SearchUserSystemExtensionPost>> logger, IWebHostEnvironment env, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserSystemExtensionPostService>();

            this.configuration = configuration;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] UserSystemExtensionPostModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            itemModel.Created = DateTime.Now;
            itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemModel.Active = true;
            var item = mapper.Map<UserSystemExtensionPosts>(itemModel);
            if (item == null) throw new AppException("Item không tồn tại");
            // Kiểm tra item có tồn tại chưa?
            var messageUserCheck = await this.domainService.GetExistItemMessage(item);
            if (!string.IsNullOrEmpty(messageUserCheck))
                throw new AppException(messageUserCheck);

            // Lấy thông tin folder upload file logo + background lên
            string folderUploadPath = string.Empty;
            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");

            if (isProduct)
                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
            else
                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER);

            // Lấy đường dẫn cho file logo
            string fileLogoPath = string.Empty;
            string fileLogoTempPath = string.Empty;
            if (!string.IsNullOrEmpty(itemModel.LogoImgTempName))
            {

                fileLogoTempPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.LogoImgTempName);
                fileLogoPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileLogoTempPath));
                // Kiểm tra có tồn tại file trong temp chưa?
                if (System.IO.File.Exists(fileLogoTempPath) && !System.IO.File.Exists(fileLogoPath))
                {
                    FileUtils.CreateDirectory(folderUploadUrl);
                    FileUtils.SaveToPath(fileLogoPath, System.IO.File.ReadAllBytes(fileLogoTempPath));
                    item.LogoImgUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER, Path.GetFileName(fileLogoTempPath));
                }
            }
            // Lấy đường dẫn cho file background
            string fileBackGroundPath = string.Empty;
            string fileBackGroundTempPath = string.Empty;
            if (!string.IsNullOrEmpty(itemModel.BackGroundImgTempName))
            {

                fileBackGroundTempPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.BackGroundImgTempName);
                fileBackGroundPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileBackGroundTempPath));
                // Kiểm tra có tồn tại file trong temp chưa?
                if (System.IO.File.Exists(fileBackGroundTempPath) && !System.IO.File.Exists(fileBackGroundPath))
                {
                    FileUtils.CreateDirectory(folderUploadUrl);
                    FileUtils.SaveToPath(fileBackGroundPath, System.IO.File.ReadAllBytes(fileBackGroundTempPath));
                    item.BackGroundImgUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER, Path.GetFileName(fileBackGroundTempPath));
                }
            }

            success = await this.domainService.CreateAsync(item);
            // Xóa thông tin file logo + background trong folder temp
            System.IO.File.Delete(fileLogoTempPath);
            System.IO.File.Delete(fileBackGroundTempPath);
            if (!success)
            {
                // Xóa thông tin file logo + background trên folder upload
                System.IO.File.Delete(fileLogoPath);
                System.IO.File.Delete(fileBackGroundPath);
                throw new Exception("Lỗi trong quá trình xử lý");
            }
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = true;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] UserSystemExtensionPostModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            itemModel.Updated = DateTime.Now;
            itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            itemModel.Active = true;
            var item = mapper.Map<UserSystemExtensionPosts>(itemModel);
            if (item == null) throw new AppException("Item không tồn tại");
            // Kiểm tra item có tồn tại chưa?
            var messageUserCheck = await this.domainService.GetExistItemMessage(item);
            if (!string.IsNullOrEmpty(messageUserCheck))
                throw new AppException(messageUserCheck);

            // Lấy thông tin folder upload file logo + background lên
            string folderUploadPath = string.Empty;
            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");

            if (isProduct)
                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
            else
                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER);

            // Lấy đường dẫn cho file logo
            string fileLogoPath = string.Empty;
            string fileLogoTempPath = string.Empty;
            if (!string.IsNullOrEmpty(itemModel.LogoImgTempName))
            {

                fileLogoTempPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.LogoImgTempName);
                fileLogoPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileLogoTempPath));
                // Kiểm tra có tồn tại file trong temp chưa?
                if (System.IO.File.Exists(fileLogoTempPath) && !System.IO.File.Exists(fileLogoPath))
                {
                    FileUtils.CreateDirectory(folderUploadUrl);
                    FileUtils.SaveToPath(fileLogoPath, System.IO.File.ReadAllBytes(fileLogoTempPath));
                    item.LogoImgUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER, Path.GetFileName(fileLogoTempPath));
                }
            }
            // Lấy đường dẫn cho file background
            string fileBackGroundPath = string.Empty;
            string fileBackGroundTempPath = string.Empty;
            if (!string.IsNullOrEmpty(itemModel.BackGroundImgTempName))
            {

                fileBackGroundTempPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.BackGroundImgTempName);
                fileBackGroundPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileBackGroundTempPath));
                // Kiểm tra có tồn tại file trong temp chưa?
                if (System.IO.File.Exists(fileBackGroundTempPath) && !System.IO.File.Exists(fileBackGroundPath))
                {
                    FileUtils.CreateDirectory(folderUploadUrl);
                    FileUtils.SaveToPath(fileBackGroundPath, System.IO.File.ReadAllBytes(fileBackGroundTempPath));
                    item.BackGroundImgUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.SYSTEM_FOLDER, Path.GetFileName(fileBackGroundTempPath));
                }
            }

            success = await this.domainService.UpdateAsync(item);
            // Xóa thông tin file logo + background trong folder temp
            System.IO.File.Delete(fileLogoTempPath);
            System.IO.File.Delete(fileBackGroundTempPath);
            if (!success)
            {
                // Xóa thông tin file logo + background trên folder upload
                System.IO.File.Delete(fileLogoPath);
                System.IO.File.Delete(fileBackGroundPath);
                throw new Exception("Lỗi trong quá trình xử lý");
            }
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = true;
            return appDomainResult;
        }

    }
}
