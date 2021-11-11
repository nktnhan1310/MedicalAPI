using Medical.Core.App.Controllers;
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
    [Route("api/user-allergy")]
    [ApiController]
    [Description("Quản lý nhóm dị ứng của user")]
    [Authorize]
    public class UserAllergyController : BaseController<UserAllergies, UserAllergyModel, SearchUserAllergy>
    {
        private IUserService userService;
        private IAllergyTypeService allergyTypeService;
        public UserAllergyController(IServiceProvider serviceProvider, ILogger<BaseController<UserAllergies, UserAllergyModel, SearchUserAllergy>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserAllergyService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.allergyTypeService = serviceProvider.GetRequiredService<IAllergyTypeService>();
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
                var itemModel = mapper.Map<UserAllergyModel>(item);
                var allergyTypeInfos = await this.allergyTypeService.GetAsync(e => !e.Deleted && e.Active && e.Id == itemModel.AllergyTypeId);
                if (allergyTypeInfos != null && allergyTypeInfos.Any())
                    itemModel.AllergyTypeName = allergyTypeInfos.FirstOrDefault().Name;

                var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Active && e.Id == itemModel.UserId);
                if (userInfos != null && userInfos.Any())
                    itemModel.UserFullName = userInfos.FirstOrDefault().LastName + " " + userInfos.FirstOrDefault().FirstName;


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
