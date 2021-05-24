using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Chi tiết lịch khám
    /// </summary>
    [Table("ExaminationScheduleDetails")]
    public class ExaminationScheduleDetails : MedicalAppDomain
    {
        /// <summary>
        /// Lịch khám
        /// </summary>
        public int ScheduleId { get; set; }
        /// <summary>
        /// Ca khám
        /// </summary>
        public int ConfigTimeExaminationId { get; set; }
        /// <summary>
        /// Phòng khám
        /// </summary>
        public int RoomExaminationId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Ca khám
        /// </summary>
        [NotMapped]
        public string ConfigTimeExaminationValue { get; set; }

        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Chức danh + tên bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorDisplayName { get; set; }

        /// <summary>
        /// Phòng khám
        /// </summary>
        [NotMapped]
        public string RoomExaminationName { get; set; }

        #endregion

    }
}
