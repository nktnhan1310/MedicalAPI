using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Models
{
    /// <summary>
    /// Thông báo hệ thống
    /// </summary>
    public class NotificationModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Loại thông báo
        /// </summary>
        public int? NotificationTypeId { get; set; }

        /// <summary>
        /// Link web
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// Link app
        /// </summary>
        public string AppUrl { get; set; }

        /// <summary>
        /// gửi từ user
        /// </summary>
        public int? FromUserId { get; set; }

        [DefaultValue(false)]
        public bool IsSendNotify { get; set; }

        /// <summary>
        /// Mã template notification
        /// </summary>
        public int? NotificationTemplateId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Cờ check thông báo đã được đọc chưa
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Từ user
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// Loại thông báo
        /// </summary>
        public string NotificationTypeName { get; set; }

        /// <summary>
        /// List thông báo theo user
        /// </summary>
        public List<int> UserIds { get; set; }  

        /// <summary>
        /// List thông báo theo user
        /// </summary>
        public List<int> HospitalIds { get; set; }

        /// <summary>
        /// List thông báo theo nhóm người dùng
        /// </summary>
        public List<int> UserGroupIds { get; set; }

        #endregion
    }
}
