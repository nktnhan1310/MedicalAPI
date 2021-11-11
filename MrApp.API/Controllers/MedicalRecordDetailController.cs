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
using System.Linq.Expressions;
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
        private IExaminationFormDetailService examinationFormDetailService;
        private IMedicalBillService medicalBillService;
        private IMedicalRecordService medicalRecordService;
        private IUserFileService userFileService;

        public MedicalRecordDetailController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            medicalRecordDetailService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            medicalRecordDetailFileService = serviceProvider.GetRequiredService<IMedicalRecordDetailFileService>();
            examinationFormDetailService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            medicalBillService = serviceProvider.GetRequiredService<IMedicalBillService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
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
            medicalRecordDetailInfo.UserFiles = await this.userFileService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == medicalRecordDetailInfo.Id);
            if (medicalRecordDetailInfo.UserFiles != null && medicalRecordDetailInfo.UserFiles.Any())
                medicalRecordDetailInfo.UserFiles = medicalRecordDetailInfo.UserFiles.OrderByDescending(e => e.Id).ToList();
            medicalRecordDetailInfo.MedicalBills = await this.medicalBillService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == medicalRecordDetailInfo.Id);
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
        /// <param name="updateMedicalRecordDetail"></param>
        /// <returns></returns>
        [HttpPost("update-medical-record-detail-multiple-files/{medicalRecordDetailId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMedicalRecordDetailFile([FromBody] UpdateMedicalRecordDetail updateMedicalRecordDetail)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            List<string> filePaths = new List<string>();
            List<string> folderUploadPaths = new List<string>();
            if (updateMedicalRecordDetail == null || updateMedicalRecordDetail.UserFiles != null || updateMedicalRecordDetail.UserFiles.Any())
                throw new AppException("Thông tin file không hợp lệ");
            foreach (var file in updateMedicalRecordDetail.UserFiles)
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
            success = await this.medicalRecordDetailService.UpdateMedicalRecordDetailFileAsync(updateMedicalRecordDetail.MedicalRecordDetailId, updateMedicalRecordDetail.UserFiles);
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
        /// Cập nhật thông tin file chi tiết hồ sơ bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailFileId"></param>
        /// <param name="medicalRecordDetailFileModel"></param>
        /// <returns></returns>
        [HttpPost("update-medical-record-detail-file/{medicalRecordDetailFileId}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMedicalRecordDetailFile([FromBody] MedicalRecordDetailFileModel medicalRecordDetailFileModel)
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
        /// Lấy danh sách dịch vụ phát sinh trong hồ sơ bệnh án
        /// </summary>
        /// <param name="searchExaminationFormDetail"></param>
        /// <returns></returns>
        [HttpGet("get-examination-form-detail")]
        public async Task<AppDomainResult> GetExaminationFormDetails([FromQuery] SearchExaminationFormDetail searchExaminationFormDetail)
        {
            searchExaminationFormDetail.UserId = LoginContext.Instance.CurrentUser.UserId;
            searchExaminationFormDetail.IsFromMedicalRecordDetail = true;
            var pagedData = await this.examinationFormDetailService.GetPagedListData(searchExaminationFormDetail);
            return new AppDomainResult()
            {
                Data = mapper.Map<PagedList<ExaminationFormDetailModel>>(pagedData),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Thêm mới tiểu sử bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailModel"></param>
        /// <returns></returns>
        [HttpPost("add-medical-record-detail")]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public async Task<AppDomainResult> AddItem([FromBody] MedicalRecordDetailModel medicalRecordDetailModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                // Lấy thông tin hồ sơ của user
                var userMedicalRecordInfos = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (userMedicalRecordInfos != null && userMedicalRecordInfos.Any())
                {
                    medicalRecordDetailModel.MedicalRecordId = userMedicalRecordInfos.FirstOrDefault().Id;
                }
                else throw new AppException("User chưa có thông tin hồ sơ người bệnh");


                medicalRecordDetailModel.Created = DateTime.Now;
                medicalRecordDetailModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                medicalRecordDetailModel.Active = true;
                var item = mapper.Map<MedicalRecordDetails>(medicalRecordDetailModel);
                List<UserFiles> medicalRecordDetailFiles = new List<UserFiles>();
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                if (medicalRecordDetailModel.UserFiles != null && medicalRecordDetailModel.UserFiles.Any())
                {


                    foreach (var file in medicalRecordDetailModel.UserFiles)
                    {
                        string filePath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME, file.FileName);

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
                            medicalRecordDetailFiles.Add(new UserFiles()
                            {
                                Created = DateTime.Now,
                                CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                Active = true,
                                Deleted = false,
                                FileName = Path.GetFileName(filePath),
                                FileExtension = Path.GetExtension(filePath),
                                FileType = file.FileType,
                                ContentType = ContentFileTypeUtilities.GetMimeType(filePath),
                                FileUrl = fileUrl
                            });
                        }
                        item.UserFiles = medicalRecordDetailFiles;
                    }
                    success = await this.medicalRecordDetailService.CreateAsync(item);
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
                    appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                    if (folderUploadPaths.Any())
                    {
                        foreach (var folderUploadPath in folderUploadPaths)
                        {
                            System.IO.File.Delete(folderUploadPath);
                        }
                    }
                    throw new Exception("Lỗi trong quá trình xử lý");
                }
            }
            else throw new AppException(ModelState.GetErrorMessage());

            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật tiểu sử bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailModel"></param>
        /// <returns></returns>
        [HttpPut("update-medical-record-detail")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateItem([FromBody] MedicalRecordDetailModel medicalRecordDetailModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                var userMedicalRecordInfos = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
                if (userMedicalRecordInfos != null && userMedicalRecordInfos.Any())
                {
                    medicalRecordDetailModel.MedicalRecordId = userMedicalRecordInfos.FirstOrDefault().Id;
                }
                else throw new AppException("User chưa có thông tin hồ sơ người bệnh");

                medicalRecordDetailModel.Updated = DateTime.Now;
                medicalRecordDetailModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                List<string> filePaths = new List<string>();
                List<string> folderUploadPaths = new List<string>();
                if (medicalRecordDetailModel != null)
                {
                    var item = mapper.Map<MedicalRecordDetails>(medicalRecordDetailModel);

                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
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
                                file.MedicalRecordDetailId = item.Id;
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

                    success = await this.medicalRecordDetailService.UpdateAsync(item);
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

            }
            else throw new AppException(ModelState.GetErrorMessage());

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

        /// <summary>
        /// Xóa array item
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpDelete("delete-medical-record-detail-file-multiples")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteItem([FromBody] List<int> itemIds)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = true;
            if (itemIds != null && itemIds.Any())
            {
                foreach (var itemId in itemIds)
                {
                    success &= await this.medicalRecordDetailFileService.DeleteAsync(itemId);
                }
            }
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
        /// Cập nhật thông tin ghi chú của user
        /// </summary>
        /// <param name="updateMedicalRecordDetailNoteModel"></param>
        /// <returns></returns>
        [HttpPost("update-note")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMedicalRecordDetailNote([FromBody] UpdateMedicalRecordDetailNoteModel updateMedicalRecordDetailNoteModel)
        {
            bool success = false;

            // Lấy thông tin hồ sơ của user
            var userMedicalRecordInfos = await this.medicalRecordService.GetAsync(e => !e.Deleted && e.Active && e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (userMedicalRecordInfos == null || !userMedicalRecordInfos.Any())
                throw new AppException("Không tìm thấy thông tin hồ sơ");

            var existMedicalRecordDetails = await this.medicalRecordDetailService.GetAsync(e => !e.Deleted && e.Active && e.MedicalRecordId == userMedicalRecordInfos.FirstOrDefault().Id && e.Id == updateMedicalRecordDetailNoteModel.MedicalRecordDetailId);
            if (existMedicalRecordDetails == null || !existMedicalRecordDetails.Any())
                throw new AppException("Không tìm thấy thông tin tiểu sử");
            var updateMedicalRecordDetail = existMedicalRecordDetails.FirstOrDefault();
            updateMedicalRecordDetail.Updated = DateTime.Now;
            updateMedicalRecordDetail.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            updateMedicalRecordDetail.Note = updateMedicalRecordDetailNoteModel.Note;

            Expression<Func<MedicalRecordDetails, object>>[] includeProperties = new Expression<Func<MedicalRecordDetails, object>>[]
            {
                e => e.Updated,
                e => e.UpdatedBy,
                e => e.Note,
            };
            success = await this.medicalRecordDetailService.UpdateFieldAsync(updateMedicalRecordDetail, includeProperties);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        #region Contants

        public const string MEDICAL_RECORD_FOLDER_NAME = "medicalrecord";

        #endregion
    }
}
