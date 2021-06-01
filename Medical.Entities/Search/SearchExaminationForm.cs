using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Model Search cho phiếu khám bệnh (lịch hẹn)
    /// </summary>
    public class SearchExaminationForm : BaseHospitalSearch
    {
        /// <summary>
        /// Tìm theo user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Tìm theo hồ sơ
        /// </summary>
        public int? RecordId { get; set; }
        /// <summary>
        /// Loại khám (theo bác sĩ, theo ngày)
        /// </summary>
        public int? TypeId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }
        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }
        /// <summary>
        /// Dịch vụ khám
        /// </summary>
        public int? ExaminationTypeId { get; set; }
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }
        
    }
}
