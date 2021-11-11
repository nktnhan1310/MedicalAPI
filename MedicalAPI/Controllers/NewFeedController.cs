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
    [Route("api/new-feed")]
    [ApiController]
    [Description("Quản lý tin tức")]
    [Authorize]
    public class NewFeedController : CoreHospitalController<NewFeeds, NewFeedModel, BaseHospitalSearch>
    {
        private IConfiguration configuration;
        public NewFeedController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<NewFeeds, NewFeedModel, BaseHospitalSearch>> logger, IWebHostEnvironment env, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.domainService = this.serviceProvider.GetRequiredService<INewFeedService>();

            this.configuration = configuration;
        }

        /// <summary>
        /// Thêm mới thông tin new feed
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] NewFeedModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemModel.Active = true;
                var item = mapper.Map<NewFeeds>(itemModel);
                string fileBackGroundPath = string.Empty;
                string fileUploadPath = string.Empty;
                string folderBackGroundUploadPath = string.Empty;

                string fileLogoPath = string.Empty;
                string folderLogoPath = string.Empty;

                string fileBannerPath = string.Empty;
                string folderBannerPath = string.Empty;

                string folderUploadUrl = string.Empty;
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    // Lấy thông tin đường dẫn file url
                    // Kiểm tra có tồn tại file trong temp chưa?
                    // Cập nhật nếu có thông tin file logo
                    if (!string.IsNullOrEmpty(itemModel.LogoFileName))
                    {
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderLogoPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderLogoPath = Path.Combine(Directory.GetCurrentDirectory());
                        fileLogoPath = Path.Combine(folderLogoPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.LogoFileName);
                        folderUploadUrl = Path.Combine(folderLogoPath, UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER);
                        fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileLogoPath));
                        if (System.IO.File.Exists(fileLogoPath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            // ------- START GET URL FOR FILE
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(fileLogoPath));
                            item.LogoUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER, Path.GetFileName(fileLogoPath));
                        }
                    }

                    // Cập nhật nếu có thông tin file banner
                    if (!string.IsNullOrEmpty(itemModel.BannerFileName))
                    {
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderBannerPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderBannerPath = Path.Combine(Directory.GetCurrentDirectory());
                        fileBannerPath = Path.Combine(folderBannerPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.BannerFileName);
                        folderUploadUrl = Path.Combine(folderBannerPath, UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER);
                        fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileBannerPath));
                        if (System.IO.File.Exists(fileBannerPath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            // ------- START GET URL FOR FILE
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(fileBannerPath));
                            item.BannerUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER, Path.GetFileName(fileBannerPath));
                        }
                    }
                    // Cập nhật nếu có thông tin file background
                    if (!string.IsNullOrEmpty(itemModel.BackGroundImgFileName))
                    {
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderBackGroundUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderBackGroundUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                        fileBackGroundPath = Path.Combine(folderBackGroundUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.BackGroundImgFileName);
                        folderUploadUrl = Path.Combine(folderBackGroundUploadPath, UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER);
                        fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileBackGroundPath));
                        if (System.IO.File.Exists(fileBackGroundPath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            // ------- START GET URL FOR FILE
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(fileBackGroundPath));
                            item.BackGroundImgUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER, Path.GetFileName(fileBackGroundPath));
                        }
                    }


                    success = await this.domainService.CreateAsync(item);
                    // Thành công
                    if (success)
                    {
                        // Xóa file trong thư mục temp
                        if (!string.IsNullOrEmpty(fileBackGroundPath))
                            System.IO.File.Delete(fileBackGroundPath);
                        if (!string.IsNullOrEmpty(fileLogoPath))
                            System.IO.File.Delete(fileLogoPath);
                        if (!string.IsNullOrEmpty(fileBannerPath))
                            System.IO.File.Delete(fileBannerPath);

                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        // Xóa file trong thư mục temp
                        if (!string.IsNullOrEmpty(fileBackGroundPath))
                            System.IO.File.Delete(fileBackGroundPath);

                        if (!string.IsNullOrEmpty(fileLogoPath))
                            System.IO.File.Delete(fileLogoPath);
                        if (!string.IsNullOrEmpty(fileBannerPath))
                            System.IO.File.Delete(fileBannerPath);

                        // Xóa file trong thư mục upload
                        if (!string.IsNullOrEmpty(folderBackGroundUploadPath))
                            System.IO.File.Delete(folderBackGroundUploadPath);
                        if (!string.IsNullOrEmpty(folderBannerPath))
                            System.IO.File.Delete(folderBannerPath);
                        if (!string.IsNullOrEmpty(folderLogoPath))
                            System.IO.File.Delete(folderLogoPath);
                        throw new Exception("Lỗi trong quá trình xử lý");
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new AppException("Item không tồn tại");
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin new feed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] NewFeedModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<NewFeeds>(itemModel);
                string fileBackGroundPath = string.Empty;
                string fileUploadPath = string.Empty;
                string folderBackGroundUploadPath = string.Empty;

                string fileLogoPath = string.Empty;
                string folderLogoPath = string.Empty;

                string fileBannerPath = string.Empty;
                string folderBannerPath = string.Empty;

                string folderUploadUrl = string.Empty;
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    // Cập nhật nếu có thông tin file logo
                    if (!string.IsNullOrEmpty(itemModel.LogoFileName))
                    {
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderLogoPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderLogoPath = Path.Combine(Directory.GetCurrentDirectory());
                        fileLogoPath = Path.Combine(folderLogoPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.LogoFileName);
                        folderUploadUrl = Path.Combine(folderLogoPath, UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER);
                        fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileLogoPath));
                        if (System.IO.File.Exists(fileLogoPath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            // ------- START GET URL FOR FILE
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(fileLogoPath));
                            item.LogoUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER, Path.GetFileName(fileLogoPath));
                        }
                    }

                    // Cập nhật nếu có thông tin file banner
                    if (!string.IsNullOrEmpty(itemModel.BannerFileName))
                    {
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderBannerPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderBannerPath = Path.Combine(Directory.GetCurrentDirectory());
                        fileBannerPath = Path.Combine(folderBannerPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.BannerFileName);
                        folderUploadUrl = Path.Combine(folderBannerPath, UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER);
                        fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileBannerPath));
                        if (System.IO.File.Exists(fileBannerPath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            // ------- START GET URL FOR FILE
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(fileBannerPath));
                            item.BannerUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER, Path.GetFileName(fileBannerPath));
                        }
                    }
                    // Cập nhật nếu có thông tin file background
                    if (!string.IsNullOrEmpty(itemModel.BackGroundImgFileName))
                    {
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderBackGroundUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderBackGroundUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                        fileBackGroundPath = Path.Combine(folderBackGroundUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, itemModel.BackGroundImgFileName);
                        folderUploadUrl = Path.Combine(folderBackGroundUploadPath, UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER);
                        fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(fileBackGroundPath));
                        if (System.IO.File.Exists(fileBackGroundPath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            // ------- START GET URL FOR FILE
                            FileUtils.CreateDirectory(folderUploadUrl);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(fileBackGroundPath));
                            item.BackGroundImgUrl = Path.Combine(UPLOAD_FOLDER_NAME, CoreContants.NEW_FEED_FOLDER, Path.GetFileName(fileBackGroundPath));
                        }
                    }
                    success = await this.domainService.UpdateAsync(item);
                    if (success)
                    {
                        // Xóa file trong thư mục temp
                        if (!string.IsNullOrEmpty(fileBackGroundPath))
                            System.IO.File.Delete(fileBackGroundPath);
                        if (!string.IsNullOrEmpty(fileLogoPath))
                            System.IO.File.Delete(fileLogoPath);
                        if (!string.IsNullOrEmpty(fileBannerPath))
                            System.IO.File.Delete(fileBannerPath);

                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        // Xóa file trong thư mục temp
                        if (!string.IsNullOrEmpty(fileBackGroundPath))
                            System.IO.File.Delete(fileBackGroundPath);

                        if (!string.IsNullOrEmpty(fileLogoPath))
                            System.IO.File.Delete(fileLogoPath);
                        if (!string.IsNullOrEmpty(fileBannerPath))
                            System.IO.File.Delete(fileBannerPath);

                        // Xóa file trong thư mục upload
                        if (!string.IsNullOrEmpty(folderBackGroundUploadPath))
                            System.IO.File.Delete(folderBackGroundUploadPath);
                        if (!string.IsNullOrEmpty(folderBannerPath))
                            System.IO.File.Delete(folderBannerPath);
                        if (!string.IsNullOrEmpty(folderLogoPath))
                            System.IO.File.Delete(folderLogoPath);
                        throw new Exception("Lỗi trong quá trình xử lý");
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }


    }
}
