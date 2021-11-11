using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchSpecialListType : BaseHospitalSearch
    {
        /// <summary>
        /// Tổng hợp theo ngày khám bệnh
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Tìm theo mã trưởng khoa
        /// </summary>
        public int? ManagerId { get; set; }
    }
}
