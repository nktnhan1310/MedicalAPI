using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class CityModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        public string CountryName { get; set; }
    }
}
