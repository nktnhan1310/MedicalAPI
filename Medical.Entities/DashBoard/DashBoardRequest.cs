using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class DashBoardRequest
    {
        /// <summary>
        /// Theo bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }
        /// <summary>
        /// Theo danh sách trạng thái
        /// </summary>
        public string StatusList { get; set; }
        /// <summary>
        /// Theo ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }
        /// <summary>
        /// Theo tháng
        /// </summary>
        public int? MonthValue { get; set; }
        /// <summary>
        /// Theo năm
        /// </summary>
        public int? YearValue { get; set; }
        public int? ServiceTypeId { get; set; }
        /// <summary>
        /// Kiểm tra user active không
        /// </summary>
        public bool IsCheckActiveUser { get; set; }

    }
}
