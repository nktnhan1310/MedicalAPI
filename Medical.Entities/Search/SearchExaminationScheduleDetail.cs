using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationScheduleForm
    {
        public int? HospitalId { get; set; }
        public int? SpecialistTypeId { get; set; }
        public DateTime? ExaminationDate { get; set; }
        public int? DoctorId { get; set; }
        public int? ExaminationScheduleDetailId { get; set; }
        /// <summary>
        /// Cờ check tái khám
        /// </summary>
        public bool? IsReExamination { get; set; }
    }
}
