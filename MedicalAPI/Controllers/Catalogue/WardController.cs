﻿using Medical.Core.App.Controllers;
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
    [Route("api/wards")]
    [ApiController]
    [Description("Quản lý phường/xã")]
    [Authorize]
    public class WardController : CatalogueController<Wards, WardModel, SearchBaseLocation>
    {
        public WardController(IServiceProvider serviceProvider, ILogger<CatalogueController<Wards, WardModel, SearchBaseLocation>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IWardService>();
        }

        /// <summary>
        /// Lấy thông tin phường/xã theo mã thành phố
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet("get-by-city-id/{cityId}")]
        public async Task<AppDomainResult> GetByCityId(int cityId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var wards = await this.catalogueService.GetAsync(e => !e.Deleted && e.Active && e.CityId == cityId, e => new Wards()
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                CityId = e.CityId,
                CityName = e.CityName,
                DistrictId = e.DistrictId,
                DistrictName = e.DistrictName,
                Active = e.Active,
            });
            var wardModels = mapper.Map<IList<WardModel>>(wards);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = wardModels
            };
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin phường/xã theo mã quận/huyện
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        [HttpGet("get-by-district-id/{districtId}")]
        public async Task<AppDomainResult> GetByDistrictId(int districtId)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var wards = await this.catalogueService.GetAsync(e => !e.Deleted && e.Active && e.DistrictId == districtId, e => new Wards()
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                CityId = e.CityId,
                CityName = e.CityName,
                DistrictId = e.DistrictId,
                DistrictName = e.DistrictName,
                Active = e.Active,
            });
            var wardModels = mapper.Map<IList<WardModel>>(wards);
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                Data = wardModels
            };
            return appDomainResult;
        }


    }
}
