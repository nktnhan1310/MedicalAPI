using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Interface.Services;
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
using System.Threading.Tasks;

namespace MedicalAPI.Controllers.Catalogue
{
    [Route("api/districts")]
    [ApiController]
    [Description("Quản lý quận/huyện/xã")]
    [Authorize]
    public class DistrictController : CatalogueController<Districts, DistrictModel, SearchBaseLocation>
    {
        public DistrictController(IServiceProvider serviceProvider, ILogger<CatalogueController<Districts, DistrictModel, SearchBaseLocation>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IDistrictService>();
        }

        /// <summary>
        /// Lấy thông tin quận huyện theo mã thành phố
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet("get-by-city-id/{cityId}")]
        public async Task<AppDomainResult> GetByCityId(int cityId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var districts = await this.catalogueService.GetAsync(e => !e.Deleted && !e.Active && e.CityId == cityId, e => new Districts()
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                CityId = e.CityId,
                CityName = e.CityName,
                Active = e.Active,
            });
            var districtModels = mapper.Map<IList<DistrictModel>>(districts);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = districtModels
            };
            return appDomainResult;
        }
    }
}
