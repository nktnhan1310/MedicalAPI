using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchSystemFile : BaseSearch
    {
        /// <summary>
        /// Loại file
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// Mã quảng cảo của hệ thống
        /// </summary>
        public int? SystemAdvertisementId { get; set; }
    }
}
