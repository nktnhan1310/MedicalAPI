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

    }
}
