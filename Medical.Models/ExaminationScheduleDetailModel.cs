using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// Từ giờ 
        /// </summary>
        public int? FromTime { get; set; }

        /// <summary>
        /// Text hiển thị từ giờ
        /// </summary>
        public string FromTimeText { get; set; }

        /// <summary>
        /// Đến giờ
        /// </summary>
        public int? ToTime { get; set; }

        /// <summary>
        /// Text hiển thị đến giờ
        /// </summary>
        public string ToTimeText { get; set; }

        /// <summary>
        /// Ca khám
        /// </summary>
        //public int ConfigTimeExaminationId { get; set; }
        /// <summary>
        /// Phòng khám
        /// </summary>
        public int RoomExaminationId { get; set; }

        /// <summary>
        /// Số ca khám tối đa trong ngày theo khung giờ
        /// </summary>
        public int? MaximumExamination { get; set; }

        /// <summary>
        /// Mã bác sĩ thay thế
        /// </summary>
        public int? ReplaceDoctorId { get; set; }

        /// <summary>
        /// Id của buổi
        /// </summary>
        public int? SessionTypeId { get; set; }

        /// <summary>
        /// Cờ check sử dụng cấu hình số phút khám của bệnh viện
        /// </summary>
        public bool IsUseHospitalConfig { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên buổi khám
        /// </summary>
        public string SessionTypeName { get; set; }
        /// <summary>
        /// Giá trị từ giờ
        /// </summary>
        public string FromTimeDisplay { get; set; }

        /// <summary>
        /// Giá trị đến giờ
        /// </summary>
        public string ToTimeDisplay { get; set; }

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
        /// Tên hiển thị của bác sĩ thay thế
        /// </summary>
        public string ReplaceDoctorDisplayName { get; set; }

        /// <summary>
        /// Tên bác sĩ thay thế của lịch nếu có
        /// </summary>
        public string ReplaceDoctorScheduleDisplayName { get; set; }


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
