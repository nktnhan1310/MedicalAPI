using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Dịch vụ của bệnh viện
    /// </summary>
    [Table("ServiceTypeMappingHospital")]
    public class ServiceTypeMappingHospital : MedicalAppDomainHospital
    {
        /// <summary>
        /// Dịch vụ
        /// </summary>
        public int ServiceTypeId { get; set; }
        /// <summary>
        /// Giá khám theo từng dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Cờ check dịch vụ có BHYT ko?
        /// </summary>
        public bool IsBHYT { get; set; }

        /// <summary>
        /// Tổng số lượt khám/ngày
        /// </summary>
        public int TotalVisitNo { get; set; }
    }
}
