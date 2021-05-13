using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Cấu hình ca khám
    /// </summary>
    [Table("ConfigTimeExaminations")]
    public class ConfigTimeExaminations : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Buổi (sáng/chiều/tối)
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Giá trị ca
        /// </summary>
        [StringLength(1000)]
        public string Value { get; set; }
        /// <summary>
        /// Số thứ tự ca
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
    }
}
