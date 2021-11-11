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

namespace MedicalAPI.Controllers
{
    [Route("api/user-pregnancy")]
    [ApiController]
    [Description("Quản lý thông tin thai kì của user")]
    [Authorize]
    public class UserPregnancyController : BaseController<UserPregnancies, UserPregnancyModel, SearchUserPregnancy>
    {
        private IUserPregnancyDetailService userPregnancyDetailService;
        public UserPregnancyController(IServiceProvider serviceProvider, ILogger<BaseController<UserPregnancies, UserPregnancyModel, SearchUserPregnancy>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserPregnancyService>();
            userPregnancyDetailService = serviceProvider.GetRequiredService<IUserPregnancyDetailService>();
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
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var itemModel = mapper.Map<UserPregnancyModel>(item);
                var userPregnancyDetails = await this.userPregnancyDetailService.GetAsync(e => e.UserPregnancyId == id);
                if (userPregnancyDetails != null && userPregnancyDetails.Any())
                    itemModel.UserPregnancyDetails = mapper.Map<IList<UserPregnancyDetailModel>>(userPregnancyDetails);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }


    }
}
