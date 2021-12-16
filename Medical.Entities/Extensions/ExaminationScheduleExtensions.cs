using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    public class ExaminationScheduleExtensions
    {
        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
<<<<<<< HEAD
        /// Danh sách ngày của lịch trực
        /// </summary>
        public List<DateTime> ExaminationDates { get; set; }

        /// <summary>
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
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
        /// Người tạo
        /// </summary>
        public string CreatedBy { get; set; }

        #region Extension Properties

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public string DoctorCode { get; set; }

        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public string SpecialistTypeCode { get; set; }

        /// <summary>
        /// Danh sách ca làm việc
        /// </summary>
        public IList<ExaminationScheduleDetails> ExaminationScheduleDetails { get; set; }

        /// <summary>
        /// Danh sách ca làm việc được import
        /// </summary>
        public IList<ExamintionScheduleDetailExtensions> ExamintionScheduleDetailExtensions { get; set; }

        /// <summary>
        /// STT import
        /// </summary>
        public int? ImportIndex { get; set; }

        #endregion


    }

    public class ExamintionScheduleDetailExtensions
    {
        /// <summary>
        /// Mã phòng
        /// </summary>
        public string RoomExaminationCode { get; set; }
        /// <summary>
        /// Từ giờ
        /// </summary>
        public string FromTimeText { get; set; }
        /// <summary>
        /// Đến giờ
        /// </summary>
        public string ToTimeText { get; set; }

        /// <summary>
        /// Giới hạn số lượt khám theo từng ca
        /// </summary>
        public int? MaximumExamination { get; set; }
    }

}
