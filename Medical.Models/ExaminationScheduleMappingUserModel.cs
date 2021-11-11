using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class ExaminationScheduleMappingUserModel : MedicalAppDomainHospitalModel
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
        public int UserId { get; set; }
    }
}
