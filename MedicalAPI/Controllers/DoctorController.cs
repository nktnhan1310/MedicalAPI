using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Utilities;
using Medical.Models;
using Medical.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Medical.Core.App.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace MedicalAPI.Controllers
{
    [Route("api/doctor")]
    [ApiController]
    [Description("Quản lý thông tin bác sĩ")]
    [Authorize]
    public class DoctorController : CoreHospitalController<Doctors, DoctorModel, SearchDoctor>
    {
        private readonly IDoctorDetailService doctorDetailService;
        private readonly IUserService userService;
        private readonly IUserGroupService userGroupService;
        private readonly IUserInGroupService userInGroupService;
        private readonly INotificationService notificationService;
        private readonly INotificationTemplateService notificationTemplateService;
        private readonly INotificationTypeService notificationTypeService;
        private readonly IHubContext<NotificationHub> hubContext;


        public DoctorController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<Doctors, DoctorModel, SearchDoctor>> logger, IWebHostEnvironment env, IHubContext<NotificationHub> hubContext) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IDoctorService>();
            this.doctorDetailService = serviceProvider.GetRequiredService<IDoctorDetailService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
            userInGroupService = serviceProvider.GetRequiredService<IUserInGroupService>();
            notificationService = serviceProvider.GetRequiredService<INotificationService>();
            notificationTemplateService = serviceProvider.GetRequiredService<INotificationTemplateService>();
            notificationTypeService = serviceProvider.GetRequiredService<INotificationTypeService>();
            this.hubContext = hubContext;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [MedicalAppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] DoctorModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.HospitalId.HasValue)
                    itemModel.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                itemModel.Created = DateTime.Now;
                itemModel.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                itemModel.Active = true;
                var item = mapper.Map<Doctors>(itemModel);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.domainService.CreateAsync(item);
                    if (success)
                    {
                        // ------------------------- TẠO NOTIFY CHO USER
                        var defaultTemplateCreateDoctors = await this.notificationTemplateService.GetAsync(e => e.Code == CoreContants.NOTI_TEMPLATE_DOCTOR_CREATE);
                        var notificationTypeHospitals = await this.notificationTypeService.GetAsync(e => e.Code == CatalogueUtilities.NotificationType.HOS.ToString());
                        NotificationTemplates notificationTemplates = null;
                        int? notificationTemplateId = null;
                        int? notificationTypeId = null;
                        if(defaultTemplateCreateDoctors != null && defaultTemplateCreateDoctors.Any())
                        {
                            notificationTemplates = defaultTemplateCreateDoctors.FirstOrDefault();
                            notificationTemplateId = defaultTemplateCreateDoctors.FirstOrDefault().Id;
                        }
                        if (notificationTypeHospitals != null && notificationTypeHospitals.Any())
                            notificationTypeId = notificationTypeHospitals.FirstOrDefault().Id;
                        Notifications notifications = new Notifications()
                        {
                            Active = true,
                            Deleted = false,
                            Created = DateTime.Now,
                            CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                            FromUserId = LoginContext.Instance.CurrentUser.UserId,
                            IsRead = false,
                            IsSendNotify = false,
                            HospitalId = LoginContext.Instance.CurrentUser.HospitalId,
                            NotificationTemplateId = notificationTemplateId,
                            NotificationTypeId = notificationTypeId,
                            WebUrl = string.Empty,
                            AppUrl = string.Empty
                        };
                        bool successNotify = await this.notificationService.CreateAsync(notifications);

                        // Tạo notify cho user
                        if(item.UserId.HasValue && item.UserId.Value > 0)
                        {
                            NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                            {
                                Active = true,
                                Deleted = false,
                                Created = DateTime.Now,
                                CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                NotificationId = notifications.Id,
                                HospitalId = LoginContext.Instance.CurrentUser.HospitalId,
                                ToUserId = item.UserId,
                                IsRead = false,
                            };
                        }


                        // HUB GỬI NOTIFY
                        if (successNotify)
                            await hubContext.Clients.All.SendAsync(CoreContants.GET_TOTAL_NOTIFICATION);
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
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
        /// Lấy thông tin user chưa được phân bố của bệnh viện
        /// </summary>
        /// <param name="hospitaId"></param>
        /// <returns></returns>
        [HttpGet("get-user-hospital-infos")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserInfos(int? hospitaId)
        {
            // Lấy thông tin group bác sĩ
            List<int> groupDoctorIds = new List<int>();
            var doctorGroupRoleInfos = await this.userGroupService.GetAsync(e => (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId) && e.Code == "BS");
            if (doctorGroupRoleInfos != null && doctorGroupRoleInfos.Any())
            {
                groupDoctorIds = doctorGroupRoleInfos.Select(e => e.Id).ToList();
                var userInGroupDoctors = await this.userInGroupService.GetAsync(e => !e.Deleted && groupDoctorIds.Contains(e.UserGroupId));
                if(userInGroupDoctors != null && userInGroupDoctors.Any())
                {
                    var userInGroupDoctorIds = userInGroupDoctors.Select(e => e.UserId).Distinct().ToList();
                    // Lấy tất cả user đã được phân bố cho bệnh viện
                    var doctorUserIds = this.domainService.Get(e => !e.Deleted
                    && e.UserId.HasValue
                    && (!hospitaId.HasValue || e.HospitalId == hospitaId)
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId.Value)
                    ).Select(e => e.UserId).ToList();

                    var userInfos = await this.userService.GetAsync(e => !e.Deleted
                    && e.HospitalId.HasValue
                    && !e.IsAdmin
                    && userInGroupDoctorIds.Contains(e.Id)
                    && e.Id != LoginContext.Instance.CurrentUser.UserId
                    && (!hospitaId.HasValue || e.HospitalId == hospitaId)
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.HospitalId == LoginContext.Instance.CurrentUser.HospitalId.Value)
                    && !doctorUserIds.Contains(e.Id)
                    , e => new Users()
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Phone = e.Phone,
                        Email = e.Email,
                        HospitalId = e.HospitalId
                    });
                    return new AppDomainResult()
                    {
                        Data = mapper.Map<IList<UserModel>>(userInfos),
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new AppException("Không có tài khoản bác sĩ thích hợp");
            }
            else throw new AppException("Không có tài khoản bác sĩ thích hợp");
        }

        /// <summary>
        /// Lấy thông tin theo bác sĩ
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
                    var itemModel = mapper.Map<DoctorModel>(item);
                    var doctorDetails = await this.doctorDetailService.GetAsync(e => !e.Deleted && e.DoctorId == id);
                    var doctorDetailModels = mapper.Map<IList<DoctorDetailModel>>(doctorDetails);
                    itemModel.DoctorDetails = doctorDetailModels;
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
    }
}
