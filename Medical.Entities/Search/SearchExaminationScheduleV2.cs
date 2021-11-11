using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public int? Month { get; set; }
        public int? Year { get; set; }

        /// <summary>
        /// Lấy giá trị mặc định tìm kiếm là bác sĩ
        /// </summary>
        [DefaultValue((int)CatalogueUtilities.DoctorType.Doctor)]
        public int? DoctorTypeId { get; set; }

        /// <summary>
        /// Filter theo chi tiết ca trực
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }
    }
}
