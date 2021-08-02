using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Tìm kiếm lịch khám
    /// </summary>
    public class SearchExaminationSchedule : BaseHospitalSearch
    {
        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Mã lịch trực
        /// </summary>
        public int? ExaminationScheduleId { get; set; }
    }
}
