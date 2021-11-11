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
    [Route("api/allergy-type")]
    [ApiController]
    [Description("Quản lý loại dị ứng")]
    [Authorize]
    public class AllergyTypeController : CatalogueController<AllergyTypes, AllergyTypeModel, BaseSearch>
    {
        private IAllergyDescriptionTypeService allergyDescriptionTypeService;
        public AllergyTypeController(IServiceProvider serviceProvider, ILogger<BaseController<AllergyTypes, AllergyTypeModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IAllergyTypeService>();
            allergyDescriptionTypeService = serviceProvider.GetRequiredService<IAllergyDescriptionTypeService>();
        }

        /// <summary>
        /// Lấy thông tin chi tiết item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            if (id <= 0) throw new AppException("Mã id không hợp lệ");
            var existItem = await this.catalogueService.GetByIdAsync(id);
            if (existItem == null) throw new AppException("Không tìm thấy thông tin item");
            var itemModel = mapper.Map<AllergyTypeModel>(existItem);
            // Lấy thông tin mô tả của dị ứng
            var allergyDescriptionTypes = await this.allergyDescriptionTypeService.GetAsync(e => !e.Deleted && e.AllergyTypeId == existItem.Id);
            if (allergyDescriptionTypes != null && allergyDescriptionTypes.Any())
                itemModel.AllergyDescriptionTypes = mapper.Map<IList<AllergyDescriptionTypeModel>>(allergyDescriptionTypes);
            return new AppDomainResult
            {
                Data = itemModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
