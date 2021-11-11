using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
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
    [Route("api/medical-record-history")]
    [ApiController]
    [Authorize]
    [Description("Quản lý tiền sử/tiền sử phẫu thuật")]
    public class MedicalRecordHistoryController : BaseController<MedicalRecordHistories, MedicalRecordHistoryModel, SearchMedicalRecordHistory>
    {
        protected IUserService userService;
        public MedicalRecordHistoryController(IServiceProvider serviceProvider, ILogger<BaseController<MedicalRecordHistories, MedicalRecordHistoryModel, SearchMedicalRecordHistory>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMedicalRecordHistoryService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
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
                var itemModel = mapper.Map<MedicalRecordHistoryModel>(item);
                var userInfo = await this.userService.GetByIdAsync(LoginContext.Instance.CurrentUser.UserId);
                if (userInfo != null)
                {
                    itemModel.UserFullName = userInfo.LastName + " " + userInfo.FirstName;
                    itemModel.UserPhone = userInfo.Phone;
                    itemModel.UserEmail = userInfo.Email;
                }
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
