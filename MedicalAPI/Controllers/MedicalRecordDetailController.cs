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

namespace MedicalAPI.Controllers
{
    [Route("api/medical-record-detail")]
    [ApiController]
    [Description("Quản lý hồ sơ chi tiết bệnh án")]
    [Authorize]
    public class MedicalRecordDetailController : CoreHospitalController<MedicalRecordDetails, MedicalRecordDetailModel, SearchMedicalRecordDetail>
    {
        private IHttpContextAccessor httpContextAccessor;
        private IConfiguration configuration;
        private IMedicalRecordDetailFileService medicalRecordDetailFileService;
        private IExaminationFormDetailService examinationFormDetailService;
        private IUserFileService userFileService;

        public MedicalRecordDetailController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<MedicalRecordDetails, MedicalRecordDetailModel, SearchMedicalRecordDetail>> logger, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            medicalRecordDetailFileService = serviceProvider.GetRequiredService<IMedicalRecordDetailFileService>();
            examinationFormDetailService = serviceProvider.GetRequiredService<IExaminationFormDetailService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
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
            MedicalRecordDetailModel medicalRecordDetailModel = new MedicalRecordDetailModel();
            SearchMedicalRecordDetail searchMedicalRecordDetail = new SearchMedicalRecordDetail()
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Id desc",
                MedicalRecordDetailId = id,
                //UserId = userId,
            };
            var pagedList = await this.domainService.GetPagedListData(searchMedicalRecordDetail);
            if (pagedList != null && pagedList.Items.Any())
            {
                var recordDetailInfo = pagedList.Items.FirstOrDefault();
                recordDetailInfo.UserFiles = await userFileService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == recordDetailInfo.Id);
                medicalRecordDetailModel = mapper.Map<MedicalRecordDetailModel>(recordDetailInfo);
            }
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = medicalRecordDetailModel
            };
        }

        /// <summary>
        /// Lấy thông tin chi tiết hồ sơ bệnh án theo user
        /// </summary>
        /// <param name="recordDetailId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("get-record-detail-info-by-user/{recordDetailId}")]
        public async Task<IActionResult> GetRecordDetailInfoByUser(int recordDetailId)
        {
            SearchMedicalRecordDetail searchMedicalRecordDetail = new SearchMedicalRecordDetail()
            {
                PageIndex = 1,
                PageSize = 20,
                OrderBy = "Id desc",
                MedicalRecordDetailId = recordDetailId,
                //UserId = userId,
            };
            var pagedList = await this.domainService.GetPagedListData(searchMedicalRecordDetail);
            if (pagedList != null && pagedList.Items.Any())
            {
                var recordDetailInfo = pagedList.Items.FirstOrDefault();
                recordDetailInfo.UserFiles = await userFileService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == recordDetailInfo.Id);
                var recordDetailInfoModel = mapper.Map<MedicalRecordDetailModel>(recordDetailInfo);
                return Ok(recordDetailInfoModel);
            }
            else return NotFound("Không tìm thấy thông tin");
        }

        /// <summary>
        /// Lấy thông tin dịch vụ phát sinh
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
        [HttpPost()]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] MedicalRecordDetailModel medicalRecordDetailModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
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
                        string folderUploadPath = string.Empty;
                        var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                        if (isProduct)
                            folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                        else
                            folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                        string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                        string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                        string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                        if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(folderUploadUrl))
                        {
                            FileUtils.CreateDirectory(folderUploadUrl);
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
                    success = await this.domainService.CreateAsync(item);
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
        /// Cập nhật thông tin tiểu sử bệnh án
        /// </summary>
        /// <param name="id"></param>
        /// <param name="medicalRecordDetailModel"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] MedicalRecordDetailModel medicalRecordDetailModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
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
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(folderUploadUrl))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
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

                    success = await this.domainService.UpdateAsync(item);
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
    }
}
