using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationIndex
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int ServiceTypeId { get; set; }
        /// <summary>
        /// Tìm theo ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }
    }
}
