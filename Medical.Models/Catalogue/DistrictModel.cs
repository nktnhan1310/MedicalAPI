using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class DistrictModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        public string CityName { get; set; }
    }
}
