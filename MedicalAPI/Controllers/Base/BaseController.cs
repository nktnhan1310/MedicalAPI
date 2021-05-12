using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Interface;
using Medical.Utilities;
using MedicalAPI.Model.DomainModel;
using MedicalAPI.Utils;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalAPI.Controllers
{
    [ApiController]
    public abstract class BaseController<E, T, F> : Controller where E : MedicalAppDomain where T : MedicalAppDomainModel where F : BaseSearch
    {
        protected readonly ILogger<BaseController<E, T, F>> logger;
        protected readonly IServiceProvider serviceProvider;
        protected readonly IMapper mapper;
        protected IDomainService<E, F> domainService;
        protected IWebHostEnvironment env;

        public BaseController(IServiceProvider serviceProvider, ILogger<BaseController<E, T, F>> logger, IWebHostEnvironment env)
        {
            this.env = env;
            this.logger = logger;
            this.mapper = serviceProvider.GetService<IMapper>();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Lấy all thông tin danh sách item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<AppDomainResult> Get()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var items = await this.domainService.GetAllAsync();
                var itemModels = mapper.Map<IList<T>>(items);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModels,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "Get", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }

            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public virtual async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var item = await this.domainService.GetByIdAsync(id);
                if (item != null)
                {
                    var itemModel = mapper.Map<T>(item);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else
                {
                    appDomainResult = new AppDomainResult()
                    {
                        Success = false,
                        ResultCode = (int)HttpStatusCode.InternalServerError
                    };
                }

            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "GetById", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }

            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<AppDomainResult> AddItem([FromBody] T itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                bool success = false;
                if (ModelState.IsValid)
                {
                    var item = mapper.Map<E>(itemModel);
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
                        success = await this.domainService.CreateAsync(item);
                        if (success)
                            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                        else
                            appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
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
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "AddItem", ex.Message));

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
        public virtual async Task<AppDomainResult> UpdateItem(int id, [FromBody] T itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                bool success = false;
                if (ModelState.IsValid)
                {
                    var item = mapper.Map<E>(itemModel);
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
                        success = await this.domainService.UpdateAsync(item);
                        if (success)
                            appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                        else
                            appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;

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
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "UpdateItem", ex.Message));

                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật 1 phần item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public virtual async Task<AppDomainResult> PatchItem(int id, [FromBody] T itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                bool success = false;
                var item = mapper.Map<E>(itemModel);
                if (item != null)
                {
                    Expression<Func<E, object>>[] includeProperties = new Expression<Func<E, object>>[]
                    {
                        x => x.Active,
                    };
                    success = await this.domainService.UpdateFieldAsync(item, includeProperties);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                    appDomainResult.Success = success;
                }
                else
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                    appDomainResult.Success = false;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "PatchItem", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }


        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                bool success = await this.domainService.DeleteAsync(id);
                if (success)
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    appDomainResult.Success = success;
                }
                else
                {
                    appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
                    appDomainResult.Success = false;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "DeleteItem", ex.Message));

                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [ActionName("GetPagedData")]
        [HttpPost]
        public virtual async Task<AppDomainResult> GetPagedData([FromBody] F baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                if (ModelState.IsValid)
                {
                    baseSearch.OrderBy = "Id";
                    PagedList<E> pagedData = await this.domainService.GetPagedListData(baseSearch);
                    PagedList<T> pagedDataModel = mapper.Map<PagedList<T>>(pagedData);
                    appDomainResult = new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
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
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "GetPagedData", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        #region Files

        [HttpGet("{id}")]
        public virtual async Task<ActionResult> DownloadFile(int id)
        {
            await Task.Run(() =>
            {
            });
            throw new Exception(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "DownloadFile", "Error Download"));
        }

        /// <summary>
        /// Upload Single File
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<AppDomainResult> UploadFile(IFormFile file)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                await Task.Run(() =>
                {
                    if (file != null && file.Length > 0)
                    {
                        string fileName = file.FileName;
                        string fileUploadPath = Path.Combine(env.ContentRootPath, "temp");
                        string path = Path.Combine(fileUploadPath, fileName);
                        FileUtils.CreateDirectory(fileUploadPath);
                        var fileByte = FileUtils.StreamToByte(file.OpenReadStream());
                        FileUtils.SaveToPath(path, fileByte);
                        appDomainResult.Success = true;
                    }
                });
                
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "UploadFile", ex.Message));

                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }

            return appDomainResult;
        }

        /// <summary>
        /// Upload Multiple File
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("multiple-files")]
        public virtual async Task<AppDomainResult> UploadFiles(List<IFormFile> files)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                await Task.Run(() =>
                {
                    if (files != null && files.Any())
                    {
                        foreach (var file in files)
                        {
                            string fileName = file.FileName;
                            string fileUploadPath = Path.Combine(env.ContentRootPath, "temp");
                            string path = Path.Combine(fileUploadPath, fileName);
                            FileUtils.CreateDirectory(fileUploadPath);
                            var fileByte = FileUtils.StreamToByte(file.OpenReadStream());
                            FileUtils.SaveToPath(path, fileByte);
                        }
                        appDomainResult.Success = true;
                    }
                });
                
            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "UploadFiles", ex.Message));
                appDomainResult.Success = false;
                appDomainResult.ResultCode = (int)HttpStatusCode.InternalServerError;
            }
            return appDomainResult;
        }

        #endregion
    }
}
