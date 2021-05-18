using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using MedicalAPI.Model;
using MedicalAPI.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Description("Quản lý chức năng người dùng")]
    public class PermitObjectController : CatalogueController<PermitObjects, PermitObjectModel, BaseSearch>
    {
        public PermitObjectController(IServiceProvider serviceProvider, ILogger<BaseController<PermitObjects, PermitObjectModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IPermitObjectService>();
        }

        /// <summary>
        /// Lấy thông tin những chức năng cần được phân quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AppDomainResult> GetCatalogueController()
        {
            return await Task.Run(() =>
            {
                AppDomainResult appDomainResult = new AppDomainResult();
                AppDomain currentDomain = AppDomain.CurrentDomain;
                Assembly[] assems = currentDomain.GetAssemblies();
                var controllers = new List<object>();
                foreach (Assembly assem in assems)
                {
                    var controller = assem.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type) && !type.IsAbstract)
                  .Select(e => new
                  {
                      Id = e.Name.Replace("Controller", string.Empty),
                      Name = string.Format("{0}", ReflectionUtils.GetClassDescription(e)).Replace("Controller", string.Empty)
                  }).OrderBy(e => e.Name)
                      .Distinct();

                    controllers.AddRange(controller);
                }
                appDomainResult = new AppDomainResult()
                {
                    Data = controllers,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };


                return appDomainResult;
            });


        }

        /// <summary>
        /// Lấy thông tin chức năng theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var item = await this.catalogueService.GetByIdAsync(id);
                if (item != null)
                {
                    var itemModel = mapper.Map<PermitObjectModel>(item);
                    itemModel.ToView();
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");

            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "GetById", ex.Message));
                throw new Exception(ex.Message);
            }
            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới chức năng
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        public override Task<AppDomainResult> AddItem([FromBody] PermitObjectModel itemModel)
        {
            itemModel.ToModel();
            return base.AddItem(itemModel);
        }

        /// <summary>
        /// Cập nhật thông tin chức năng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public override Task<AppDomainResult> UpdateItem(int id, [FromBody] PermitObjectModel itemModel)
        {
            itemModel.ToModel();
            return base.UpdateItem(id, itemModel);
        }

    }
}
