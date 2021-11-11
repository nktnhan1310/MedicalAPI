using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationScheduleDetail : BaseSearch
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }
        /// <summary>
        /// Mã phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }
        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }
        /// <summary>
        /// Mã ca khám
        /// </summary>
        public int? SessionTypeId { get; set; }

        [DefaultValue("DoctorId desc")]
        public new string OrderBy { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }
    }

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
