using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationScheduleDetailAddition
    {
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn bệnh viện")]
        public int HospitalId { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày khám")]
        public DateTime ExaminationDate { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
        public int SpecialistTypeId { get; set; }

        /// <summary>
        /// Từ giờ
        /// </summary>
        public int FromTime { get; set; }

        /// <summary>
        /// Đến giờ
        /// </summary>
        public int ToTime { get; set; }
    }
}
