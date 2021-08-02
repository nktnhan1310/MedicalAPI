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
    [Description("Hồ sơ bệnh án/ Hồ sơ người bệnh")]
    [Authorize]
    public class MedicalRecordController : BaseController
    {
        private readonly IMedicalRecordAdditionService medicalRecordAdditionService;
        private readonly IMedicalRecordService medicalRecordService;
        private readonly IMedicalRecordFileService medicalRecordFileService;
        private readonly IMedicalRecordDetailService medicalRecordDetailService;
        private readonly IMedicalRecordDetailFileService medicalRecordDetailFileService;
        private readonly IUserService userService;

        public MedicalRecordController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            this.medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            medicalRecordAdditionService = serviceProvider.GetRequiredService<IMedicalRecordAdditionService>();
            medicalRecordFileService = serviceProvider.GetRequiredService<IMedicalRecordFileService>();
            medicalRecordDetailService = serviceProvider.GetRequiredService<IMedicalRecordDetailService>();
            medicalRecordDetailFileService = serviceProvider.GetRequiredService<IMedicalRecordDetailFileService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
        }

        /// <summary>
        /// Lấy thông tin hồ sơ bệnh án của user đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-medical-record-info")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetMedicalRecordInfo()
        {
            MedicalRecordModel medicalRecordModel = new MedicalRecordModel();
            SearchMedicalRecord searchMedicalRecord = new SearchMedicalRecord()
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Id desc",
                UserId = LoginContext.Instance.CurrentUser.UserId,
            };
            var pagedItems = await this.medicalRecordService.GetPagedListData(searchMedicalRecord);
            IList<MedicalRecords> medicalRecordInfos = new List<MedicalRecords>();
            if (pagedItems != null && pagedItems.Items.Any())
                medicalRecordInfos = pagedItems.Items.ToList();
            //var medicalRecordInfos = await this.medicalRecordService.GetAsync(e => e.UserId == LoginContext.Instance.CurrentUser.UserId);
            if (medicalRecordInfos != null && medicalRecordInfos.Any())
            {
                var medicalRecordInfo = medicalRecordInfos.FirstOrDefault();
                medicalRecordModel = mapper.Map<MedicalRecordModel>(medicalRecordInfo);
                // Lấy thông tin người thân
                var medicalAdditions = await this.medicalRecordAdditionService.GetAsync(e => !e.Deleted && e.MedicalRecordId == medicalRecordInfo.Id);
                if (medicalAdditions != null)
                    medicalRecordModel.MedicalRecordAdditions = mapper.Map<IList<MedicalRecordAdditionModel>>(medicalAdditions);
                // Lấy thông tin file hồ sơ bệnh án
                var medicalFiles = await this.medicalRecordFileService.GetAsync(e => !e.Deleted && e.MedicalRecordId == medicalRecordInfo.Id);
                if (medicalFiles != null)
                    medicalRecordModel.MedicalRecordFiles = mapper.Map<IList<MedicalRecordFileModel>>(medicalFiles);

                // Lấy thông tin chi tiết hồ sơ bệnh án
                var medicalRecordDetails = await this.medicalRecordDetailService.GetAsync(e => !e.Deleted && e.MedicalRecordId == medicalRecordInfo.Id);
                if (medicalRecordDetails != null)
                {
                    medicalRecordModel.MedicalRecordDetails = mapper.Map<IList<MedicalRecordDetailModel>>(medicalRecordDetails);
                    // Lấy thông tin file của hồ sơ bệnh án
                    if (medicalRecordModel.MedicalRecordDetails.Any())
                    {
                        foreach (var detail in medicalRecordModel.MedicalRecordDetails)
                        {
                            var detailFiles = await this.medicalRecordDetailFileService.GetAsync(e => !e.Deleted && e.MedicalRecordDetailId == detail.Id);
                            detail.MedicalRecordDetailFiles = mapper.Map<IList<MedicalRecordDetailFileModel>>(detailFiles);
                        }
                    }
                }

            }
            else
            {
                var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Active && !e.IsLocked && e.Id == LoginContext.Instance.CurrentUser.UserId);
                if(userInfos != null && userInfos.Any())
                {
                    var userInfo = userInfos.FirstOrDefault();
                    MedicalRecords medicalRecords = new MedicalRecords()
                    {
                        Created = DateTime.Now,
                        CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                        Email = userInfo.Email,
                        Phone = userInfo.Phone,
                        Active = true,
                        Deleted = false,
                        UserId = LoginContext.Instance.CurrentUser.UserId,
                        UserFullName = userInfo.FirstName + " " + userInfo.LastName
                    };
                    bool success = await this.medicalRecordService.CreateAsync(medicalRecords);
                    if (success)
                        medicalRecordModel = mapper.Map<MedicalRecordModel>(medicalRecords);
                    else throw new AppException("Tạo hồ sơ thất bại!");
                }
                else throw new AppException("Lấy thông tin user thất bại!");
            }
            return new AppDomainResult()
            {
                Data = medicalRecordModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
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



        #region Contants

        public const string MEDICAL_RECORD_FOLDER_NAME = "medicalrecord";

        #endregion

    }
}
