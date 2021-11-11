using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
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
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace MrApp.API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Description("Quản lý thông báo")]
    [Authorize]
    public class NotificationController : BaseController
    {
        private INotificationService notificationService;
        private INotificationApplicationUserService notificationApplicationUserService;

        public NotificationController(IServiceProvider serviceProvider, ILogger<BaseController> logger, IWebHostEnvironment env, IMapper mapper, IConfiguration configuration) : base(serviceProvider, logger, env, mapper, configuration)
        {
            notificationService = serviceProvider.GetRequiredService<INotificationService>();
            notificationApplicationUserService = serviceProvider.GetRequiredService<INotificationApplicationUserService>();
        }

        /// <summary>
        /// Lấy ra tổng số notification chưa đọc
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-total-notification-count")]
        [Authorize]
        public async Task<AppDomainResult> GetTotalNotificationCount()
        {
            int totalNotifications = 0;
            var userNotifications = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
            && !e.IsRead
            && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
            );
            if (userNotifications != null && userNotifications.Any())
                totalNotifications = userNotifications.Count();
            return new AppDomainResult()
            {
                Data = totalNotifications,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin thông báo của user
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-current-notifications")]
        [Authorize]
        public async Task<AppDomainResult> GetNotifications()
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                SearchNotification baseSearch = new SearchNotification();
                baseSearch.PageIndex = 1;
                baseSearch.PageSize = 20;
                //if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                //    baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                baseSearch.ToUserId = LoginContext.Instance.CurrentUser.UserId;
                PagedList<Notifications> pagedData = await this.notificationService.GetPagedListData(baseSearch);
                PagedList<NotificationModel> pagedDataModel = mapper.Map<PagedList<NotificationModel>>(pagedData);
                if (pagedDataModel != null && pagedDataModel.Items.Any())
                {
                    foreach (var item in pagedDataModel.Items)
                    {
                        var notificationApplicationUserInfos = await notificationApplicationUserService.GetAsync(e => e.NotificationId == item.Id);
                        if (notificationApplicationUserInfos != null && notificationApplicationUserInfos.Any())
                        {
                            item.IsRead = notificationApplicationUserInfos.FirstOrDefault().IsRead;
                            item.Content = notificationApplicationUserInfos.OrderByDescending(e => e.Created).FirstOrDefault().NotificationContent;
                        }
                    }
                }
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetPagedData([FromQuery] SearchNotification baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                baseSearch.ToUserId = LoginContext.Instance.CurrentUser.UserId;
                PagedList<Notifications> pagedData = await this.notificationService.GetPagedListData(baseSearch);
                PagedList<NotificationModel> pagedDataModel = mapper.Map<PagedList<NotificationModel>>(pagedData);
                if (pagedDataModel != null && pagedDataModel.Items.Any())
                {
                    foreach (var item in pagedDataModel.Items)
                    {
                        var notificationApplicationUserInfos = await notificationApplicationUserService.GetAsync(e => e.NotificationId == item.Id);
                        if (notificationApplicationUserInfos != null && notificationApplicationUserInfos.Any())
                        {
                            item.IsRead = notificationApplicationUserInfos.FirstOrDefault().IsRead;
                            item.Content = notificationApplicationUserInfos.OrderByDescending(e => e.Created).FirstOrDefault().NotificationContent;
                        }
                    }
                }
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Kiểm tra user đọc thông báo chưa
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpGet("read-notification")]
        [Authorize]
        public async Task<AppDomainResult> ReadNotification([FromQuery] int? notificationId)
        {
            bool success = true;
            var notificationUsers = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
            && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
            && (!notificationId.HasValue || e.NotificationId == notificationId)
            && !LoginContext.Instance.CurrentUser.HospitalId.HasValue
            );
            if (notificationUsers != null && notificationUsers.Any())
            {
                foreach (var item in notificationUsers)
                {
                    item.IsRead = true;
                    Expression<Func<NotificationApplicationUser, object>>[] includeProperties = new Expression<Func<NotificationApplicationUser, object>>[]
                    {
                        e => e.IsRead
                    };
                    success &= await this.notificationApplicationUserService.UpdateFieldAsync(item, includeProperties);
                }
            }
            else throw new AppException("Không có thông tin thông báo");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Đọc thông báo user
        /// </summary>
        /// <param name="notificationIds"></param>
        /// <returns></returns>
        [HttpPost("read-user-notifications")]
        public async Task<AppDomainResult> ReadNotifications([FromBody] List<int> notificationIds)
        {
            bool success = true;
            var notificationUsers = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
            && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
            && ((notificationIds == null || !notificationIds.Any()) || notificationIds.Contains(e.NotificationId))
            && !LoginContext.Instance.CurrentUser.HospitalId.HasValue
            );
            if (notificationUsers != null && notificationUsers.Any())
            {
                foreach (var item in notificationUsers)
                {
                    item.IsRead = true;
                    Expression<Func<NotificationApplicationUser, object>>[] includeProperties = new Expression<Func<NotificationApplicationUser, object>>[]
                    {
                        e => e.IsRead
                    };
                    success &= await this.notificationApplicationUserService.UpdateFieldAsync(item, includeProperties);
                }
            }
            else throw new AppException("Không có thông tin thông báo");
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Xóa thông báo của user
        /// </summary>
        /// <returns></returns>
        [HttpDelete("delete-user-notifications")]
        [MedicalAppAuthorize(new string[] { CoreContants.Delete })]
        public async Task<AppDomainResult> DeleteUserNotifications([FromBody] List<int> itemIds)
        {
            bool success = false;
            var currentUserId = LoginContext.Instance.CurrentUser.UserId;
            if (itemIds != null && itemIds.Any())
            {
                success = await this.notificationService.DeleteUserNotifications(itemIds, currentUserId, null);
            }
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK,
            };
        }

    }
}
