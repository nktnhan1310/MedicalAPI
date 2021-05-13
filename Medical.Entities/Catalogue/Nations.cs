using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Dân tộc
    /// </summary>
    [Table("Nations")]
    public class Nations : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int CountryId { get; set; }
    }
}
