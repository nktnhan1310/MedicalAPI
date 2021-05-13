using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch khám theo từng bác sĩ
    /// </summary>
    [Table("ExaminationSchedules")]
    public class ExaminationSchedules : MedicalAppDomain
    {
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalName { get; set; }
        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorName { get; set; }

        /// <summary>
        /// Danh sách ca làm việc
        /// </summary>
        [NotMapped]
        public IList<ExaminationScheduleDetails> ExaminationScheduleDetails { get; set; }

        #endregion
    }
}
