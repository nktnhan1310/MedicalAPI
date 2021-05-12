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

        /// <summary>
        /// Bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
    }
}
