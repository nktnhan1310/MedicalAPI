using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchMedicalRecordDetail: BaseHospitalSearch
    {
        /// <summary>
        /// Tìm kiếm theo mã chi tiết hồ sơ
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Theo user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Mã lịch hẹn
        /// </summary>
        public int? ExaminationFormId { get; set; }
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int? ServiceTypeId { get; set; }
    }
}
