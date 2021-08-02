using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Models
{
    /// <summary>
    /// Cấu hình momo
    /// </summary>
    public class MomoConfigurationModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Thông tin partner của momo
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập Partner Code")]
        public string PartnerCode { get; set; }
        /// <summary>
        /// Key access của momo
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập AccessKey")]
        [DataType(DataType.Password)]
        public string AccessKey { get; set; }
        /// <summary>
        /// Khóa bí mật
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập SecretKey")]
        [DataType(DataType.Password)]
        public string SecretKey { get; set; }
        /// <summary>
        /// Link trả về khi thanh toán thành công của web app
        /// </summary>
        public string ReturnUrlWebApp { get; set; }
        /// <summary>
        /// Link trả về khi thanh toán thành công trên app
        /// </summary>
        public string AppLinkMobileApp { get; set; }
        /// <summary>
        /// Link thông báo thanh toán thành công
        /// </summary>
        public string NotifyUrl { get; set; }
    }
}
