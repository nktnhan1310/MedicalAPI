using Medical.Entities.DomainEntity;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DefaultValue(false)]
        public bool IsUseHospitalConfig { get; set; }

        /// <summary>
        /// Guid import lịch
        /// </summary>
        public Guid? ImportScheduleId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên buổi khám
        /// </summary>
        [NotMapped]
        public string SessionTypeName { get; set; }

        /// <summary>
        /// Giá trị từ giờ
        /// </summary>
        [NotMapped]
        public string FromTimeDisplay
        {
            get
            {
                string result = string.Empty;
                if (FromTime.HasValue) return DateTimeUtilities.ConvertTotalMinuteToString(FromTime.Value);
                return result;
            }
        }

        /// <summary>
        /// Giá trị đến giờ
        /// </summary>
        [NotMapped]
        public string ToTimeDisplay
        {
            get
            {
                string result = string.Empty;
                if (ToTime.HasValue) return DateTimeUtilities.ConvertTotalMinuteToString(ToTime.Value);
                return result;
            }
        }

        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        [NotMapped]
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [NotMapped]
        public int? DoctorId { get; set; }

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
        /// Tên hiển thị của bác sĩ thay thế
        /// </summary>
        [NotMapped]
        public string ReplaceDoctorDisplayName { get; set; }

        /// <summary>
        /// Tên hiển thị bác sĩ thay thế của lịch
        /// </summary>
        [NotMapped]
        public string ReplaceDoctorScheduleDisplayName { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorCode { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        [NotMapped]
        public DateTime? ExaminationDate { get; set; }
        /// <summary>
        /// Phòng khám
        /// </summary>
        [NotMapped]
        public string RoomExaminationName { get; set; }

        #endregion

    }
}
