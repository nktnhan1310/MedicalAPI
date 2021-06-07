using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/sms-configuration")]
    [ApiController]
    [Description("Cấu hình SMS")]
    [Authorize]
    public class SMSConfigurationController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISMSConfigurationService sMSConfigurationService;
        public SMSConfigurationController(IServiceProvider serviceProvider, IMapper mapper)
        {
            this.sMSConfigurationService = serviceProvider.GetRequiredService<ISMSConfigurationService>();
            this.mapper = mapper;
        }

        /// <summary>
        /// Lấy thông tin cấu hình SMS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew, CoreContants.View })]
        public async Task<AppDomainResult> GetSMSConfiguration()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            // Lấy thông tin cấu hình
            var configurations = await this.sMSConfigurationService.GetAsync(e => !e.Deleted && e.Active);
            if (configurations != null && configurations.Any())
            {
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK,
                    Data = mapper.Map<SMSConfiguartionModel>(configurations.FirstOrDefault())
                };
            }
            // Chưa có cấu hình => tạo mới cấu hình
            else
            {
                SMSConfiguration emailConfiguration = new SMSConfiguration()
                {
                    Deleted = false,
                    Active = true,
                    Created = DateTime.Now,
                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                    SMSType = 2,
                };
                bool success = await this.sMSConfigurationService.CreateAsync(emailConfiguration);

                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK,
                    Data = mapper.Map<SMSConfiguartionModel>(emailConfiguration)
                };
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật cấu hình SMS
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateEmailConfiguration([FromBody] SMSConfiguartionModel sMSConfiguartionModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                var sMSConfiguration = mapper.Map<SMSConfiguration>(sMSConfiguartionModel);
                bool success = await this.sMSConfigurationService.UpdateAsync(sMSConfiguration);
                if (success)
                {
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK,
                    };
                }
                else throw new Exception("Lỗi trong quá trình xử lý!");
            }
            else throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }
    }
}
