using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
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

        #region Extension Properties

        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Ca khám
        /// </summary>
        public string ConfigTimeExaminationValue { get; set; }

        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Chức danh + tên bác sĩ
        /// </summary>
        public string DoctorDisplayName { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public string DoctorCode { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Ngày khám hiển thị
        /// </summary>
        public string ExaminationDateDisplay
        {
            get
            {
                return ExaminationDate.HasValue ? ExaminationDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        /// <summary>
        /// Phòng khám
        /// </summary>
        public string RoomExaminationName { get; set; }

        #endregion

    }
}
