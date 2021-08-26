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
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Description("Thông báo hệ thống")]
    [Authorize]
    public class NotificationController : CoreHospitalController<Notifications, NotificationModel, SearchNotification>
    {
        private readonly INotificationTypeService notificationTypeService;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IUserGroupService userGroupService;
        private readonly IHospitalService hospitalService;
        private readonly IUserService userService;
        private readonly INotificationApplicationUserService notificationApplicationUserService;
        private readonly INotificationService notificationService;

        public NotificationController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<Notifications, NotificationModel, SearchNotification>> logger, IWebHostEnvironment env, IHubContext<NotificationHub> _notificationHubContext) : base(serviceProvider, logger, env)
        {
            this.domainService = this.serviceProvider.GetRequiredService<INotificationService>();
            notificationTypeService = this.serviceProvider.GetRequiredService<INotificationTypeService>();
            this._notificationHubContext = _notificationHubContext;
            userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            notificationApplicationUserService = serviceProvider.GetRequiredService<INotificationApplicationUserService>();
            notificationService = serviceProvider.GetRequiredService<INotificationService>();
        }

        /// <summary>
        /// Lấy thông tin theo id thông báo
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
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<NotificationModel>(item);
                    var notificationApplicationUsers = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted && e.NotificationId == itemModel.Id, e => new NotificationApplicationUser()
                    {
                        UserGroupId = e.UserGroupId,
                        ToUserId = e.ToUserId,
                        HospitalId = e.HospitalId
                    });
                    if(notificationApplicationUsers != null && notificationApplicationUsers.Any())
                    {
                        itemModel.HospitalIds = notificationApplicationUsers.Where(e => e.HospitalId.HasValue).Select(e => e.HospitalId.Value).ToList();
                        itemModel.UserGroupIds = notificationApplicationUsers.Where(e => e.UserGroupId.HasValue).Select(e => e.UserGroupId.Value).ToList();
                        itemModel.UserIds = notificationApplicationUsers.Where(e => e.ToUserId.HasValue).Select(e => e.ToUserId.Value).ToList();
                    }
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Item không tồn tại");
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách bệnh viện hiện tại
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-list-hospital")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetListHospitals()
        {
            var hospitals = await this.hospitalService.GetAsync(e => !e.Deleted
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.Id == LoginContext.Instance.CurrentUser.HospitalId)
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<HospitalModel>>(hospitals),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
        /// <summary>
        /// Lấy thông tin danh sách nhóm user
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-list-user-group")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetUserGroupInfos()
        {
            var userGroups = await this.userGroupService.GetAsync(e => !e.Deleted
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<UserGroupModel>>(userGroups),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Thêm mới thông báo
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] NotificationModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                {
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                }
                else itemModel.HospitalId = null;
                itemModel.FromUserId = LoginContext.Instance.CurrentUser.UserId;
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemModel.Active = true;
                var item = mapper.Map<Notifications>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.domainService.CreateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    if (item.Active && !item.IsSendNotify)
                    {
                        await _notificationHubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new AppException("Item không tồn tại");
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin notification
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem(int id, [FromBody] NotificationModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                {
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                }
                else itemModel.HospitalId = null;
                itemModel.FromUserId = LoginContext.Instance.CurrentUser.UserId;
                itemModel.Updated = DateTime.Now;
                itemModel.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                var item = mapper.Map<Notifications>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.domainService.UpdateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.Success = success;
                    if (item.Active && !item.IsSendNotify)
                    {
                        await _notificationHubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                    }
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
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
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
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
        //[HttpGet("get-current-notifications")]
        //[Authorize]
        //public async Task<AppDomainResult> GetNotifications()
        //{
        //    IList<NotificationModel> notificationModels = new List<NotificationModel>();
        //    var userNotificationApplications = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
        //    && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
        //    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
        //    );
        //    if (userNotificationApplications != null && userNotificationApplications.Any())
        //    {
        //        List<Notifications> notifications = new List<Notifications>();
        //        var notificationReadIds = userNotificationApplications.Where(e => e.IsRead).Select(e => e.NotificationId).Distinct().ToList();
        //        var notificationUnReadIds = userNotificationApplications.Where(e => !e.IsRead).Select(e => e.NotificationId).Distinct().ToList();
        //        if (notificationReadIds != null && notificationReadIds.Any())
        //        {
        //            Expression<Func<Notifications, Notifications>> select = e => new Notifications()
        //            {
        //                Id = e.Id,
        //                Active = e.Active,
        //                IsRead = true,
        //                NotificationTemplateId = e.NotificationTemplateId,
        //                Content = e.Content,
        //                Created = e.Created,
        //                CreatedBy = e.CreatedBy,
        //                Title = e.Title,
        //            };
        //            var notificationReadInfos = await this.domainService.GetAsync(e => !e.Deleted && notificationReadIds.Contains(e.Id), select);
        //            notifications.AddRange(notificationReadInfos);
        //        }
        //        if (notificationUnReadIds != null && notificationUnReadIds.Any())
        //        {
        //            Expression<Func<Notifications, Notifications>> select = e => new Notifications()
        //            {
        //                Id = e.Id,
        //                Active = e.Active,
        //                IsRead = false,
        //                Content = e.Content,
        //                Created = e.Created,
        //                CreatedBy = e.CreatedBy,
        //                Title = e.Title,
        //                NotificationTemplateId = e.NotificationTemplateId,
        //            };
        //            var notificationUnReadInfos = await this.domainService.GetAsync(e => !e.Deleted && notificationUnReadIds.Contains(e.Id), select);
        //            notifications.AddRange(notificationUnReadInfos);
        //        }
        //        notificationModels = mapper.Map<List<NotificationModel>>(notifications);
        //    }
        //    else throw new AppException("Người dùng hiện tại không có thông báo");
        //    return new AppDomainResult()
        //    {
        //        Data = notificationModels,
        //        Success = true,
        //        ResultCode = (int)HttpStatusCode.OK
        //    };
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-data")]
        [MedicalAppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> GetPagedData([FromQuery] SearchNotification baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                baseSearch.ToUserId = LoginContext.Instance.CurrentUser.UserId;
                PagedList<Notifications> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<NotificationModel> pagedDataModel = mapper.Map<PagedList<NotificationModel>>(pagedData);
                if(pagedDataModel != null && pagedDataModel.Items.Any())
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
            bool success = false;
            var notificationUsers = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
            && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
            && (!notificationId.HasValue || e.NotificationId == notificationId)
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
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
            bool success = false;
            var notificationUsers = await this.notificationApplicationUserService.GetAsync(e => !e.Deleted
            && e.ToUserId == LoginContext.Instance.CurrentUser.UserId
            && ((notificationIds == null || !notificationIds.Any()) || notificationIds.Contains(e.NotificationId))
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId)
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
            int? hospitalId = LoginContext.Instance.CurrentUser.HospitalId.HasValue ? LoginContext.Instance.CurrentUser.HospitalId : null;
            if(itemIds != null && itemIds.Any())
            {
                success = await this.notificationService.DeleteUserNotifications(itemIds, currentUserId, hospitalId);
            }
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK,
            };
        }
    }
}
