using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationScheduleForm
    {
        public int HospitalId { get; set; }
        public int SpecialistTypeId { get; set; }
        public DateTime? ExaminationDate { get; set; }
        public int? DoctorId { get; set; }
    }
}
