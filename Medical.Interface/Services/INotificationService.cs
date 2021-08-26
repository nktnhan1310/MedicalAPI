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
        Task<bool> DeleteUserNotifications(List<int> notificationIds, int userId, int? hospitalId);
    }
}
