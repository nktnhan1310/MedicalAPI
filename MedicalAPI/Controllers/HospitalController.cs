using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Medical.Interface;

namespace MedicalAPI.Controllers
{
    [Route("api/hospital")]
    [ApiController]
    [Description("Quản lý bệnh viện")]
    [Authorize]
    public class HospitalController : BaseController<Hospitals, HospitalModel, SearchHospital>
    {
        private readonly IHospitalFileService hospitalFileService;
        private readonly IServiceTypeMappingHospitalService serviceTypeMappingHospitalService;
        private readonly IChannelMappingHospitalService channelMappingHospitalService;
        private readonly IHospitalHistoryService hospitalHistoryService;
        private readonly IHospitalTypeService hospitalTypeService;
        private readonly IHospitalFunctionTypeService hospitalFunctionTypeService;

        private readonly IBankInfoService bankInfoService;


        private readonly IConfiguration configuration;
        public HospitalController(IServiceProvider serviceProvider
            , ILogger<BaseController<Hospitals, HospitalModel, SearchHospital>> logger
            , IWebHostEnvironment env
            , IConfiguration configuration
            ) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IHospitalService>();
            hospitalFileService = serviceProvider.GetRequiredService<IHospitalFileService>();
            serviceTypeMappingHospitalService = serviceProvider.GetRequiredService<IServiceTypeMappingHospitalService>();
            channelMappingHospitalService = serviceProvider.GetRequiredService<IChannelMappingHospitalService>();
            bankInfoService = serviceProvider.GetRequiredService<IBankInfoService>();
            hospitalHistoryService = serviceProvider.GetRequiredService<IHospitalHistoryService>();
            hospitalTypeService = serviceProvider.GetRequiredService<IHospitalTypeService>();
            hospitalFunctionTypeService = serviceProvider.GetRequiredService<IHospitalFunctionTypeService>();

            this.configuration = configuration;
        }

        /// <summary>
        /// Lấy all thông tin danh sách item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> Get()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var items = await this.domainService.GetAllAsync();
            var itemModels = mapper.Map<IList<HospitalModel>>(items);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = itemModels,
                ResultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin bệnh viện
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                id = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                // Lấy thông tin kênh đăng ký bệnh viện
                item.ChannelMappingHospitals = await this.channelMappingHospitalService.GetAsync(e => !e.Deleted && e.HospitalId == item.Id);
                if (item.ChannelMappingHospitals != null && item.ChannelMappingHospitals.Any())
                    item.ChannelIds = item.ChannelMappingHospitals.Select(e => e.ChannelId).ToList();
                // Lấy thông tin mapping dịch vụ bệnh viện
                item.ServiceTypeMappingHospitals = await this.serviceTypeMappingHospitalService.GetAsync(e => !e.Deleted && e.HospitalId == item.Id);
                // Lấy thông tin file của thông tin bệnh viện
                item.HospitalFiles = await this.hospitalFileService
                    .GetAsync(e => !e.Deleted && e.HospitalId == item.Id
                    , e => new HospitalFiles()
                    {
                        Id = e.Id,
                        Deleted = e.Deleted,
                        Active = e.Active,
                        Updated = e.Updated,
                        UpdatedBy = e.UpdatedBy,
                        Description = e.Description,
                        FileName = e.FileName,
                        FileExtension = e.FileExtension,
                        HospitalId = e.HospitalId,
                        FileType = e.FileType,
                        FileUrl = e.FileUrl,
                    });
                // Lấy thông tin ngân hàng liên kết của bệnh viện
                item.BankInfos = await this.bankInfoService.GetAsync(e => !e.Deleted && e.HospitalId == item.Id);
                if (item.TickEndReceiveExamination.HasValue)
                {
                    TimeSpan ts = TimeSpan.FromTicks(item.TickEndReceiveExamination.Value);
                    item.TickEndReceiveExaminationValue = ts.ToString("hh':'mm':'ss");
                }
            }
            else
                throw new KeyNotFoundException("Item không tồn tại");
            var itemModel = mapper.Map<HospitalModel>(item);
            appDomainResult.Data = itemModel;
            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            appDomainResult.Success = true;

            return appDomainResult;
        }

        /// <summary>
        /// Chỉnh sửa thông tin bệnh viện với form file
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] HospitalModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                itemModel.Deleted = false;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemModel.Created = DateTime.Now;
                itemModel.Code = StringCipher.GetFirstCharOfWord(itemModel.Name);
                var item = mapper.Map<Hospitals>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.HospitalFiles != null && item.HospitalFiles.Any())
                    {
                        foreach (var file in item.HospitalFiles)
                        {
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");

                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                //var currentLinkSite = $"{Medical.Extensions.HttpContext.Current.Request.Scheme}://{Medical.Extensions.HttpContext.Current.Request.Host}/{UPLOAD_FOLDER_NAME}/";
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Path.GetFileName(filePath));

                                filePaths.Add(filePath);
                                file.Created = DateTime.Now;
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                //file.FileContent = System.IO.File.ReadAllBytes(filePath);
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.HospitalId = item.Id;
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
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

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
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] HospitalModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                itemModel.Id = id;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                if (LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.Id = LoginContext.Instance.CurrentUser.HospitalId.Value;
                var item = mapper.Map<Hospitals>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.HospitalFiles != null && item.HospitalFiles.Any())
                    {
                        foreach (var file in item.HospitalFiles)
                        {
                            // ------- START GET URL FOR FILE
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);

                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                //var currentLinkSite = $"{Medical.Extensions.HttpContext.Current.Request.Scheme}://{Medical.Extensions.HttpContext.Current.Request.Host}/{UPLOAD_FOLDER_NAME}/";
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, Path.GetFileName(filePath));

                                // ------- END GET URL FOR FILE

                                filePaths.Add(filePath);
                                file.Created = DateTime.Now;
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                //file.FileContent = System.IO.File.ReadAllBytes(filePath);
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.HospitalId = item.Id;
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
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override Task<AppDomainResult> GetPagedData([FromQuery] SearchHospital baseSearch)
        {
            return base.GetPagedData(baseSearch);
        }

        /// <summary>
        /// Lấy danh sách lịch sử chỉnh sửa bệnh viện
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-history-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetHistoryPagedData([FromQuery] BaseHospitalSearch baseSearch)
        {
            if (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId.Value > 0)
                baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId.Value;
            var pagedList = await this.hospitalHistoryService.GetPagedListData(baseSearch);
            var pagedListModel = mapper.Map<PagedList<HospitalHistoryModel>>(pagedList);
            return new AppDomainResult()
            {
                Data = pagedListModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        [HttpGet("download-file/{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Download })]
        public override async Task<AppDomainResult> DownloadFile(int id)
        {
            var fileInfo = await this.hospitalFileService.GetByIdAsync(id);
            if (fileInfo != null)
                return new AppDomainResult()
                {
                    Data = fileInfo.FileUrl,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            else
                throw new Exception("Hospital Download File Error!");
        }
    }
}
