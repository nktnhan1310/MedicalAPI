using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationDate
    {
        /// <summary>
        /// Tìm kiếm theo bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }
        /// <summary>
        /// Tìm kiếm theo chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Theo ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }
    }
}
