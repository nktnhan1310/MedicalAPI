using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bảng cấu hình hệ thống
    /// </summary>
    public class SystemConfiguartions : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Nội dung chính sách
        /// </summary>
        public string Value { get; set; }
    }
}
