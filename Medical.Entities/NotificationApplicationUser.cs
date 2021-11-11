using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class NotificationApplicationUser : MedicalAppDomainHospital
    {
        /// <summary>
        /// Theo thông báo nào
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Nội dung thông báo của user
        /// </summary>
        public string NotificationContent { get; set; }

        /// <summary>
        /// Gửi đến user
        /// </summary>
        public int? ToUserId { get; set; }
        /// <summary>
        /// Gửi nhóm người dùng
        /// </summary>
        public int? UserGroupId { get; set; }
        public bool IsRead { get; set; }

    }
}
