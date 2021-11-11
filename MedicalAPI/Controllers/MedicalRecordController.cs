using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;

namespace MedicalAPI.Controllers
{
    [Route("api/medical-record")]
    [ApiController]
    [Description("Quản lý hồ sơ khám bệnh")]
    [Authorize]
    public class MedicalRecordController : BaseController<MedicalRecords, MedicalRecordModel, SearchMedicalRecord>
    {
        private readonly IMedicalRecordAdditionService medicalRecordAdditionService;
        private readonly IRelationService relationService;
        private readonly IMedicalRecordService medicalRecordService;
        //private readonly IMedicalRecordFileService medicalRecordFileService;
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        private readonly INotificationService notificationService;
        private readonly IUserFileService userFileService;

        private IHubContext<NotificationAppHub> appHubContext;

        public MedicalRecordController(IServiceProvider serviceProvider, ILogger<BaseController<MedicalRecords, MedicalRecordModel, SearchMedicalRecord>> logger
            , IWebHostEnvironment env
            , IConfiguration configuration
            , IHubContext<NotificationAppHub> appHubContext
            ) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            medicalRecordAdditionService = serviceProvider.GetRequiredService<IMedicalRecordAdditionService>();
            relationService = serviceProvider.GetRequiredService<IRelationService>();
            medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            this.configuration = configuration;
            //medicalRecordFileService = serviceProvider.GetRequiredService<IMedicalRecordFileService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            notificationService = serviceProvider.GetRequiredService<INotificationService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileService>();
            this.appHubContext = appHubContext;
        }

        /// <summary>
        /// Lây thông tin tài khoản khach
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-list-user-customer")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserCustomerInfo(string userName)
        {
            var userCustomerInfos = await this.userService.GetAsync(e =>
            !e.Deleted && e.Active && !e.IsLocked && !e.HospitalId.HasValue && !e.IsAdmin
            && (string.IsNullOrEmpty(userName)
            || (e.Phone.Contains(userName) || e.Email.Contains(userName) || e.UserName.Contains(userName)
            || e.UserFullName.Contains(userName)
            ))
            , e => new Users()
            {
                Id = e.Id,
                Phone = e.Phone,
                Email = e.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                UserName = e.UserName,
                UserFullName = e.UserFullName,
            });

            return new AppDomainResult()
            {
                Data = mapper.Map<IList<UserModel>>(userCustomerInfos),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin user làm hồ sơ
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("get-user-info/{userId}")]
        public async Task<AppDomainResult> GetUserInfo(int? userId)
        {
            UserModel userInfo = null;
            if (!userId.HasValue || userId.Value == 0)
                throw new AppException("Không tìm thấy thông tin user");
            var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Active && !e.IsLocked && e.Id == userId);
            if (userInfos != null && userInfos.Any())
            {
                userInfo = userInfos.Select(e => new UserModel()
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    UserFullName = e.UserFullName
                }).FirstOrDefault();
            }
            else throw new AppException("Không tìm thấy thông tin user");
            return new AppDomainResult()
            {
                Success = true,
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

            SearchMedicalRecord searchMedicalRecord = new SearchMedicalRecord()
            {
                PageIndex = 1,
                PageSize = 10,
                OrderBy = "Id desc",
                MedicalRecordId = id
            };
            MedicalRecords item = null;
            var pagedItems = await this.domainService.GetPagedListData(searchMedicalRecord);
            if (pagedItems != null && pagedItems.Items.Any())
                item = pagedItems.Items.FirstOrDefault();
            //var item = await this.domainService.GetByIdAsync(id);

            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId))
                    )
                {
                    var itemModel = mapper.Map<MedicalRecordModel>(item);
                    var medicalAdditions = await this.medicalRecordAdditionService.GetAsync(e => !e.Deleted && e.MedicalRecordId == id);
                    if (medicalAdditions != null)
                        itemModel.MedicalRecordAdditions = mapper.Map<IList<MedicalRecordAdditionModel>>(medicalAdditions);
                    var medicalFiles = await this.userFileService.GetAsync(e => !e.Deleted && e.MedicalRecordId == id);
                    if (medicalFiles != null)
                        itemModel.UserFiles = mapper.Map<IList<UserFileModel>>(medicalFiles);
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
        /// Cập nhật thông tin hồ sơ khám bệnh
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] MedicalRecordModel itemModel)
        {
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
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();

                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            // Kiểm tra có tồn tại file trong temp chưa?
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);

                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                // ------- START GET URL FOR FILE
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
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
                        // Remove file trong thư mục temp
                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        // Xóa file trong thư mục upload
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
        /// Thêm mới hồ sơ khám bệnh
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] MedicalRecordModel itemModel)
        {
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
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadUrl, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, MEDICAL_RECORD_FOLDER_NAME, Path.GetFileName(filePath));
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
                    success = await this.domainService.CreateAsync(item);
                    if (success)
                    {
                        if (item.UserId > 0)
                        {
                            await notificationService.CreateCustomNotificationUser(null, null
                            , new List<int>() { item.UserId }
                            , string.Empty
                            , string.Empty
                            , LoginContext.Instance.CurrentUser.UserName
                            , null
                            , false
                            , "USER"
                            , CoreContants.NOTI_TEMPLATE_MEDICAL_RECORD_CREATE
                            );
                            await appHubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                        }

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
        /// Lấy thông tin danh sách quan hệ của hồ sơ bệnh án
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-list-relation")]
        public async Task<AppDomainResult> GetListRelation()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var relations = await this.relationService.GetAsync(e => !e.Deleted && e.Active);
            var relationModels = mapper.Map<IList<RelationModel>>(relations);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = relationModels
            };
            return appDomainResult;
        }

    }
}
