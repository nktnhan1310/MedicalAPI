using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Cấu hình phí tiện ích
    /// </summary>
    public class SystemConfigFee : MedicalAppDomain
    {
        /// <summary>
        /// Phí tiện ích hệ thống
        /// </summary>
        public double? Fee { get; set; }
        /// <summary>
        /// Cờ check tính theo phần trăm phí dịch vụ
        /// </summary>
        [DefaultValue(false)]
        public bool IsCheckRate { get; set; }
        /// <summary>
        /// Tỉ lệ tính phí trên phí dịch vụ
        /// </summary>
        public double? Rate { get; set; }
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }
    }
}
