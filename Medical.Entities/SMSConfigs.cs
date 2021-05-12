using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Cấu hình gửi SMS
    /// </summary>
    [Table("SMSConfig")]
    public class SMSConfigs : MedicalAppDomain
    {
        /// <summary>
        /// Theo bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// Loại tin nhắn
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Tổng số SMS gửi theo cấu hình thời gian
        /// </summary>
        public int TotalSMS { get; set; }
        /// <summary>
        /// Tổng số phút gửi mỗi đợt
        /// </summary>
        public int MinutePerSending { get; set; }
        /// <summary>
        /// Đường dẫn webservice
        /// </summary>
        public string WebServiceUrl { get; set; }
        /// <summary>
        /// Key chứng thực SMS
        /// </summary>
        public string AuthorizationKey { get; set; }
        /// <summary>
        /// Tin nhắn có dấu không?
        /// </summary>
        public bool IsUnicode { get; set; }
        /// <summary>
        /// Có mã hóa không?
        /// </summary>
        public bool IsEncoding { get; set; }
        /// <summary>
        /// Mẫu gửi
        /// </summary>
        public string Template { get; set; }

    }
}
