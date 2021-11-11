using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationFormHistory : BaseSearch
    {
        /// <summary>
        /// Search theo mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Theo mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// Theo mã hồ sơ
        /// </summary>
        public int? RecordId { get; set; }

        /// <summary>
        /// Theo mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Theo mã phiếu
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Theo mã dịch vụ
        /// </summary>
        public int? ServiceTypeId { get; set; }

        /// <summary>
        /// Theo trạng thái phiếu
        /// </summary>
        public List<int> StatusIds { get; set; }

    }
}
