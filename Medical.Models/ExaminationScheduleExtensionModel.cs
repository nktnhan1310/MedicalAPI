using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Models
{
    public class ExaminationScheduleExtensionModel
    {
        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int? DoctorId { get; set; }

        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn bệnh viện")]
        public int? HospitalId { get; set; }

        /// <summary>
        /// Danh sách ngày của lịch trực
        /// </summary>
        public List<DateTime> ExaminationDates { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn từ ngày")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn đến ngày")]
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Số ca khám tối đa trong buổi sáng
        /// </summary>
        public int? MaximumMorningExamination { get; set; }
        /// <summary>
        /// Số ca khám tối đa trong buổi chiều
        /// </summary>
        public int? MaximumAfternoonExamination { get; set; }
        /// <summary>
        /// Số ca khám tối đa trong buổi khác
        /// </summary>
        public int? MaximumOtherExamination { get; set; }

        /// <summary>
        /// Cờ check sử dụng cấu hình số phút khám của bệnh viện
        /// </summary>
        [DefaultValue(false)]
        public bool IsUseHospitalConfig { get; set; }

        /// <summary>
        /// Danh sách ca làm việc
        /// </summary>
        public IList<ExaminationScheduleDetailModel> ExaminationScheduleDetails { get; set; }
    }
}
