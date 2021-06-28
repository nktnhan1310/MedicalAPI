using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationScheduleDetailV2 : BaseSearch
    {
        public int? HospitalId { get; set; }
        public int? DoctorId { get; set; }
        public int? SpecialistTypeId { get; set; }
        public int? Gender { get; set; }
        public int? DegreeTypeId { get; set; }
        public int? SessionId { get; set; }
        public int? DayOfWeek { get; set; }
        public DateTime? ExaminationDate { get; set; }
    }
}
