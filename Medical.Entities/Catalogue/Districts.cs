using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
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
        public int CityId { get; set; }
    }
}
