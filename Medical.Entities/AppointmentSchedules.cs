using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch hẹn
    /// </summary>
    [Table("AppointmentSchedules")]
    public class AppointmentSchedules : MedicalAppDomain
    {
        /// <summary>
        /// Phiếu khám bệnh
        /// </summary>
        public int ExaminationFormId { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Ngày hẹn
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Chi phí khám
        /// </summary>
        public double? Price { get; set; }
    }
}
