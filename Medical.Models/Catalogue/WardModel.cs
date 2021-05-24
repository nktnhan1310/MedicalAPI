using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class WardModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Quận
        /// </summary>
        public int? DistrictId { get; set; }
    }
}
