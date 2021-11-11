using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        
        /// <summary>
        /// Loại hình thông báo
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Mã phiếu khám (nếu có)
        /// </summary>
        public string ExaminationFormIds { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách mã phiếu của thông báo
        /// </summary>
        [NotMapped]
        public List<int> ExaminationFormSplitIds
        {
            get
            {
                if(!string.IsNullOrEmpty(ExaminationFormIds))
                {
                    var listExaminationFormId = ExaminationFormIds.Split(";").Select(e => Convert.ToInt32(e)).ToList();
                    return listExaminationFormId;
                }
                return null;
            }
        }

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

        /// <summary>
        /// Tiêu đề
        /// </summary>
        [NotMapped]
        public string Title { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        [NotMapped]
        public string Content { get; set; }

        #endregion

    }
}
