using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    /// <summary>
    /// Dân tộc
    /// </summary>
    public class NationModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }
    }
}
