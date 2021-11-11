using AutoMapper;
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

namespace MrApp.API.Controllers
{
    [Route("api/user-folder")]
    [ApiController]
    [Authorize]
    [Description("Quản lý folder của user")]
    public class UserFolderController : BaseController
    {
        protected IUserFolderService userFolderService;
        protected IUserFileService userFileService;
        public UserFolderController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            userFolderService = serviceProvider.GetRequiredService<IUserFolderService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
        }

        /// <summary>
        /// Lấy danh sách folder của user
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-paged-folder")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetFolders([FromQuery] SearchUserFolder baseSearch)
        {
            baseSearch.UserId = LoginContext.Instance.CurrentUser.UserId;
            var userFolders = await this.userFolderService.GetPagedListData(baseSearch);
            var userFolderModels = mapper.Map<PagedList<UserFolderModel>>(userFolders);
            return new AppDomainResult()
            {
                Success = true,
                Data = userFolderModels,
                ResultCode = (int)HttpStatusCode.OK
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
            searchUserFolder.UserId = LoginContext.Instance.CurrentUser.UserId;
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

        /// <summary>
        /// Lấy thông tin folder theo id
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [HttpGet("get-folder-info")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetFolderInfoById([FromQuery] int folderId)
        {
            UserFolderModel userFolderModel = null;
            var folderInfos = await this.userFolderService.GetAsync(e => (!e.UserId.HasValue 
            || e.UserId == LoginContext.Instance.CurrentUser.UserId) && e.Id == folderId);
            if (folderInfos != null && folderInfos.Any())
            {
                userFolderModel = mapper.Map<UserFolderModel>(folderInfos.FirstOrDefault());
                var userFileInfolders = await this.userFileService.GetAsync(e => !e.Deleted && e.Active && e.FolderId == folderId);
                if (userFileInfolders != null)
                    userFolderModel.UserFiles = mapper.Map<IList<UserFileModel>>(userFileInfolders);
            }
            else throw new AppException("Không tìm thấy thông tin folder");
            return new AppDomainResult()
            {
                Success = true,
                Data = userFolderModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }


        /// <summary>
        /// Thêm mới user folder
        /// </summary>
        /// <param name="userFolderModel"></param>
        /// <returns></returns>
        [HttpPost("add-new-user-folder")]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddNewFolder([FromBody] UserFolderModel userFolderModel)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                var userFolder = mapper.Map<UserFolders>(userFolderModel);
                if (userFolder != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.userFolderService.GetExistItemMessage(userFolder);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                }

                userFolder.Active = true;
                userFolder.Deleted = false;
                userFolder.Created = DateTime.Now;
                userFolder.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                userFolder.UserId = LoginContext.Instance.CurrentUser.UserId;
                userFolder.TypeId = 4;
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();

                if (userFolder.UserFiles != null && userFolder.UserFiles.Any())
                {
                    foreach (var file in userFolder.UserFiles)
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
                            file.UserId = LoginContext.Instance.CurrentUser.UserId;
                            file.Active = true;
                            file.Deleted = false;
                            file.FileName = Path.GetFileName(filePath);
                            file.FileExtension = Path.GetExtension(filePath);
                            file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                            file.FileUrl = fileUrl;
                        }
                    }
                }
                success = await this.userFolderService.CreateAsync(userFolder);

                if (success)
                {
                    // Xóa file trong fodler tạm
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    // Xóa file trong fodler tạm
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    // Xóa file trong fodler upload
                    foreach (var folderUploadPath in folderUploadPaths)
                    {
                        System.IO.File.Delete(folderUploadPath);
                    }
                    throw new AppException("Lỗi trong quá trình xử lý");
                }

            }
            else throw new AppException(ModelState.GetErrorMessage());
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin user folder
        /// </summary>
        /// <param name="userFolderModel"></param>
        /// <returns></returns>
        [HttpPut("update-user-folder")]
        public async Task<AppDomainResult> UpdateUserFolder([FromBody] UserFolderModel userFolderModel)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                if (userFolderModel.Id <= 0)
                    throw new AppException("Không tìm thấy folder");
                var folderInfos = await this.userFolderService.GetAsync(e => e.Id == userFolderModel.Id 
                && (e.UserId == LoginContext.Instance.CurrentUser.UserId));
                if (folderInfos == null || !folderInfos.Any())
                    throw new AppException("Không tìm thấy thông tin folder");
                var userFolder = mapper.Map<UserFolders>(userFolderModel);
                if (userFolder != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.userFolderService.GetExistItemMessage(userFolder);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                }

                userFolder.Active = true;
                userFolder.Deleted = false;
                userFolder.Updated = DateTime.Now;
                userFolder.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                userFolder.UserId = LoginContext.Instance.CurrentUser.UserId;
                userFolder.TypeId = 4;

                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();

                if (userFolder.UserFiles != null && userFolder.UserFiles.Any())
                {
                    foreach (var file in userFolder.UserFiles)
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
                            file.UserId = LoginContext.Instance.CurrentUser.UserId;
                            file.Active = true;
                            file.Deleted = false;
                            file.FileName = Path.GetFileName(filePath);
                            file.FileExtension = Path.GetExtension(filePath);
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
                success = await this.userFolderService.UpdateAsync(userFolder);

                if (success)
                {
                    // Xóa file trong fodler tạm
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    // Xóa file trong fodler tạm
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    // Xóa file trong fodler upload
                    foreach (var folderUploadPath in folderUploadPaths)
                    {
                        System.IO.File.Delete(folderUploadPath);
                    }
                    throw new AppException("Lỗi trong quá trình xử lý");
                }

            }
            else throw new AppException(ModelState.GetErrorMessage());
            appDomainResult.Success = success;
            return appDomainResult;
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
            searchUserFile.UserId = LoginContext.Instance.CurrentUser.UserId;
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
            searchUserFile.UserId = LoginContext.Instance.CurrentUser.UserId;
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
        /// Up load file vào folder nếu có
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="userFileModels"></param>
        /// <returns></returns>
        [HttpPost("upload-user-file")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> UploadUserFile(int? folderId, [FromBody] List<UserFileModel> userFileModels)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            List<string> filePaths = new List<string>();
            List<string> folderUploadPaths = new List<string>();
            if (folderId.HasValue)
            {
                var folderInfos = await this.userFolderService.GetAsync(e => e.Id == folderId 
                && (!e.UserId.HasValue || e.UserId == LoginContext.Instance.CurrentUser.UserId));
                if (folderInfos == null || !folderInfos.Any())
                    throw new AppException("Không tìm thấy thông tin folder");
            }


            if (userFileModels != null && userFileModels.Any())
            {
                var userFiles = mapper.Map<List<UserFiles>>(userFileModels);

                foreach (var file in userFiles)
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
                        file.UserId = LoginContext.Instance.CurrentUser.UserId;
                        file.Active = true;
                        file.Deleted = false;
                        file.FileName = Path.GetFileName(filePath);
                        file.FileExtension = Path.GetExtension(filePath);
                        file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                        file.FileUrl = fileUrl;
                        file.FolderId = folderId.HasValue ? folderId.Value : file.FolderId;
                    }
                }
                success = await this.userFileService.CreateAsync(userFiles);
                if (success)
                {
                    // Remove file trong thư mục temp
                    if (filePaths.Any())
                    {
                        foreach (var filePath in filePaths)
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    // Xóa hết file trong thư mục temp
                    if (filePaths.Any())
                    {
                        foreach (var filePath in filePaths)
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    // Xóa file trong thư mục upload
                    if (folderUploadPaths.Any())
                    {
                        foreach (var folderUploadPath in folderUploadPaths)
                        {
                            System.IO.File.Delete(folderUploadPath);
                        }
                    }

                    throw new AppException("Lỗi trong quá trình xử lý");
                }


            }
            else throw new AppException("Không có thông tin file upload");

            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa folder
        /// </summary>
        /// <param name="folderIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-user-folders")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteUserFolder([FromBody] List<int> folderIds)
        {
            bool success = true;
            if (folderIds != null && folderIds.Any())
            {
                var existFolderInfos = await this.userFolderService.GetAsync(e => !e.Deleted && e.Active && folderIds.Contains(e.Id) && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (existFolderInfos == null || !existFolderInfos.Any())
                    throw new AppException("Không tìm thấy thông tin folder");
                foreach (var folderId in folderIds)
                {
                    if (!existFolderInfos.Select(e => e.Id).ToList().Contains(folderId)) continue;
                    success &= await this.userFolderService.DeleteAsync(folderId);
                    // Xóa tất cả file trong folder
                    var userFileInFolders = await this.userFileService.GetAsync(e => !e.Deleted && e.Active 
                    && e.FolderId == folderId && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                    if (userFileInFolders != null && userFileInFolders.Any())
                    {
                        foreach (var userFileInFolder in userFileInFolders)
                        {
                            await this.userFileService.DeleteAsync(userFileInFolder.Id);
                        }
                    }
                }
            }
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa file
        /// </summary>
        /// <param name="fileIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-user-files")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteUserFile([FromBody] List<int> fileIds)
        {
            bool success = false;
            if (fileIds != null && fileIds.Any())
            {
                var existFileInfos = await this.userFileService.GetAsync(e => !e.Deleted && e.Active && fileIds.Contains(e.Id) && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (existFileInfos == null || !existFileInfos.Any())
                    throw new AppException("Không tìm thấy thông tin file");
                foreach (var fileId in fileIds)
                {
                    if (!existFileInfos.Select(e => e.Id).ToList().Contains(fileId)) continue;
                    success = await this.userFileService.DeleteAsync(fileId);
                }
            }
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
