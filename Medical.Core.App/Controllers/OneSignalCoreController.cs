using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Medical.Core.App.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class OneSignalCoreController : ControllerBase
    {
        protected IConfiguration configuration;
        private IDeviceAppService deviceAppService;
        public OneSignalCoreController(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.deviceAppService = serviceProvider.GetRequiredService<IDeviceAppService>();
        }

        /// <summary>
        /// TEST đẩy thử notification cho user
        /// </summary>
        /// <param name="createNotificationModel"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<AppDomainResult> TestPushNotification([FromBody] CreateNotificationModel createNotificationModel)
        {
            Guid appId = Guid.Parse(this.configuration.GetSection(AppSettingKey.ONE_SIGNAL_APP_ID).Value);
            var apiKey = this.configuration.GetSection(AppSettingKey.ONE_SIGNAL_API_KEY).Value.ToString();
            string result = await OneSignalUtilities.OneSignalPushNotification(createNotificationModel, appId, apiKey);
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = result
            };
        }

        /// <summary>
        /// Đẩy thông tin dữ liệu của one signal => Lưu xuống db
        /// </summary>
        /// <param name="oneSignalDataModel"></param>
        /// <returns></returns>
        [HttpPost("post-user-one-signal-data")]
        public virtual async Task<AppDomainResult> UpdateUserPlayerId([FromBody] OneSignalDataModel oneSignalDataModel)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            if (LoginContext.Instance.CurrentUser == null || LoginContext.Instance.CurrentUser.UserId <= 0) throw new AppException("Tài khoản chưa đăng nhập!");

            Task<bool> taskResult = null;

            // Lấy ra thông tin playerId đã lưu của user
            var deviceAppInfo = await this.deviceAppService.GetSingleAsync(e => e.UserId == LoginContext.Instance.CurrentUser.UserId && e.PlayerId == oneSignalDataModel.PlayerId);
            // Thêm mới
            if (deviceAppInfo == null)
            {
                DeviceApps deviceApps = new DeviceApps()
                {
                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                    Created = DateTime.Now,
                    Active = true,
                    Deleted = false,
                    DeviceTypeId = oneSignalDataModel.TypeId ?? 0,
                    PlayerId = oneSignalDataModel.PlayerId,
                    UserId = LoginContext.Instance.CurrentUser.UserId
                };
                taskResult = this.deviceAppService.CreateAsync(deviceApps);
            }
            // Cập nhật
            else
            {
                deviceAppInfo.Updated = DateTime.Now;
                deviceAppInfo.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                deviceAppInfo.UserId = LoginContext.Instance.CurrentUser.UserId;
                deviceAppInfo.Active = oneSignalDataModel.Active ?? false;
                deviceAppInfo.DeviceTypeId = oneSignalDataModel.TypeId ?? 0;

                Expression<Func<DeviceApps, object>>[] includeProperties = new Expression<Func<DeviceApps, object>>[]
                {
                    e => e.Updated,
                    e => e.UpdatedBy,
                    e => e.UserId,
                    e => e.Active,
                    e => e.DeviceTypeId
                };

                taskResult = this.deviceAppService.UpdateFieldAsync(deviceAppInfo, includeProperties);
            }

            return new AppDomainResult()
            {
                Success = await taskResult,
                ResultCode = (int)HttpStatusCode.OK,
            };
        }
    }
}
