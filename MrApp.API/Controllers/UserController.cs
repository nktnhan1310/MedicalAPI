using AutoMapper;
using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public class UserController : BaseController
    {
        private readonly IUserService userService;
        private readonly IUserFileService userFileService;
        public UserController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
        }

        /// <summary>
        /// Lấy thông tin user theo Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-user-info")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserInfo()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.UserId == 0)
                throw new KeyNotFoundException("id không tồn tại");
            var item = await this.userService.GetByIdAsync(LoginContext.Instance.CurrentUser.UserId, e => new Users()
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
                Password = e.Password,
                Gender = e.Gender
            });
            if (item != null)
            {
                var itemModel = mapper.Map<UserModel>(item);
                itemModel.ConfirmPassWord = item.Password;
                var userFiles = await userFileService.GetAsync(e => !e.Deleted && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (userFiles != null)
                    itemModel.UserFiles = mapper.Map<IList<UserFileModel>>(userFiles);
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
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("update-user-info")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateItem([FromBody] UserModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemModel.Id = LoginContext.Instance.CurrentUser.UserId;
                var item = mapper.Map<Users>(itemModel);
                if (itemModel.IsResetPassword)
                    item.Password = SecurityUtils.HashSHA1(itemModel.NewPassWord);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.userService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {

                            string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);
                            // ------- START GET URL FOR FILE
                            string folderUploadPath = string.Empty;
                            var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                            folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.USER_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                
                                FileUtils.CreateDirectory(folderUploadPath);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, CoreContants.USER_FOLDER_NAME, Path.GetFileName(filePath));
                                // ------- END GET URL FOR FILE
                                filePaths.Add(filePath);
                                file.Created = DateTime.Now;
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.UserId = item.Id;
                                file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                                file.FileUrl = fileUrl;
                            }
                            else
                            {
                                file.Updated = DateTime.Now;
                                file.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                            }
                        }
                    }
                    success = await this.userService.UpdateAsync(item);
                    if (success)
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                        // Remove file trong thư mục temp
                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                    }
                    else
                    {
                        if (folderUploadPaths.Any())
                        {
                            foreach (var folderUploadPath in folderUploadPaths)
                            {
                                System.IO.File.Delete(folderUploadPath);
                            }
                        }
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

        /// <summary>
        /// Cập nhật 1 phần user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> PatchItem(int id, [FromBody] UserModel itemModel)
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
                success = await this.userService.UpdateFieldAsync(item, includeProperties);
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
