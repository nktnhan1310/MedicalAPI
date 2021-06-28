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
    [Route("api/medical-record-detail")]
    [ApiController]
    [Description("Chi tiết hồ sơ bệnh án")]
    [Authorize]
    public class MedicalRecordDetailController : BaseController
    {
        private IMedicalRecordDetailService medicalRecordDetailService;
        private IMedicalRecordDetailFileService medicalRecordDetailFileService;
        public MedicalRecordDetailController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            medicalRecordDetailService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            medicalRecordDetailFileService = serviceProvider.GetRequiredService<IMedicalRecordDetailFileService>();
        }

        /// <summary>
        /// Lấy danh sách phân trang chi tiết hồ sơ
        /// </summary>
        /// <param name="searchMedicalRecordDetail"></param>
        /// <returns></returns>
        [HttpGet("get-paged-list-record-detail")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetPagedListRecordDetail([FromQuery] SearchMedicalRecordDetail searchMedicalRecordDetail)
        {
            if (LoginContext.Instance.CurrentUser == null)
                throw new UnauthorizedAccessException("User không tồn tại");
            searchMedicalRecordDetail.UserId = LoginContext.Instance.CurrentUser.UserId;
            var pagedListItem = await this.medicalRecordDetailService.GetPagedListData(searchMedicalRecordDetail);
            return new AppDomainResult()
            {
                Data = mapper.Map<PagedList<MedicalRecordDetailModel>>(pagedListItem),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy tất cả chi tiết của hồ sơ bệnh án của user
        /// </summary>
        /// <param name="medicalRecordId"></param>
        /// <returns></returns>
        [HttpGet("get-all-record-detail/medicalRecordId")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetAllRecordDetail(int? medicalRecordId)
        {
            SearchMedicalRecordDetail searchMedicalRecordDetail = new SearchMedicalRecordDetail()
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                UserId = LoginContext.Instance.CurrentUser.UserId,
                MedicalRecordId = medicalRecordId,
                OrderBy = "Id desc"
            };
            var pagedListItem = await this.medicalRecordDetailService.GetPagedListData(searchMedicalRecordDetail);
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<MedicalRecordDetailModel>>(pagedListItem.Items),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin chi tiết của chi tiết hồ sơ bệnh án
        /// </summary>
        /// <param name="recordDetailId"></param>
        /// <returns></returns>
        [HttpGet("get-record-detail-info/{recordDetailId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetRecordDetailInfo(int recordDetailId)
        {
            SearchMedicalRecordDetail searchMedicalRecordDetail = new SearchMedicalRecordDetail()
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                UserId = LoginContext.Instance.CurrentUser.UserId,
                MedicalRecordDetailId = recordDetailId,
                OrderBy = "Id desc"
            };
            var pagedListItem = await this.medicalRecordDetailService.GetPagedListData(searchMedicalRecordDetail);
            if (pagedListItem == null || !pagedListItem.Items.Any())
                throw new AppException("Không tìm thấy item");
            var medicalRecordDetailInfo = pagedListItem.Items.FirstOrDefault();
            medicalRecordDetailInfo.MedicalRecordDetailFiles = await this.medicalRecordDetailFileService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == medicalRecordDetailInfo.Id);
            return new AppDomainResult()
            {
                Data = mapper.Map<MedicalRecordDetailModel>(medicalRecordDetailInfo),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật thông tin file chi tiết hồ sơ bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailId"></param>
        /// <param name="updateMedicalRecordDetail"></param>
        /// <returns></returns>
        [HttpPut("update-medical-record-detail-multiple-files/{medicalRecordDetailId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMedicalRecordDetailFile(int medicalRecordDetailId, [FromBody] UpdateMedicalRecordDetail updateMedicalRecordDetail)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            List<string> filePaths = new List<string>();
            List<string> folderUploadPaths = new List<string>();
            if (updateMedicalRecordDetail != null && updateMedicalRecordDetail.MedicalRecordDetailFiles != null && updateMedicalRecordDetail.MedicalRecordDetailFiles.Any())
            {
                foreach (var file in updateMedicalRecordDetail.MedicalRecordDetailFiles)
                {
                    string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);
                    // ------- START GET URL FOR FILE
                    string folderUploadPath = string.Empty;
                    var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                    folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME);
                    string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
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
                        file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                        file.FileUrl = fileUrl;
                    }
                    else
                    {
                        file.Updated = DateTime.Now;
                        file.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    }
                }
                success = await this.medicalRecordDetailService.UpdateMedicalRecordDetailFileAsync(medicalRecordDetailId, updateMedicalRecordDetail.MedicalRecordDetailFiles);
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
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin file chi tiết hồ sơ bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailFileId"></param>
        /// <param name="medicalRecordDetailFileModel"></param>
        /// <returns></returns>
        [HttpPut("update-medical-record-detail-file/{medicalRecordDetailFileId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMedicalRecordDetailFile(int medicalRecordDetailFileId, [FromBody] MedicalRecordDetailFileModel medicalRecordDetailFileModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            List<string> filePaths = new List<string>();
            List<string> folderUploadPaths = new List<string>();

            if (medicalRecordDetailFileModel != null)
            {
                string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, medicalRecordDetailFileModel.FileName);
                // ------- START GET URL FOR FILE
                string folderUploadPath = string.Empty;
                var folderUpload = configuration.GetValue<string>("MySettings:FolderUpload");
                folderUploadPath = Path.Combine(folderUpload, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME);
                string fileUploadPath = Path.Combine(folderUploadPath, Path.GetFileName(filePath));
                if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                {
                    FileUtils.CreateDirectory(folderUploadPath);
                    FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                    folderUploadPaths.Add(fileUploadPath);
                    string fileUrl = Path.Combine(CoreContants.UPLOAD_FOLDER_NAME, CoreContants.MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
                    // ------- END GET URL FOR FILE
                    filePaths.Add(filePath);
                    medicalRecordDetailFileModel.Updated = DateTime.Now;
                    medicalRecordDetailFileModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    medicalRecordDetailFileModel.Active = true;
                    medicalRecordDetailFileModel.Deleted = false;
                    medicalRecordDetailFileModel.FileName = Path.GetFileName(filePath);
                    medicalRecordDetailFileModel.FileExtension = Path.GetExtension(filePath);
                    medicalRecordDetailFileModel.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                    medicalRecordDetailFileModel.FileUrl = fileUrl;
                }
                var medicalRecordDetailFile = mapper.Map<MedicalRecordDetailFiles>(medicalRecordDetailFileModel);
                success = await this.medicalRecordDetailFileService.UpdateAsync(medicalRecordDetailFile);
            }
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
            return appDomainResult;
        }

        /// <summary>
        /// Xóa file chi tiết hồ sơ bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailFileId"></param>
        /// <returns></returns>
        [HttpDelete("delete-medical-record-detail-file/{medicalRecordDetailFileId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteMedicalRecordDetailFile(int medicalRecordDetailFileId)
        {
            bool success = await this.medicalRecordDetailFileService.DeleteAsync(medicalRecordDetailFileId);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
