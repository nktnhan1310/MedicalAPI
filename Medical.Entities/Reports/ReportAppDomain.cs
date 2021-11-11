using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class ReportAppDomain
    {
        public long RowNumber { get; set; }

        /// <summary>
        /// Tổng số item của báo cáo
        /// </summary>
        public long TotalItem { get; set; }
    }
}
