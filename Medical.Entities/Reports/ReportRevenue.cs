using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities.Reports
{
    public class ReportRevenue : ReportAppDomain
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public string HospitalCode { get; set; }

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }

        /// <summary>
        /// Tổng doanh thu theo bệnh viện
        /// </summary>
        public double? TotalPrice { get; set; }

        /// <summary>
        /// Giá tiền hiển thị
        /// </summary>
        public string TotalPriceDisplay
        {
            get
            {
                return TotalPrice.HasValue ? TotalPrice.Value.ToString("#,###") : string.Empty;
            }
        }
    }
}
