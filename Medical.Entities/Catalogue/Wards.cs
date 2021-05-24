using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Xã/phường
    /// </summary>
    [Table("Wards")]
    public class Wards : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Mã quận
        /// </summary>
        public int? DistrictId { get; set; }

        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        [StringLength(1000)]
        public string CityName { get; set; }

        /// <summary>
        /// Tên quận trực thuộc
        /// </summary>
        [StringLength(1000)]
        public string DistrictName { get; set; }
    }
}
