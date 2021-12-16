﻿using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Interface.Services.Base;
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

namespace Medical.Core.App.Controllers
{
    [ApiController]
    public abstract class CatalogueCoreHospitalController<E, T, DomainSearch> : CoreHospitalController<E, T, DomainSearch> where E : MedicalCatalogueAppDomainHospital where T : MedicalCatalogueAppDomainHospitalModel where DomainSearch : BaseHospitalSearch, new()
    {
        protected ICatalogueHospitalService<E, DomainSearch> catalogueService;

        public CatalogueCoreHospitalController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<E, T, DomainSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
        }

        /// <summary>
        /// Lấy all thông tin danh sách item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> Get()
        {
            DomainSearch baseSearch = new DomainSearch();
            IList<T> itemModels = new List<T>();
            baseSearch.PageIndex = 1;
            baseSearch.PageSize = int.MaxValue;
            baseSearch.OrderBy = "Id";
            baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;

            var pagedItems = await this.catalogueService.GetPagedListData(baseSearch);
            if (pagedItems != null && pagedItems.Items != null && pagedItems.Items.Any())
                itemModels = mapper.Map<IList<T>>(pagedItems.Items);
            //var items = await this.catalogueService.GetAsync(e =>
            //!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId
            //);
            //var itemModels = mapper.Map<IList<T>>(items);
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

            var item = await this.catalogueService.GetByIdAsync(id);
            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<T>(item);
                    if (itemModel.HospitalId.HasValue && itemModel.HospitalId.Value > 0)
                    {
                        var hospitalInfos = await this.hospitalService.GetAsync(e => e.Id == itemModel.HospitalId.Value);
                        if (hospitalInfos != null && hospitalInfos.Any()) itemModel.HospitalName = hospitalInfos.FirstOrDefault().Name;
                    }
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
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] T itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Active = true;
                itemModel.Deleted = false;
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<E>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.catalogueService.CreateAsync(item);
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
        /// Thêm mới nhiều item
        /// </summary>
        /// <param name="itemModels"></param>
        /// <returns></returns>
        [HttpPost("add-multiple-item")]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddMultipleItem([FromBody] IList<T> itemModels)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var items = mapper.Map<IList<E>>(itemModels);
            foreach (var item in items)
            {
                item.Created = DateTime.Now;
                item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                    item.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                // Kiểm tra item có tồn tại chưa?
                var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                if (!string.IsNullOrEmpty(messageUserCheck))
                    throw new AppException(messageUserCheck);

            }
            success = await this.catalogueService.CreateAsync(items);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin list item
        /// </summary>
        /// <param name="itemModels"></param>
        /// <returns></returns>
        [HttpPut("update-multiple-item")]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> UpdateMultipleItem([FromBody] IList<T> itemModels)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var items = mapper.Map<IList<E>>(itemModels);
            foreach (var item in items)
            {
                item.Updated = DateTime.Now;
                item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                    item.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                // Kiểm tra item có tồn tại chưa?
                var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                if (!string.IsNullOrEmpty(messageUserCheck))
                    throw new AppException(messageUserCheck);

            }
            success = await this.catalogueService.UpdateAsync(items);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = success;
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
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] T itemModel)
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
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.catalogueService.UpdateAsync(item);
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
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = await this.catalogueService.DeleteAsync(id);
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
        /// Xóa nhiều lựa chọn
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-multiples")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> Delete([FromBody] List<int> itemIds)
        {
            if (itemIds == null || !itemIds.Any()) throw new AppException("Không tìm thấy thông tin id");

            var existItems = await this.catalogueService.GetAsync(e => !e.Deleted && itemIds.Contains(e.Id)
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
            );
            if (existItems == null || !existItems.Any()) throw new AppException("Không tìm thấy thông tin item");
            bool success = true;
            success &= await this.catalogueService.DeleteAsync(itemIds);
            if (!success) throw new Exception("Lỗi trong quá trình xử lý");

            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> GetPagedData([FromQuery] DomainSearch baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                PagedList<E> pagedData = await this.catalogueService.GetPagedListData(baseSearch);
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

        /// <summary>
        /// Down load template file import
        /// </summary>
        /// <returns></returns>
        [HttpGet("download-template-import")]
        [MedicalAppAuthorize(new string[] { CoreContants.Download })]
        public virtual async Task<ActionResult> DownloadTemplateImport()
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, TEMPLATE_FOLDER_NAME, CATALOGUE_HOSPITAL_TEMPLATE_NAME);
            if (!System.IO.File.Exists(path))
                throw new AppException("File template không tồn tại!");
            var file = await System.IO.File.ReadAllBytesAsync(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateImport.xlsx");
        }

        /// <summary>
        /// Tải về file kết quả sau khi import
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("download-import-result-file/{fileName}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Download })]
        public virtual async Task<ActionResult> DownloadImportFileResult(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name không tồn tại!");
            if (env == null)
                throw new Exception("IHostingEnvironment is null => inject to constructor");
            var webRoot = env.ContentRootPath;
            string path = Path.Combine(webRoot, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, fileName);
            var file = await System.IO.File.ReadAllBytesAsync(path);
            // Xóa file thư mục temp
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("KetQuaCD-{0:dd-MM-yyyy_HH_mm_ss}{1}", DateTime.Now, Path.GetExtension(fileName)));
        }

        /// <summary>
        /// Import file danh mục
        /// </summary>
        /// <param name="hostpialId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("import-template-file")]
        [MedicalAppAuthorize(new string[] { CoreContants.Import })]
        public virtual async Task<AppDomainImportResult> ImportTemplateFile(int? hostpialId, IFormFile file)
        {
            AppDomainImportResult appDomainImportResult = new AppDomainImportResult();
            var fileStream = file.OpenReadStream();
            if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                hostpialId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            appDomainImportResult = await this.catalogueService.ImportTemplateFile(fileStream, hostpialId);
            if (appDomainImportResult.ResultFile != null)
            {
                var webRoot = env.ContentRootPath;
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string path = Path.Combine(webRoot, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, fileName);
                FileUtils.CreateDirectory(Path.Combine(webRoot, TEMP_FOLDER_NAME));
                FileUtils.SaveToPath(path, appDomainImportResult.ResultFile);
                appDomainImportResult.ResultFile = null;
                appDomainImportResult.DownloadFileName = fileName;
            }
            return appDomainImportResult;
        }


        #region Contants

        public const string TEMPLATE_FOLDER_NAME = "Template";
        public const string CATALOGUE_TEMPLATE_NAME = "CatalogueTemplate.xlsx";
        public const string CATALOGUE_HOSPITAL_TEMPLATE_NAME = "CatalogueHospitalTemplate.xlsx";

        #endregion
    }
}