using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Thông báo
    /// </summary>
    [Table("Notifications")]
    public class Notifications : MedicalAppDomainHospital
    {
        /// <summary>
        /// Loại thông báo
        /// </summary>
        public int? NotificationTypeId { get; set; }
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// gửi từ user
        /// </summary>
        public int? FromUserId { get; set; }

        [DefaultValue(false)]
        public bool IsSendNotify { get; set; }

        #region Extension Properties

        /// <summary>
        /// Cờ check thông báo đã được đọc chưa
        /// </summary>
        [NotMapped]
        public bool IsRead { get; set; }

        /// <summary>
        /// Từ user
        /// </summary>
        [NotMapped]
        public string FromUserName { get; set; }

        /// <summary>
        /// Loại thông báo
        /// </summary>
        [NotMapped]
        public string NotificationTypeName { get; set; }

        /// <summary>
        /// List thông báo theo user
        /// </summary>
        [NotMapped]
        public List<int> UserIds { get; set; }

        /// <summary>
        /// List thông báo theo hospital
        /// </summary>
        [NotMapped]
        public List<int> HospitalIds { get; set; }

        /// <summary>
        /// List thông báo theo nhóm người dùng
        /// </summary>
        [NotMapped]
        public List<int> UserGroupIds { get; set; }



        #endregion

    }
}
