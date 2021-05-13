using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Tìm kiếm lịch khám
    /// </summary>
    public class SearchExaminationSchedule : BaseSearch
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }
    }
}
