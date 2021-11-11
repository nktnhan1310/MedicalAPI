using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
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
    [Route("api/base-file")]
    [ApiController]
    [Description("Upload file lên hệ thống")]
    [Authorize]
    public class BaseFileController : BaseController
    {
        private ISystemFileService systemFileService;
        public BaseFileController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            systemFileService = serviceProvider.GetRequiredService<ISystemFileService>();
        }

        /// <summary>
        /// Lấy thông tin file hệ thống
        /// </summary>
        /// <param name="searchSystemFile"></param>
        /// <returns></returns>
        [HttpGet("get-system-files")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public virtual async Task<AppDomainResult> GetSystemFile([FromQuery] SearchSystemFile searchSystemFile)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                PagedList<SystemFiles> pagedData = await this.systemFileService.GetPagedListData(searchSystemFile);
                PagedList<SystemFileModel> pagedDataModel = mapper.Map<PagedList<SystemFileModel>>(pagedData);
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
        /// Upload Single File
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("upload-file")]
        [MedicalAppAuthorize(new string[] { CoreContants.Upload })]
        public virtual async Task<AppDomainResult> UploadFile(IFormFile file)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            await Task.Run(() =>
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                    string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME);
                    string path = Path.Combine(fileUploadPath, fileName);
                    FileUtils.CreateDirectory(fileUploadPath);
                    var fileByte = FileUtils.StreamToByte(file.OpenReadStream());
                    FileUtils.SaveToPath(path, fileByte);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = fileName
                    };
                }
                else throw new AppException("Không có thông tin file upload");
            });
            return appDomainResult;
        }

        /// <summary>
        /// Upload Multiple File
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("upload-multiple-files")]
        [MedicalAppAuthorize(new string[] { CoreContants.Upload })]
        public virtual async Task<AppDomainResult> UploadFiles(List<IFormFile> files)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            await Task.Run(() =>

            {
                if (files != null && files.Any())
                {
                    List<string> fileNames = new List<string>();
                    foreach (var file in files)
                    {
                        string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                        string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.TEMP_FOLDER_NAME);
                        string path = Path.Combine(fileUploadPath, fileName);
                        FileUtils.CreateDirectory(fileUploadPath);
                        var fileByte = FileUtils.StreamToByte(file.OpenReadStream());
                        FileUtils.SaveToPath(path, fileByte);
                        fileNames.Add(fileName);
                    }
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = fileNames
                    };
                }
            });
            return appDomainResult;
        }

    }
}
