using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Http;
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
    [Route("api/system-config-fee")]
    [ApiController]
    [Description("Cấu hình phí tiện ích")]
    public class SystemConfigFeeController : ControllerBase
    {
        private IMapper mapper;
        private ISystemConfigFeeService systemConfigFeeService;
        public SystemConfigFeeController(IServiceProvider serviceProvider, IMapper mapper)
        {
            this.mapper = mapper;
            systemConfigFeeService = serviceProvider.GetRequiredService<ISystemConfigFeeService>();
        }

        /// <summary>
        /// Lấy thông tin cấu hình phí tiện ích
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-system-config-fee-configuration")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetSystemConfigFee()
        {
            SystemConfigFeeModel systemConfigFeeModel = new SystemConfigFeeModel();
            var systemConfigFees = await this.systemConfigFeeService.GetAsync(e => !e.Deleted && e.Active);
            if (systemConfigFees != null && systemConfigFees.Any())
                systemConfigFeeModel = mapper.Map<SystemConfigFeeModel>(systemConfigFees.FirstOrDefault());
            else
            {
                SystemConfigFee systemConfigFee = new SystemConfigFee()
                {
                    Active = true,
                    Deleted = false,
                    Created = DateTime.Now,
                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                    IsCheckRate = false
                };
                bool success = await this.systemConfigFeeService.CreateAsync(systemConfigFee);
                if (success) systemConfigFeeModel = mapper.Map<SystemConfigFeeModel>(systemConfigFee);
                else throw new AppException("Lấy thông tin cấu hình phí tiện ích thất bại");
            }
            return new AppDomainResult()
            {
                Data = systemConfigFeeModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật thông tin cấu hình phí tiện ích
        /// </summary>
        /// <param name="systemConfigFeeModel"></param>
        /// <returns></returns>
        [HttpPut("update-system-config-fee")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateSystemConfigFee([FromBody] SystemConfigFeeModel systemConfigFeeModel)
        {
            bool success = false;
            if (systemConfigFeeModel != null)
            {
                if (ModelState.IsValid)
                {
                    var systemConfigFee = mapper.Map<SystemConfigFee>(systemConfigFeeModel);
                    systemConfigFee.Updated = DateTime.Now;
                    systemConfigFee.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    var existItemMessage = await this.systemConfigFeeService.GetExistItemMessage(systemConfigFee);
                    if (!string.IsNullOrEmpty(existItemMessage))
                        throw new AppException(existItemMessage);
                    success = await this.systemConfigFeeService.UpdateAsync(systemConfigFee);
                    if (!success)
                        throw new AppException("Cập nhật thông tin cấu hình thất bại");
                }
                else throw new AppException(ModelState.GetErrorMessage());
            }
            else throw new AppException("Không có thông tin cập nhật");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
