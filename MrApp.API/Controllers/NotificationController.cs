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
            List<NotificationModel> notificationModels = new List<NotificationModel>();
            var userNotificationApplications = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
            && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
            );
            if (userNotificationApplications != null && userNotificationApplications.Any())
            {
                List<Notifications> notifications = new List<Notifications>();
                var notificationReadIds = userNotificationApplications.Where(e => e.IsRead).Select(e => e.NotificationId).Distinct().ToList();
                var notificationUnReadIds = userNotificationApplications.Where(e => !e.IsRead).Select(e => e.NotificationId).Distinct().ToList();
                if (notificationReadIds != null && notificationReadIds.Any())
                {
                    Expression<Func<Notifications, Notifications>> select = e => new Notifications()
                    {
                        Id = e.Id,
                        Active = e.Active,
                        IsRead = true,
                        Content = e.Content,
                        Created = e.Created,
                        CreatedBy = e.CreatedBy,
                        Title = e.Title,
                    };
                    var notificationReadInfos = await this.notificationService.GetAsync(e => !e.Deleted && notificationReadIds.Contains(e.Id), select);
                    notifications.AddRange(notificationReadInfos);
                }
                if (notificationUnReadIds != null && notificationUnReadIds.Any())
                {
                    Expression<Func<Notifications, Notifications>> select = e => new Notifications()
                    {
                        Id = e.Id,
                        Active = e.Active,
                        IsRead = false,
                        Content = e.Content,
                        Created = e.Created,
                        CreatedBy = e.CreatedBy,
                        Title = e.Title,
                    };
                    var notificationUnReadInfos = await this.notificationService.GetAsync(e => !e.Deleted && notificationUnReadIds.Contains(e.Id), select);
                    notifications.AddRange(notificationUnReadInfos);
                }
                notificationModels = mapper.Map<List<NotificationModel>>(notifications);

            }
            else throw new AppException("Người dùng hiện tại không có thông báo");
            return new AppDomainResult()
            {
                Data = notificationModels,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Kiểm tra user đọc thông báo chưa
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="isReadAll"></param>
        /// <returns></returns>
        [HttpGet("read-notification")]
        [Authorize]
        public async Task<AppDomainResult> ReadNotification([FromQuery] int? notificationId, bool isReadAll = false)
        {
            bool success = false;
            var notificationUsers = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
            && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
            && (isReadAll || (!notificationId.HasValue || e.NotificationId == notificationId))
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

    }
}
