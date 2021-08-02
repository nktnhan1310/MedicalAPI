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
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MedicalAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Description("Quản lý thông tin người dùng")]
    [Authorize]
    public class UserController : CoreHospitalController<Users, UserModel, SearchUser>
    {
        private readonly IUserService userService;
        private readonly IUserInGroupService userInGroupService;
        private readonly IUserFileService userFileService;
        private IConfiguration configuration;
        public UserController(IServiceProvider serviceProvider, ILogger<UserController> logger, IWebHostEnvironment env, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            userInGroupService = serviceProvider.GetRequiredService<IUserInGroupService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
            this.configuration = configuration;
        }

        /// <summary>
        /// Lấy thông tin danh sách user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("get-list-user")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserInfos([FromQuery] string userName)
        {
            var users = await this.userService.GetAsync(e => !e.Deleted
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.Id == LoginContext.Instance.CurrentUser.HospitalId)
            && (string.IsNullOrEmpty(userName) || (e.UserName.Contains(userName) || e.LastName.Contains(userName) || e.FirstName.Contains(userName)))
            , e => new Users()
            {
                Id = e.Id,
                UserName = e.UserName,
                FirstName = e.FirstName,
                LastName = e.LastName,
            });
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<UserModel>>(users),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin all user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> Get()
        {
            var items = await this.domainService.GetAsync(e => !e.Deleted
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId.Value));
            var itemModels = mapper.Map<IList<UserModel>>(items);
            return new AppDomainResult()
            {
                Success = true,
                Data = itemModels,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.domainService.GetByIdAsync(id);

            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<UserModel>(item);
                    itemModel.ConfirmPassWord = item.Password;
                    var userInGroups = await this.userInGroupService.GetAsync(e => !e.Deleted && e.UserId == id);
                    if (userInGroups != null)
                        itemModel.UserGroupIds = userInGroups.Select(e => e.UserGroupId).ToList();
                    var userFiles = await this.userFileService.GetAsync(e => !e.Deleted && e.UserId == id);
                    if (userFiles != null)
                        itemModel.UserFiles = mapper.Map<IList<UserFileModel>>(userFiles);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Không có quyền truy cập");
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
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
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] UserModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<Users>(itemModel);
                if (itemModel.IsResetPassword)
                    item.Password = SecurityUtils.HashSHA1(itemModel.NewPassWord);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            // ------- START GET URL FOR FILE
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));
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
                    success = await this.domainService.UpdateAsync(item);
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
        /// Thêm mới user
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] UserModel itemModel)
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
                var item = mapper.Map<Users>(itemModel);
                item.Password = SecurityUtils.HashSHA1(itemModel.Password);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));


                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));
                                filePaths.Add(filePath);
                                file.Created = DateTime.Now;
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.UserId = item.Id;
                                file.FileUrl = fileUrl;
                            }
                        }
                    }
                    success = await this.domainService.CreateAsync(item);
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
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
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

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> GetPagedData([FromQuery] SearchUser baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                PagedList<Users> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<UserModel> pagedDataModel = mapper.Map<PagedList<UserModel>>(pagedData);
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

        #region Contants

        public const string USER_FOLDER_NAME = "user";

        #endregion

    }
}
