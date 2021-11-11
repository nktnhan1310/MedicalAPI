using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
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

namespace MedicalAPI.Controllers
{
    [Route("api/app-policy")]
    [ApiController]
    [Description("Quản lý chính sách của app")]
    public class AppPolicyController : CoreHospitalController<AppPolicies, AppPolicyModel, SearchAppPolicy>
    {
        private IAppPolicyDetailService appPolicyDetailService;
        public AppPolicyController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<AppPolicies, AppPolicyModel, SearchAppPolicy>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IAppPolicyService>();
            this.appPolicyDetailService = serviceProvider.GetRequiredService<IAppPolicyDetailService>();
        }

        /// <summary>
        /// Lấy thông tin chi tiết của chính sách
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            if (id <= 0) throw new AppException("Không tìm thấy thông tin item");
            var item = await this.domainService.GetByIdAsync(id);
            if (item == null) throw new AppException("Không tìm thấy thông tin item");
            var itemModel = mapper.Map<AppPolicyModel>(item);
            var policyDetailTask = this.appPolicyDetailService.GetAsync(e => !e.Deleted && e.AppPolicyId == item.Id);
            itemModel.AppPolicyDetails = mapper.Map<IList<AppPolicyDetailModel>>(await policyDetailTask);
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = itemModel
            };
        }

    }
}
