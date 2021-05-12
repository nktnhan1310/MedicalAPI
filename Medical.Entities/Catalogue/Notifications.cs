using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Thông báo
    /// </summary>
    [Table("Notifications")]
    public class Notifications : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Loại thông báo
        /// </summary>
        public int NoficationTypeId { get; set; }
    }
}
