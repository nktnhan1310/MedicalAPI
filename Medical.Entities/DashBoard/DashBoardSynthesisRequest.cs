using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class DashBoardSynthesisRequest
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }
        /// <summary>
        /// List trạng thái phiếu
        /// </summary>
        public List<int> StatusIds { get; set; }

        /// <summary>
        /// Loại chọn
        /// 0 => năm
        /// != 0 => ngày
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
        /// Giá trị tháng tìm kiếm
        /// </summary>
        public int? MonthValue { get; set; }
        /// <summary>
        /// Giá trị năm
        /// </summary>
        public int? YearValue { get; set; }

        /// <summary>
        /// Theo loại dịch vụ
        /// </summary>
        public int? ServiceTypeId { get; set; }
    }
}
