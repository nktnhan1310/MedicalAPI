using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Phiếu khám bệnh
    /// </summary>
    public class ExaminationFormModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã phiếu khám bệnh
        /// </summary>
        [StringLength(50, ErrorMessage = "Mã phiếu khám bệnh phải nhỏ hơn 50 kí tự!")]
        public string Code { get; set; }
        /// <summary>
        /// Hồ sơ khám bệnh
        /// </summary>
        [Required]
        public int RecordId { get; set; }
        /// <summary>
        /// Loại khám (theo ngày/theo bác sĩ)
        /// </summary>
        public int? TypeId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public int SpecialistTypeId { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Có BHYT?
        /// </summary>
        public bool IsBHYT { get; set; }
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
        /// Trạng thái phiếu (Chờ xác nhận lịch hẹn/Đã xác nhận lịch hẹn/Chờ xác nhận hủy/Đã hủy/Chờ xác nhận tái khám/Đã xác nhận tái khám)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Chi phí khám
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Số thứ tự khám bệnh
        /// </summary>
        public int? ExaminationIndex { get; set; }

        #region Extension Properties

        /// <summary>
        /// Comment khi duyệt phiếu khám
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Lịch sử tạo phiếu khám bệnh (lịch hẹn)
        /// </summary>
        public IList<ExaminationHistoryModel> ExaminationHistories { get; set; }

        /// <summary>
        /// Lịch sử thanh toán
        /// </summary>
        public IList<PaymentHistoryModel> PaymentHistories { get; set; }

        #endregion
    }
}
