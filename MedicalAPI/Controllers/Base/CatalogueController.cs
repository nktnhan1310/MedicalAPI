using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Interface.Services.Base;
using Medical.Utilities;
using MedicalAPI.Model.DomainModel;
using MedicalAPI.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalAPI.Controllers
{
    [ApiController]
    public abstract class CatalogueController<E, T, DomainSearch> : BaseController<E, T, DomainSearch> where E : MedicalCatalogueAppDomain where T : MedicalCatalogueAppDomainModel where DomainSearch : BaseSearch
    {
        protected ICatalogueService<E, DomainSearch> catalogueService;

        public CatalogueController(IServiceProvider serviceProvider, ILogger<BaseController<E, T, DomainSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
        }

        /// <summary>
        /// Lấy all thông tin danh sách item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override async Task<AppDomainResult> Get()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var items = await this.catalogueService.GetAllAsync();
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
        public override async Task<AppDomainResult> GetById(int id)
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
        public override async Task<AppDomainResult> AddItem([FromBody] T itemModel)
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
                        var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                        if (!string.IsNullOrEmpty(messageUserCheck))
                        {
                            appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                            appDomainResult.Success = false;
                            appDomainResult.ResultMessage = messageUserCheck;
                            return appDomainResult;
                        }
                        success = await this.catalogueService.CreateAsync(item);
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
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] T itemModel)
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
                        var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                        if (!string.IsNullOrEmpty(messageUserCheck))
                        {
                            appDomainResult.ResultCode = (int)HttpStatusCode.BadRequest;
                            appDomainResult.Success = false;
                            appDomainResult.ResultMessage = messageUserCheck;
                            return appDomainResult;
                        }
                        success = await this.catalogueService.UpdateAsync(item);
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
        public override async Task<AppDomainResult> PatchItem(int id, [FromBody] T itemModel)
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
                    success = await this.catalogueService.UpdateFieldAsync(item, includeProperties);
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
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                bool success = await this.catalogueService.DeleteAsync(id);
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
        public override async Task<AppDomainResult> GetPagedData([FromBody] DomainSearch baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                if (ModelState.IsValid)
                {
                    baseSearch.OrderBy = "Id";
                    PagedList<E> pagedData = await this.catalogueService.GetPagedListData(baseSearch);
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

    }
}
