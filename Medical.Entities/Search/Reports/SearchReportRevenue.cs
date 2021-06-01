using Medical.Entities.DomainEntity.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchReportRevenue : ReportBaseSearch
    {
        /// <summary>
        /// Tìm theo mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }
    }
}
