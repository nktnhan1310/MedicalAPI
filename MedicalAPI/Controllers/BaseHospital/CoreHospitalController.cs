using AutoMapper;
using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.Services;
using Medical.Models.DomainModel;
using Medical.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [ApiController]
    public abstract class CoreHospitalController<E, T, F> : ControllerBase where E : MedicalAppDomainHospital where T : MedicalAppDomainHospitalModel where F : BaseHospitalSearch
    {
        protected readonly ILogger<CoreHospitalController<E, T, F>> logger;
        protected readonly IServiceProvider serviceProvider;
        protected readonly IMapper mapper;
        protected IDomainService<E, F> domainService;
        protected IWebHostEnvironment env;
        private readonly IUserService userService;

        public CoreHospitalController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<E, T, F>> logger, IWebHostEnvironment env)
        {
            this.env = env;
            this.logger = logger;
            this.mapper = serviceProvider.GetService<IMapper>();
            this.serviceProvider = serviceProvider;
            userService = serviceProvider.GetRequiredService<IUserService>();
        }

        /// <summary>
        /// Lấy all thông tin danh sách item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public virtual async Task<AppDomainResult> Get()
        {
            var items = await this.domainService.GetAsync(e =>
            !LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId.Value);
            var itemModels = mapper.Map<IList<T>>(items);
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
        public virtual async Task<AppDomainResult> GetById(int id)
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
                    var itemModel = mapper.Map<T>(item);
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
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public virtual async Task<AppDomainResult> AddItem([FromBody] T itemModel)
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
                var item = mapper.Map<E>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.domainService.CreateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
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
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public virtual async Task<AppDomainResult> UpdateItem(int id, [FromBody] T itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<E>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.domainService.UpdateAsync(item);
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
        /// Cập nhật 1 phần item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public virtual async Task<AppDomainResult> PatchItem(int id, [FromBody] T itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
            itemModel.Updated = DateTime.Now;
            itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            var item = mapper.Map<E>(itemModel);
            if (item != null)
            {
                Expression<Func<E, object>>[] includeProperties = new Expression<Func<E, object>>[]
                {
                        x => x.Active,
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

            return appDomainResult;
        }


        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public virtual async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = await this.domainService.DeleteAsync(id);
            if (success)
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");

            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public virtual async Task<AppDomainResult> GetPagedData([FromQuery] F baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null)
                    baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                PagedList<E> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<T> pagedDataModel = mapper.Map<PagedList<T>>(pagedData);
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

        #region Files

        [HttpGet("download-file/{fileId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Download })]
        public virtual async Task<ActionResult> DownloadFile(int fileId)
        {
            await Task.Run(() =>
            {
            });
            throw new Exception("Error Download");
        }

        /// <summary>
        /// Upload Single File
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("upload-file")]
        [MedicalAppAuthorize(new string[] { CoreContants.Upload })]
        public virtual async Task<AppDomainResult> UploadFile(IFormFile file)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            await Task.Run(() =>
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                    string fileUploadPath = Path.Combine(env.ContentRootPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME);
                    string path = Path.Combine(fileUploadPath, fileName);
                    FileUtils.CreateDirectory(fileUploadPath);
                    var fileByte = FileUtils.StreamToByte(file.OpenReadStream());
                    FileUtils.SaveToPath(path, fileByte);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = fileName
                    };
                }
            });
            return appDomainResult;
        }

        /// <summary>
        /// Upload Multiple File
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("upload-multiple-files")]
        [MedicalAppAuthorize(new string[] { CoreContants.Upload })]
        public virtual async Task<AppDomainResult> UploadFiles(List<IFormFile> files)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            await Task.Run(() =>
            {
                if (files != null && files.Any())
                {
                    List<string> fileNames = new List<string>();
                    foreach (var file in files)
                    {
                        string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                        string fileUploadPath = Path.Combine(env.ContentRootPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME);
                        string path = Path.Combine(fileUploadPath, fileName);
                        FileUtils.CreateDirectory(fileUploadPath);
                        var fileByte = FileUtils.StreamToByte(file.OpenReadStream());
                        FileUtils.SaveToPath(path, fileByte);
                        fileNames.Add(fileName);
                    }
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = fileNames
                    };
                }
            });
            return appDomainResult;
        }


        /// <summary>
        /// Lấy thông tin quyền của chức năng
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-permission-detail")]
        public virtual async Task<AppDomainResult> GetPermission()
        {
            List<int> permissionIds = new List<int>();
            bool isViewAll = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                    , new List<string>() { CoreContants.ViewAll });
            if (isViewAll) permissionIds.Add((int)CoreContants.PermissionContants.ViewAll);

            bool isView = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                    , new List<string>() { CoreContants.View });
            if (isView) permissionIds.Add((int)CoreContants.PermissionContants.View);

            bool isAddNew = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                    , new List<string>() { CoreContants.AddNew });
            if (isAddNew) permissionIds.Add((int)CoreContants.PermissionContants.AddNew);

            bool isUpdate = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                    , new List<string>() { CoreContants.Update });
            if (isUpdate) permissionIds.Add((int)CoreContants.PermissionContants.Update);

            bool isDelete = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                    , new List<string>() { CoreContants.Delete });
            if (isDelete) permissionIds.Add((int)CoreContants.PermissionContants.Delete);


            bool isDownload = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                    , new List<string>() { CoreContants.Download });
            if (isDownload) permissionIds.Add((int)CoreContants.PermissionContants.Download);


            bool isExport = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                    , new List<string>() { CoreContants.Export });
            if (isExport) permissionIds.Add((int)CoreContants.PermissionContants.Export);


            bool isImport = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                   , new List<string>() { CoreContants.Import });
            if (isImport) permissionIds.Add((int)CoreContants.PermissionContants.Import);


            bool isUpload = await this.userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, ControllerContext.ActionDescriptor.ControllerName
                   , new List<string>() { CoreContants.Upload });
            if (isUpload) permissionIds.Add((int)CoreContants.PermissionContants.Upload);

            return new AppDomainResult()
            {
                Data = permissionIds,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #endregion

        #region Contants

        public const string TEMP_FOLDER_NAME = "Temp";
        public const string UPLOAD_FOLDER_NAME = "upload";

        #endregion


    }
}
