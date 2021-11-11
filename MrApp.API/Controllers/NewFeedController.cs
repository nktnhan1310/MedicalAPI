using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.Services;
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
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/new-feed")]
    [ApiController]
    [Description("Quản lí tin tức")]
    [Authorize]
    public class NewFeedController : BaseController
    {
        private INewFeedService newFeedService;
        private IHospitalService hospitalService;
        public NewFeedController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            this.newFeedService = serviceProvider.GetRequiredService<INewFeedService>();
            this.hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
        }

        /// <summary>
        /// Lấy thông tin tin tức theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetById(int id)
        {
            NewFeedModel newFeedModel = new NewFeedModel();
            var newFeedInfo = await this.newFeedService.GetByIdAsync(id);
            if (newFeedInfo != null)
            {
                newFeedModel = mapper.Map<NewFeedModel>(newFeedInfo);
                if(newFeedModel.HospitalId.HasValue && newFeedModel.HospitalId.Value > 0)
                {
                    var hospitalInfo = await this.hospitalService.GetByIdAsync(newFeedModel.HospitalId.Value);
                    newFeedModel.HospitalName = hospitalInfo != null ? hospitalInfo.Name : string.Empty;
                }
            }
            return new AppDomainResult()
            {
                Data = newFeedModel,
                ResultCode = (int)HttpStatusCode.OK,
                Success = true,
            };
        }

        /// <summary>
        /// Lấy thông tin danh sách tin tức
        /// </summary>
        /// <param name="baseHospitalSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> Get([FromQuery] BaseHospitalSearch baseHospitalSearch)
        {
            PagedList<NewFeedModel> pagedListModel = new PagedList<NewFeedModel>();
            var pagedList = await this.newFeedService.GetPagedListData(baseHospitalSearch);
            if (pagedList != null && pagedList.Items.Any())
                pagedListModel = mapper.Map<PagedList<NewFeedModel>>(pagedList);

            return new AppDomainResult()
            {
                Success = true,
                Data = pagedListModel,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
