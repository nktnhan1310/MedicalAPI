using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface INotificationService : ICoreHospitalService<Notifications, SearchNotification>
    {
        /// <summary>
        /// Xóa notification
        /// </summary>
        /// <param name="notificationIds"></param>
        /// <param name="userId"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        Task<bool> DeleteUserNotifications(List<int> notificationIds, int userId, int? hospitalId);

        /// <summary>
        /// Custom template thông báo
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="toUserIds"></param>
        /// <param name="webUrl"></param>
        /// <param name="appUrl"></param>
        /// <param name="createdBy"></param>
        /// <param name="examinationFormid"></param>
        /// <param name="isMrApp"></param>
        /// <param name="notificationTypeCode"></param>
        /// <param name="defaultTemplateCode"></param>
        /// <returns></returns>
        Task<bool> CreateCustomNotificationUser(
            int? fromUserId // Được gửi từ user
            , int? hospitalId // Mã bệnh viện
            , List<int> toUserIds // Gửi đến user nào
            , string webUrl // Link web
            , string appUrl // Link app
            , string createdBy // Được tạo bởi
            , int? examinationFormid // Mã phiếu hẹn
            , bool isMrApp = false
            , string notificationTypeCode = "USER" // Loại thông báo
            , string defaultTemplateCode = "" //Template mặc định
            , List<int> examinationformIds = null // Danh sách phiếu khám
            , List<int> examinationScheduleIds = null // Danh sách lịch trực
            , List<int> examinationScheduleDetailIds = null // Danh sách ca trực
            );

        /// <summary>
        /// CLEAR NỘI DUNG THÔNG BÁO NÀO CÓ THỜI HẠN LỚN HƠN 7 NGÀY
        /// </summary>
        /// <returns></returns>
        Task ClearNotificationDataJob();

    }
}
