using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Medical.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class NotificationService : CoreHospitalService<Notifications, SearchNotification>, INotificationService
    {
        private readonly IMedicalDbContext Context;

        public NotificationService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IMedicalDbContext Context) : base(unitOfWork, mapper)
        {
            this.Context = Context;
        }

        protected override string GetStoreProcName()
        {
            return "Notification_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchNotification baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@FromUserId", baseSearch.FromUserId),
                new SqlParameter("@ToUserId", baseSearch.ToUserId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@NotificationTypeId", baseSearch.NotificationTypeId),
                new SqlParameter("@NotificationId", baseSearch.NotificationId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Thêm mới thông báo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Notifications item)
        {
            bool result = false;
            if (item != null)
            {
                item.Id = 0;
                await this.unitOfWork.Repository<Notifications>().CreateAsync(item);
                await this.unitOfWork.SaveAsync();

                // ---------------- Tạo thông tin báo cáo cho user nếu active notification
                if (item.Active && !item.IsSendNotify)
                    await CreateNotifyUserData(item);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin thông báo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Notifications item)
        {
            bool result = false;
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();

            if (exists != null)
            {
                exists = mapper.Map<Notifications>(item);
                unitOfWork.Repository<Notifications>().Update(exists);
            }
            await unitOfWork.SaveAsync();
            result = true;
            if (exists.Active && !exists.IsSendNotify)
                await CreateNotifyUserData(item);
            return result;
        }

        /// <summary>
        /// Tạo thông tin thông báo cho user theo điều kiện tương ứng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task CreateNotifyUserData(Notifications item)
        {
            await Task.Run(() =>
            {
                object obj = new object();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    if (item.UserIds == null)
                        item.UserIds = new List<int>();
                    if (item.HospitalIds == null)
                        item.HospitalIds = new List<int>();
                    if (item.UserGroupIds == null)
                        item.UserGroupIds = new List<int>();
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NotificationId", item.Id),
                        new SqlParameter("@FromUserId", item.FromUserId),
                        new SqlParameter("@UserIds", string.Join(",", item.UserIds)),
                        new SqlParameter("@HospitalId", item.HospitalId),
                        new SqlParameter("@HospitalIds", string.Join(",", item.HospitalIds)),
                        new SqlParameter("@UserGroupIds", string.Join(",", item.UserGroupIds)),
                        new SqlParameter("@CreatedBy", item.CreatedBy)
                    };

                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "Nofification_CreateData";
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }

        /// <summary>
        /// Xóa thông tin user thông báo
        /// </summary>
        /// <param name="notificationIds"></param>
        /// <param name="userId"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserNotifications(List<int> notificationIds, int userId, int? hospitalId)
        {
            bool success = false;
            foreach (var notificationId in notificationIds)
            {
                var notificationUsers = await this.unitOfWork.Repository<NotificationApplicationUser>().GetQueryable().Where(e => !e.Deleted
            && e.ToUserId == userId
            && e.NotificationId == notificationId
            && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
            ).ToListAsync();
                if (notificationUsers != null && notificationUsers.Any())
                {
                    foreach (var notificationUser in notificationUsers)
                    {
                        this.unitOfWork.Repository<NotificationApplicationUser>().Delete(notificationUser);
                    }
                    await this.unitOfWork.SaveAsync();
                    success = true;
                }
            }
            return success;
        }


        /// <summary>
        /// Tạo custom notification trong hệ thống
        /// </summary>
        /// <param name="fromUserId">Gửi từ user</param>
        /// <param name="hospitalId">Mã bệnh viện</param>
        /// <param name="toUserIds">List user nhận</param>
        /// <param name="webUrl">Link web URL</param>
        /// <param name="appUrl">Link App Url</param>
        /// <param name="createdBy">Được tạo bởi</param>
        /// <param name="notificationTypeCode">Loại thông báo</param>
        /// <param name="defaultTemplateCode">Mã code mẫu template mặc định</param>
        /// <returns></returns>
        public async Task<bool> CreateCustomNotificationUser(
            int? fromUserId // Được gửi từ user
            , int? hospitalId // Mã bệnh viện
            , List<int> toUserIds // Gửi đến user nào
            , string webUrl // Link web
            , string appUrl // Link app
            , string createdBy // Được tạo bởi
            , bool isMrApp = false
            , string notificationTypeCode = "USER" // Loại thông báo
            , string defaultTemplateCode = "") //Template mặc định
        {
            bool success = false;
            int? notificationTypeId = null;

            var notificationTypeUserInfo = await this.unitOfWork.Repository<NotificationTypes>().GetQueryable().Where(e => e.Code == CatalogueUtilities.NotificationType.USER.ToString()).FirstOrDefaultAsync();
            if (notificationTypeUserInfo != null) notificationTypeId = notificationTypeUserInfo.Id;

            // Lấy thông tin mẫu default template theo mặc định
            var notifyTemplateInfo = await this.unitOfWork.Repository<NotificationTemplates>().GetQueryable().Where(e => e.Code == defaultTemplateCode).FirstOrDefaultAsync();

            if (toUserIds != null && toUserIds.Any())
            {
                if (notifyTemplateInfo != null)
                {
                    Notifications notifications = new Notifications()
                    {
                        Active = true,
                        Deleted = false,
                        Created = DateTime.Now,
                        CreatedBy = createdBy,
                        AppUrl = appUrl,
                        WebUrl = webUrl,
                        FromUserId = fromUserId,
                        HospitalId = hospitalId,
                        IsRead = false,
                        IsSendNotify = false,
                        NotificationTemplateId = notifyTemplateInfo.Id,
                    };
                    await this.unitOfWork.Repository<Notifications>().CreateAsync(notifications);
                    await this.unitOfWork.SaveAsync();
                    switch (defaultTemplateCode)
                    {
                        // 1.Khởi tạo thông tin bác sĩ bác sĩ
                        case CoreContants.NOTI_TEMPLATE_DOCTOR_CREATE:
                            {
                                foreach (var toUserId in toUserIds)
                                {

                                    NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                    {
                                         Active =  true,
                                         Deleted = false,
                                         IsRead = false,
                                         Created = DateTime.Now,
                                         CreatedBy = createdBy,
                                         HospitalId = hospitalId,
                                         NotificationContent = notifyTemplateInfo.Content,
                                         NotificationId = notifications.Id,
                                         ToUserId = toUserId,
                                    };
                                    await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                }
                                await this.unitOfWork.SaveAsync();
                            }
                            break;
                        // 2.Thông báo hồ sơ bệnh án cho user (Nếu admin tạo)
                        case CoreContants.NOTI_TEMPLATE_MEDICAL_RECORD_CREATE:
                            {
                                foreach (var toUserId in toUserIds)
                                {
                                    NotificationApplicationUser notificationApplicationUser = new NotificationApplicationUser()
                                    {
                                        Active = true,
                                        Deleted = false,
                                        IsRead = false,
                                        Created = DateTime.Now,
                                        CreatedBy = createdBy,
                                        HospitalId = hospitalId,
                                        NotificationContent = notifyTemplateInfo.Content,
                                        NotificationId = notifications.Id,
                                        ToUserId = toUserId,
                                    };
                                    await this.unitOfWork.Repository<NotificationApplicationUser>().CreateAsync(notificationApplicationUser);
                                }
                                await this.unitOfWork.SaveAsync();
                            }
                            break;

                        // 3.Tạo phiếu khám bệnh (Examination Form)
                        case CoreContants.NOTI_TEMPLATE_EXAMINATION_FORM_CREATE:
                            {
                                // 3.1: Admin tạo => thông báo cho user, doctor
                                if (!isMrApp)
                                {

                                }
                                // 3.2: User tạo => tạo thông tin noti cho doctor
                                else
                                {

                                }
                            }
                            break;
                        // 4. Cập nhật thông tin phiếu khám (Examination form info)
                        // 4.1: Bác sĩ cập nhật thông tin phiếu khám (Chờ tái khám) => thông báo cho user
                        // 4.2: Xác nhận thông tin tái khám => thông báo cho doctor
                        // 4.3: Admin cập nhật thông tin phiếu thành Đã xác nhận/Đã xác nhận tái khám => thông báo cho user + bác sĩ
                        // 4.4: Thanh toán MOMO/Online => Thông báo cho user + doctor

                        default:
                            break;
                    }
                }
                else throw new AppException("Không tìm thấy mẫu template");
            }
            else throw new AppException("Không có thông tin user gửi đi");

            return success;
        }

    }
}
