using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Phiếu khám bệnh
    /// </summary>
    [Table("ExaminationForm")]
    public class ExaminationForms : MedicalAppDomain
    {
        /// <summary>
        /// Mã phiếu khám bệnh
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Hồ sơ khám bệnh
        /// </summary>
        public int? RecordId { get; set; }
        /// <summary>
        /// Id bệnh nhân
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Loại khám (theo ngày/theo bác sĩ)
        /// </summary>
        public int? TypeId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }
        /// <summary>
        /// Có BHYT?
        /// </summary>
        public bool IsBHYT { get; set; }
        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }
        /// <summary>
        /// phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }
        /// <summary>
        /// (Tái khám/Dịch vụ/...)
        /// </summary>
        public int? ExaminationTypeId { get; set; }
        /// <summary>
        /// Ca khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }
        
        /// <summary>
        /// Chi phí khám
        /// </summary>
        public double? Price { get; set; }
    }
}
