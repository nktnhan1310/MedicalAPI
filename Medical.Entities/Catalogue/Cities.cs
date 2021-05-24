using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Thành phố
    /// </summary>
    [Table("Cities")]
    public class Cities : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        [StringLength(1000)]
        public string CountryName { get; set; }
    }
}
