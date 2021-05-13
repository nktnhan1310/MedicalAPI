using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
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
        public int DistricId { get; set; }
    }
}
