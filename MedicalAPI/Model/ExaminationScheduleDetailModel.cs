using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Chi tiết lịch khám
    /// </summary>
    public class ExaminationScheduleDetailModel : MedicalAppDomainModel
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
    }
}
