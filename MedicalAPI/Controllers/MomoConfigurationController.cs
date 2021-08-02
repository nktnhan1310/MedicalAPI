using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/momo-configuration")]
    [ApiController]
    [Description("Cấu hình thanh toán momo")]
    [Authorize]
    public class MomoConfigurationController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMomoConfigurationService momoConfigurationService;

        public MomoConfigurationController(IServiceProvider serviceProvider, IMapper mapper)
        {
            momoConfigurationService = serviceProvider.GetRequiredService<IMomoConfigurationService>();
            this.mapper = mapper;
        }

        /// <summary>
        /// Lấy thông tin cấu hình momo
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-momo-configuration")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetMomoConfiguration()
        {
            MomoConfigurationModel momoConfigurationModel = new MomoConfigurationModel();
            var momoConfigurations = await this.momoConfigurationService.GetAsync(e => !e.Deleted && e.Active);
            if (momoConfigurations != null && momoConfigurations.Any())
                momoConfigurationModel = mapper.Map<MomoConfigurationModel>(momoConfigurations.FirstOrDefault());
            else
            {
                MomoConfigurations momoConfiguration = new MomoConfigurations()
                {
                    Active = true,
                    Deleted = false,
                    Created = DateTime.Now,
                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                };
                bool success = await this.momoConfigurationService.CreateAsync(momoConfiguration);
                if (success) momoConfigurationModel = mapper.Map<MomoConfigurationModel>(momoConfiguration);
                else throw new AppException("Lấy thông tin cấu hình Momo thất bại");
            }
            return new AppDomainResult()
            {
                Data = momoConfigurationModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật thông tin cấu hình momo
        /// </summary>
        /// <param name="momoConfigurationModel"></param>
        /// <returns></returns>
        [HttpPut("update-momo-configuration")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMomoConfiguration([FromBody] MomoConfigurationModel momoConfigurationModel)
        {
            bool success = false;
            if (momoConfigurationModel != null)
            {
                if (ModelState.IsValid)
                {
                    var momoConfiguration = mapper.Map<MomoConfigurations>(momoConfigurationModel);
                    momoConfiguration.Updated = DateTime.Now;
                    momoConfiguration.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    var existItemMessage = await this.momoConfigurationService.GetExistItemMessage(momoConfiguration);
                    if (!string.IsNullOrEmpty(existItemMessage))
                        throw new AppException(existItemMessage);
                    success = await this.momoConfigurationService.UpdateAsync(momoConfiguration);
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
