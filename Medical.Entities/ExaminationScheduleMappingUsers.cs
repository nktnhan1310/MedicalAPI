using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class ExaminationScheduleMappingUsers : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã lịch trực
        /// </summary>
        public int ExaminationScheduleId { get; set; }

        /// <summary>
        /// Mã y tá/điều dưỡng
        /// </summary>
        public int DoctorId { get; set; }

        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public int? UserId { get; set; }
    }
}
