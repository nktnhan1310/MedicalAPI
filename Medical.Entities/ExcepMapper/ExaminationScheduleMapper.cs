using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class ExaminationScheduleMapper
    {
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        [Column(1)]
        public string SpecialistTypeCode { get; set; }
        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [Column(2)]
        public string DoctorCode { get; set; }
        /// <summary>
        /// Từ ngày
        /// </summary>
        [Column(3)]
        public string FromDate_s { get; set; }
        /// <summary>
        /// Đến ngày
        /// </summary>
        [Column(4)]
        public string ToDate_s { get; set; }

        /// <summary>
        /// Số lượt khám giới hạn buổi sáng
        /// </summary>
        [Column(5)]
        public int? MaximumExaminationMorning { get; set; }
        /// <summary>
        /// Số lượt khám giới buổi chiều
        /// </summary>
        [Column(6)]
        public int? MaximumExaminationAfternoon { get; set; }
        /// <summary>
        /// Số lượt khám giới hạn ngoài giờ
        /// </summary>
        [Column(7)]
        public int? MaximumExaminationOther { get; set; }

        /// <summary>
        /// Mã phòng
        /// </summary>
        [Column(8)]
        public string RoomExaminationCode { get; set; }

        /// <summary>
        /// Từ giờ
        /// </summary>
        [Column(9)]
        public string FromTime { get; set; }

        /// <summary>
        /// Đến giờ
        /// </summary>
        [Column(10)]
        public string ToTime { get; set; }

        /// <summary>
        /// Số lượt khám giới hạn theo từng ca
        /// </summary>
        [Column(11)]
        public int? MaximumExamination { get; set; }
        /// <summary>
        /// Kết quả trả về
        /// </summary>
        [Column(12)]
        public string ResultMessage { get; set; }
    }
}
