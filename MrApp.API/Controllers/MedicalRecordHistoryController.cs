using AutoMapper;
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
    [Route("api/medical-record-history")]
    [ApiController]
    [Authorize]
    [Description("Quản lý tiền sử khám/ tiền sử phẫu thuật")]
    public class MedicalRecordHistoryController : BaseController
    {
        protected IMedicalRecordHistoryService medicalRecordHistoryService;
        protected IMedicalRecordService medicalRecordService;
        protected IUserService userService;
        protected IUserFileService userFileService;
        protected IUserFolderService userFolderService;
        public MedicalRecordHistoryController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            medicalRecordHistoryService = serviceProvider.GetRequiredService<IMedicalRecordHistoryService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            userFolderService = serviceProvider.GetRequiredService<IUserFolderService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
        }

        /// <summary>
        /// Lấy thông tin danh sách phân trang tiền sử/tiền sử phẫu thuật
        /// </summary>
        /// <param name="searchmedicalrecordHistory"></param>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> Get([FromQuery] SearchMedicalRecordHistory searchmedicalrecordHistory)
        {
            searchmedicalrecordHistory.UserId = LoginContext.Instance.CurrentUser.UserId;
            var medicalRecordInfos = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (medicalRecordInfos == null || !medicalRecordInfos.Any())
                throw new AppException("Chưa tạo hồ sơ khám bệnh");
            var pagedListModel = new PagedList<MedicalRecordHistoryModel>();
            var pagedList = await this.medicalRecordHistoryService.GetPagedListData(searchmedicalrecordHistory);
            if (pagedList != null)
                pagedListModel = mapper.Map<PagedList<MedicalRecordHistoryModel>>(pagedList);
            return new AppDomainResult
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK,
            };
        }

        /// <summary>
        /// Lấy thông tin tiền sử/tiền sử phẫu thuật
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetByIdAsync(int id)
        {
            MedicalRecordHistoryModel medicalRecordHistoryModel = null;
            var medicalRecordHistoryInfo = await this.medicalRecordHistoryService.GetSingleAsync(e => !e.Deleted && e.Active
            && e.Id == id
            && e.UserId == LoginContext.Instance.CurrentUser.UserId
            );
            if (medicalRecordHistoryInfo != null)
            {
                medicalRecordHistoryModel = mapper.Map<MedicalRecordHistoryModel>(medicalRecordHistoryInfo);
                var userInfo = await this.userService.GetByIdAsync(LoginContext.Instance.CurrentUser.UserId);
                if (userInfo != null)
                {
                    medicalRecordHistoryModel.UserFullName = userInfo.LastName + " " + userInfo.FirstName;
                    medicalRecordHistoryModel.UserPhone = userInfo.Phone;
                    medicalRecordHistoryModel.UserEmail = userInfo.Email;
                }
                var userFiles = await this.userFileService.GetAsync(e => !e.Deleted && e.MedicalRecordHistoryId == medicalRecordHistoryInfo.Id);
                medicalRecordHistoryModel.UserFiles = mapper.Map<IList<UserFileModel>>(userFiles);
            }
            return new AppDomainResult
            {
                Success = true,
                Data = medicalRecordHistoryModel,
                ResultCode = (int)HttpStatusCode.OK,
            };
        }

        /// <summary>
        /// Thêm mới tiền sử/tiền sử phẫu thuật
        /// </summary>
        /// <param name="medicalRecordHistoryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddNew([FromBody] MedicalRecordHistoryModel medicalRecordHistoryModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                var medicalRecordInfos = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (medicalRecordInfos == null || !medicalRecordInfos.Any())
                    throw new AppException("Chưa tạo thông tin hồ sơ bệnh án");
                var itemUpdate = mapper.Map<MedicalRecordHistories>(medicalRecordHistoryModel);
                itemUpdate.Created = DateTime.Now;
                itemUpdate.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
                itemUpdate.MedicalRecordId = medicalRecordInfos.FirstOrDefault().Id;
                itemUpdate.Active = true;
                itemUpdate.Deleted = false;
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                // Lấy thông tin folder khác (mặc định)
                var userFolderOtherInfo = await this.userFolderService.GetSingleAsync(e => e.TypeId == (int)CatalogueUtilities.FolderType.Other);
                if (itemUpdate.UserFiles != null && itemUpdate.UserFiles.Any())
                {
                    foreach (var file in itemUpdate.UserFiles)
                    {
                        string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);
                        // ------- START GET URL FOR FILE
                        string folderUploadPath = string.Empty;
                        var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                        folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME);
                        string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
                        // Kiểm tra có tồn tại file trong temp chưa?
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            FileUtils.CreateDirectory(folderUploadPath);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                            folderUploadPaths.Add(fileUploadPath);
                            string fileUrl = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
                            // ------- END GET URL FOR FILE
                            filePaths.Add(filePath);
                            file.Created = DateTime.Now;
                            file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                            file.Active = true;
                            file.Deleted = false;
                            file.FileName = Path.GetFileName(filePath);
                            file.FileExtension = Path.GetExtension(filePath);
                            file.MedicalRecordId = itemUpdate.Id;
                            file.UserId = LoginContext.Instance.CurrentUser.UserId;
                            file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                            file.FileUrl = fileUrl;
                        }
                        if (userFolderOtherInfo != null) file.FolderId = userFolderOtherInfo.Id;
                    }
                }
                success = await this.medicalRecordHistoryService.CreateAsync(itemUpdate);
                // Remove file trong thư mục temp
                if (filePaths.Any())
                {
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                if (success)
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                else
                {
                    // Lỗi => remove file trong folder upload
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
            else throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật tiền sử/tiền sử phẫu thuật
        /// </summary>
        /// <param name="medicalRecordHistoryModel"></param>
        /// <returns></returns>
        [HttpPut]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> Update([FromBody] MedicalRecordHistoryModel medicalRecordHistoryModel)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                var medicalRecordInfos = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active
                && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (medicalRecordInfos == null || !medicalRecordInfos.Any())
                    throw new AppException("Thông tin hồ sơ bệnh án không chính xác");

                var existItems = await this.medicalRecordHistoryService.GetAsync(e => !e.Deleted && e.Active && e.Id == medicalRecordHistoryModel.Id && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (existItems == null || !existItems.Any())
                    throw new AppException("Không tìm thấy thông tin tiền sử");

                var itemUpdate = mapper.Map<MedicalRecordHistories>(medicalRecordHistoryModel);
                itemUpdate.Updated = DateTime.Now;
                itemUpdate.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemUpdate.UserId = LoginContext.Instance.CurrentUser.UserId;
                itemUpdate.MedicalRecordId = medicalRecordInfos.FirstOrDefault().Id;
                itemUpdate.Active = true;
                // Lấy thông tin folder khác (mặc định)
                var userFolderOtherInfo = await this.userFolderService.GetSingleAsync(e => e.TypeId == (int)CatalogueUtilities.FolderType.Other);

                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                if (itemUpdate.UserFiles != null && itemUpdate.UserFiles.Any())
                {
                    foreach (var file in itemUpdate.UserFiles)
                    {
                        string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);
                        // ------- START GET URL FOR FILE
                        string folderUploadPath = string.Empty;
                        var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                        folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME);
                        string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
                        // Kiểm tra có tồn tại file trong temp chưa?
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                        {
                            FileUtils.CreateDirectory(folderUploadPath);
                            FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                            folderUploadPaths.Add(fileUploadPath);
                            string fileUrl = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
                            // ------- END GET URL FOR FILE
                            filePaths.Add(filePath);
                            file.Created = DateTime.Now;
                            file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                            file.Active = true;
                            file.Deleted = false;
                            file.FileName = Path.GetFileName(filePath);
                            file.FileExtension = Path.GetExtension(filePath);
                            file.MedicalRecordId = itemUpdate.Id;
                            file.UserId = LoginContext.Instance.CurrentUser.UserId;
                            file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                            file.FileUrl = fileUrl;
                        }
                        else
                        {
                            file.Updated = DateTime.Now;
                            file.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                        }
                        if (userFolderOtherInfo != null) file.FolderId = userFolderOtherInfo.Id;
                    }
                }
                success = await this.medicalRecordHistoryService.UpdateAsync(itemUpdate);
                // Remove file trong thư mục temp
                if (filePaths.Any())
                {
                    foreach (var filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                if (success)
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                else
                {
                    // Lỗi => remove file trong folder upload
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
            else throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Xóa thông tin tiền sử/tiền sử phẫu thuật
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> Delete(List<int> itemIds)
        {
            bool success = true;
            if (itemIds != null && itemIds.Any())
            {
                var existItems = await this.medicalRecordHistoryService.GetAsync(e => !e.Deleted && e.Active && itemIds.Contains(e.Id) && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (existItems == null || !existItems.Any())
                    throw new AppException("Không tìm thấy thông tin tiền sử/tiền sử phẫu thuật");
                foreach (var itemId in itemIds)
                {
                    success &= await this.medicalRecordHistoryService.DeleteAsync(itemId);
                }
            }
            else throw new AppException("Không tìm thấy mã tiền sử");
            return new AppDomainResult
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK,
            };
        }
    }
}
