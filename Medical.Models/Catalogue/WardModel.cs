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

        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Tên quận trực thuộc
        /// </summary>
        public string DistrictName { get; set; }
    }
}
