using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchBaseLocation : BaseSearch
    {
        /// <summary>
        /// Search theo quốc gia
        /// </summary>
        public int? CountryId { get; set; }
        /// <summary>
        /// Search theo thành phố
        /// </summary>
        public int? CityId { get; set; }
        /// <summary>
        /// Search theo quận
        /// </summary>
        public int? DistrictId { get; set; }
        /// <summary>
        /// Search theo phường
        /// </summary>
        public int? WardId { get; set; }
    }
}
