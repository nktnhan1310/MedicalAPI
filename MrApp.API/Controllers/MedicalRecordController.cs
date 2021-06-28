using AutoMapper;
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

namespace MrApp.API.Controllers
{
    [Route("api/medical-record")]
    [ApiController]
    [Description("Hồ sơ bệnh án")]
    [Authorize]
    public class MedicalRecordController : BaseController
    {
        private readonly IMedicalRecordAdditionService medicalRecordAdditionService;
        private readonly IMedicalRecordService medicalRecordService;
        private readonly IMedicalRecordFileService medicalRecordFileService;
        private readonly IMedicalRecordDetailService medicalRecordDetailService;
        private readonly IMedicalRecordDetailFileService medicalRecordDetailFileService;


        public MedicalRecordController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            this.medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            medicalRecordAdditionService = serviceProvider.GetRequiredService<IMedicalRecordAdditionService>();
            medicalRecordFileService = serviceProvider.GetRequiredService<IMedicalRecordFileService>();
            medicalRecordDetailService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            medicalRecordDetailFileService = serviceProvider.GetRequiredService<IMedicalRecordDetailFileService>();
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.medicalRecordService.GetByIdAsync(id);

            if (item != null && item.UserId == LoginContext.Instance.CurrentUser.UserId)
            {
                if ((!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<MedicalRecordModel>(item);
                    // Lấy thông tin người thân
                    var medicalAdditions = await this.medicalRecordAdditionService.GetAsync(e => !e.Deleted && e.MedicalRecordId == id);
                    if (medicalAdditions != null)
                        itemModel.MedicalRecordAdditions = mapper.Map<IList<MedicalRecordAdditionModel>>(medicalAdditions);
                    // Lấy thông tin file hồ sơ bệnh án
                    var medicalFiles = await this.medicalRecordFileService.GetAsync(e => !e.Deleted && e.MedicalRecordId == id);
                    if (medicalFiles != null)
                        itemModel.MedicalRecordFiles = mapper.Map<IList<MedicalRecordFileModel>>(medicalFiles);

                    // Lấy thông tin chi tiết hồ sơ bệnh án
                    var medicalRecordDetails = await this.medicalRecordDetailService.GetAsync(e => !e.Deleted && e.MedicalRecordId == id);
                    if (medicalRecordDetails != null)
                    {
                        itemModel.MedicalRecordDetails = mapper.Map<IList<MedicalRecordDetailModel>>(medicalRecordDetails);
                        // Lấy thông tin file của hồ sơ bệnh án
                        if(itemModel.MedicalRecordDetails.Any())
                        {
                            foreach (var detail in itemModel.MedicalRecordDetails)
                            {
                                var detailFiles = await this.medicalRecordDetailFileService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == detail.Id);
                                detail.MedicalRecordDetailFiles = mapper.Map<IList<MedicalRecordDetailFileModel>>(detailFiles);
                            }
                        }    
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
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateItem(int id, [FromBody] MedicalRecordModel itemModel)
        {
            if (LoginContext.Instance.CurrentUser == null || LoginContext.Instance.CurrentUser.UserId != itemModel.UserId)
                throw new UnauthorizedAccessException("Không có quyền truy cập");
            itemModel.UserId = LoginContext.Instance.CurrentUser.UserId;
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<MedicalRecords>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.medicalRecordService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.MedicalRecordFiles != null && item.MedicalRecordFiles.Any())
                    {
                        foreach (var file in item.MedicalRecordFiles)
                        {
                            string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);
                            // ------- START GET URL FOR FILE
                            string folderUploadPath = string.Empty;
                            var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                            folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                
                                FileUtils.CreateDirectory(folderUploadPath);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
                                // ------- END GET URL FOR FILE
                                filePaths.Add(filePath);
                                file.Created = DateTime.Now;
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.MedicalRecordId = item.Id;
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
                    success = await this.medicalRecordService.UpdateAsync(item);
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
        /// Thêm mới hồ sơ bệnh án
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddItem([FromBody] MedicalRecordModel itemModel)
        {
            if (LoginContext.Instance.CurrentUser != null)
                itemModel.UserId = LoginContext.Instance.CurrentUser.UserId;
            else throw new UnauthorizedAccessException("Không có quyền truy cập");
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemModel.Active = true;
                var item = mapper.Map<MedicalRecords>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.medicalRecordService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.MedicalRecordFiles != null && item.MedicalRecordFiles.Any())
                    {
                        foreach (var file in item.MedicalRecordFiles)
                        {
                            string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadPath = string.Empty;
                            var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                            folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                
                                FileUtils.CreateDirectory(folderUploadPath);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
                                filePaths.Add(filePath);
                                file.Created = DateTime.Now;
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.MedicalRecordId = item.Id;
                                file.FileUrl = fileUrl;
                            }
                        }
                    }
                    success = await this.medicalRecordService.CreateAsync(item);
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
                    throw new AppException("Item không tồn tại");
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin danh sách hồ sơ bệnh án theo user đăng nhập
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetPagedData([FromQuery] SearchMedicalRecord baseSearch)
        {
            if (LoginContext.Instance.CurrentUser != null)
                baseSearch.UserId = LoginContext.Instance.CurrentUser.UserId;
            else throw new UnauthorizedAccessException("Không có quyền truy cập");
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                PagedList<MedicalRecords> pagedData = await this.medicalRecordService.GetPagedListData(baseSearch);
                PagedList<MedicalRecordModel> pagedDataModel = mapper.Map<PagedList<MedicalRecordModel>>(pagedData);
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
        /// Xóa hồ sơ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteItem(int id)
        {
            var existMedicalRecords = await this.medicalRecordService.GetAsync(e => e.Id == id, e => new MedicalRecords() { UserId = e.UserId });
            if (LoginContext.Instance.CurrentUser == null
                || (existMedicalRecords != null && existMedicalRecords.Any() && LoginContext.Instance.CurrentUser.UserId != existMedicalRecords.FirstOrDefault().UserId))
                throw new UnauthorizedAccessException("Không có quyền truy cập");

            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = await this.medicalRecordService.DeleteAsync(id);
            if (success)
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");

            return appDomainResult;
        }


        #region Contants

        public const string MEDICAL_RECORD_FOLDER_NAME = "medicalrecord";

        #endregion

    }
}
