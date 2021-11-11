using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    public class ExaminationFormRawData
    {
        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? BirhthDate { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public string CertificateNo { get; set; }

    }
}
