using Medical.Entities.DomainEntity;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bảng cấu hình Email
    /// </summary>
    public class EmailConfiguration : MedicalAppDomain
    {
        [Required]
        [MaxLength(1000)]
        public string SmtpServer { set; get; }
        [Required]
        public int Port { set; get; }
        [Required]
        public bool EnableSsl { set; get; }
        [Required]
        public int ConnectType { set; get; }
        [Required]
        [MaxLength(1000)]
        public string DisplayName { set; get; }
        [Required]
        [MaxLength(1000)]
        public string UserName { set; get; }
        [MaxLength(1000)]
        public string Email { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Password { set; get; }
        public int ItemSendCount { get; set; }
        public int TimeSend { get; set; }

        

    }
}
