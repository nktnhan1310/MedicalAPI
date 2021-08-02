using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Cấu hình momo
    /// </summary>
    public class MomoConfigurations : MedicalAppDomainHospital
    {
        /// <summary>
        /// Thông tin partner của momo
        /// </summary>
        public string PartnerCode { get; set; }
        /// <summary>
        /// Key access của momo
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// Khóa bí mật
        /// </summary>
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
