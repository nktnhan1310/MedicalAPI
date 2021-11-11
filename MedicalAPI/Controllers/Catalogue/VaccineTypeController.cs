using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers.Catalogue
{
    [Route("api/vaccine-type")]
    [ApiController]
    [Description("Quản lý loại vaccine")]
    [Authorize]
    public class VaccineTypeController : CatalogueCoreHospitalController<VaccineTypes, VaccineTypeModel, BaseHospitalSearch>
    {
        public VaccineTypeController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<VaccineTypes, VaccineTypeModel, BaseHospitalSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IVaccineTypeService>();
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

            var item = await this.catalogueService.GetByIdAsync(id);
            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<VaccineTypes>(item);
                    if (itemModel.HospitalId.HasValue && itemModel.HospitalId.Value > 0)
                    {
                        var hospitalInfos = await this.hospitalService.GetAsync(e => e.Id == itemModel.HospitalId.Value);
                        if (hospitalInfos != null && hospitalInfos.Any()) itemModel.HospitalName = hospitalInfos.FirstOrDefault().Name;
                    }
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
                throw new KeyNotFoundException("Item không tồn tại");

            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] VaccineTypeModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Active = true;
                itemModel.Deleted = false;
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                if(itemModel.TargetIds != null && itemModel.TargetIds.Any())
                    itemModel.TargetIdValues = string.Join(';', itemModel.TargetIds.ToList());
                var item = mapper.Map<VaccineTypes>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.catalogueService.CreateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
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
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] VaccineTypeModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemModel.Active = true;
                if (itemModel.TargetIds != null && itemModel.TargetIds.Any())
                    itemModel.TargetIdValues = string.Join(';', itemModel.TargetIds.ToList());
                var item = mapper.Map<VaccineTypes>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.catalogueService.UpdateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");

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
        /// Lấy thông tin đối tượng tiêm chủng của loại vaccine
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-target-type")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetTargetTypes()
        {
            var listTargetType = new List<object>()
            {
                new
                {
                    Id = (int)CatalogueUtilities.TargetType.Child,
                    Name = "Trẻ em (dưới 12 tuổi)"
                },
                new
                {
                    Id = (int)CatalogueUtilities.TargetType.Youth,
                    Name = "Thanh thiếu niên (12 đến 18 tuổi)"
                },
                new
                {
                    Id = (int)CatalogueUtilities.TargetType.Adult,
                    Name = "Người lớn"
                },
                new
                {
                    Id = (int)CatalogueUtilities.TargetType.Elder,
                    Name = "Người già (trên 65 tuổi)"
                },
                new
                {
                    Id = (int)CatalogueUtilities.TargetType.Pregnant,
                    Name = "Phụ nữ mang thai"
                },
            };
            return new AppDomainResult()
            {
                Success = true,
                Data = listTargetType,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
