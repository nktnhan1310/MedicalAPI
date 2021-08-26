using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    public class DashBoardSaleRequest
    {
        [DefaultValue(1)]
        public int PageIndex { get; set; }
        [DefaultValue(20)]
        public int PageSize { get; set; }
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// Loại tìm kiếm theo (ngày/tháng/năm)
        /// </summary>
        public int? SelectedType { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Tìm kiếm theo tháng
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Tìm kiếm theo năm
        /// </summary>
        public int? Year { get; set; }

    }
}
