using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Quận
    /// </summary>
    [Table("Districts")]
    public class Districts : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        [StringLength(1000)]
        public string CityName { get; set; }
    }
}
