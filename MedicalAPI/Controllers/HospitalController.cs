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
using MedicalAPI.Model;
using MedicalAPI.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Quản lý bệnh viện")]
    public class HospitalController : BaseController<Hospitals, HospitalModel, SearchHospital>
    {
        private readonly IHospitalFileService hospitalFileService;
        private readonly IServiceTypeMappingHospitalService serviceTypeMappingHospitalService;
        private readonly IChannelMappingHospitalService channelMappingHospitalService;
        public HospitalController(IServiceProvider serviceProvider, ILogger<BaseController<Hospitals, HospitalModel, SearchHospital>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IHospitalService>();
            hospitalFileService = serviceProvider.GetRequiredService<IHospitalFileService>();
            serviceTypeMappingHospitalService = serviceProvider.GetRequiredService<IServiceTypeMappingHospitalService>();
            channelMappingHospitalService = serviceProvider.GetRequiredService<IChannelMappingHospitalService>();
        }

        /// <summary>
        /// Lấy thông tin bệnh viện
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var item = await this.domainService.GetByIdAsync(id, e => new Hospitals()
                {
                    Id = e.Id,
                    Updated = e.Updated,
                    UpdatedBy = e.UpdatedBy,
                    Deleted = e.Deleted,
                    Active = e.Active,
                    Address = e.Address,
                    BankInfo = e.BankInfo,
                    BankNo = e.BankNo,
                    CallPortDescription = e.CallPortDescription,
                    IsHasCallPort = e.IsHasCallPort,
                    ChannelMappingHospitals = new List<ChannelMappingHospital>(),
                    Code = e.Code,
                    Name = e.Name,
                    Email = e.Email,
                    ExpertFullName = e.ExpertFullName,
                    ExpertPhone = e.ExpertPhone,
                    IsHasItExpert = e.IsHasItExpert,
                    HospitalFiles = new List<HospitalFiles>(),
                    MinutePerPatient = e.MinutePerPatient,
                    NoCallPortDescription = e.NoCallPortDescription,
                    Phone = e.Phone,
                    ServiceTypeMappingHospitals = new List<ServiceTypeMappingHospital>(),
                    Slogan = e.Slogan,
                    WebSiteUrl = e.WebSiteUrl
                });
                if(item != null)
                {
                    // Lấy thông tin kênh đăng ký bệnh viện
                    item.ChannelMappingHospitals = await this.channelMappingHospitalService.GetAsync(e => !e.Deleted && e.HospitalId == item.Id);
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
                            FileType = e.FileType
                        });
                }
                else
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                    appDomainResult.Success = false;
                    return appDomainResult;
                }
                var itemModel = mapper.Map<HospitalModel>(item);
                appDomainResult.Data = itemModel;
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        /// <summary>
        /// Chỉnh sửa thông tin bệnh viện với form file
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<AppDomainResult> AddItem([FromBody] HospitalModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                bool success = false;
                if (ModelState.IsValid)
                {
                    var item = mapper.Map<Hospitals>(itemModel);
                    if (item != null)
                    {
                        // Kiểm tra item có tồn tại chưa?
                        var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                        if (!string.IsNullOrEmpty(messageUserCheck))
                        {
                            appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                            appDomainResult.Success = false;
                            appDomainResult.ResultMessage = messageUserCheck;
                            return appDomainResult;
                        }
                        List<string> filePaths = new List<string>();
                        if (item.HospitalFiles != null && item.HospitalFiles.Any())
                        {
                            foreach (var file in item.HospitalFiles)
                            {
                                string filePath = Path.Combine(env.ContentRootPath, "temp", file.FileName);
                                // Kiểm tra có tồn tại file trong temp chưa?
                                if (System.IO.File.Exists(filePath))
                                {
                                    filePaths.Add(filePath);
                                    file.Created = DateTime.Now;
                                    file.CreatedBy = "admin";
                                    file.Active = true;
                                    file.Deleted = false;
                                    file.FileContent = System.IO.File.ReadAllBytes(filePath);
                                    file.FileName = Path.GetFileName(filePath);
                                    file.FileExtension = Path.GetExtension(filePath);
                                    file.HospitalId = item.Id;
                                    file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
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
                            appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                        
                        appDomainResult.Success = success;
                    }
                    else
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                        appDomainResult.Success = false;
                    }
                }
                else
                {
                    appDomainResult.Success = false;
                    appDomainResult.ResultMessage = ModelState.GetErrorMessage();
                    appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                }

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
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
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] HospitalModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                bool success = false;
                if (ModelState.IsValid)
                {
                    itemModel.Id = id;
                    var item = mapper.Map<Hospitals>(itemModel);
                    if (item != null)
                    {
                        // Kiểm tra item có tồn tại chưa?
                        var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                        if (!string.IsNullOrEmpty(messageUserCheck))
                        {
                            appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                            appDomainResult.Success = false;
                            appDomainResult.ResultMessage = messageUserCheck;
                            return appDomainResult;
                        }

                        List<string> filePaths = new List<string>();
                        if (item.HospitalFiles != null && item.HospitalFiles.Any())
                        {
                            foreach (var file in item.HospitalFiles)
                            {
                                string filePath = Path.Combine(env.ContentRootPath, "temp", file.FileName);
                                // Kiểm tra có tồn tại file trong temp chưa?
                                if (System.IO.File.Exists(filePath))
                                {
                                    filePaths.Add(filePath);
                                    file.Created = DateTime.Now;
                                    file.CreatedBy = "admin";
                                    file.Active = true;
                                    file.Deleted = false;
                                    file.FileContent = System.IO.File.ReadAllBytes(filePath);
                                    file.FileName = Path.GetFileName(filePath);
                                    file.FileExtension = Path.GetExtension(filePath);
                                    file.HospitalId = item.Id;
                                    file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                                }
                                else
                                {
                                    file.Updated = DateTime.Now;
                                    //---------- Người update
                                    //file.UpdatedBy = ...
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
                            appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                        appDomainResult.Success = success;
                    }
                    else
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                        appDomainResult.Success = false;
                    }
                }
                else
                {
                    appDomainResult.Success = false;
                    appDomainResult.ResultMessage = ModelState.GetErrorMessage();
                    appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                }

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        [HttpGet("{id}")]
        public override async Task<ActionResult> DownloadFile(int id)
        {
            try
            {
                var fileInfo = await this.hospitalFileService.GetByIdAsync(id);
                if(fileInfo != null)
                    return File(fileInfo.FileContent, fileInfo.ContentType, fileInfo.FileName);
                this.logger.LogInformation("Hospital Download File Error!");
                throw new Exception("Hospital Download File Error!");
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                throw ex;
            }
        }
    }
}
