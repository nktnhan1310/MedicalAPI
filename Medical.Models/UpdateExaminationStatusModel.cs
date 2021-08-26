using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Cập nhật trạng thái phiếu khám bệnh (lịch hẹn)
    /// </summary>
    public class UpdateExaminationStatusModel
    {
        /// <summary>
        /// Mã phiếu
        /// </summary>
        public int ExaminationFormId { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Nhập comment xác nhận/hủy
        /// </summary>
        [StringLength(1000, ErrorMessage = "Comment có độ dài tối đa 1000 kí tự!")]
        public string Comment { get; set; }

        /// <summary>
        /// Người thao tác
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Mã thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Mã ngân hàng
        /// </summary>
        public int? BankInfoId { get; set; }

        /// <summary>
        /// Tổng số tiền cần thanh toán
        /// </summary>
        public double? TotalPrice { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }

        /// <summary>
        /// Ca khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Chọn lại phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Chuẩn đoán của bác sĩ
        /// </summary>
        public string DoctorComment { get; set; }

        [DefaultValue(false)]
        public bool HasMedicalBill { get; set; }

        // ------------------------------------------ THÔNG TIN CHI TIẾT HỒ SƠ BỆNH ÁN
        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        public string PrescriptionCode { get; set; }

        /// <summary>
        /// Toa thuốc (list chữ)
        /// </summary>
        public string Prescription { get; set; }

        public IList<MedicalRecordDetailFileModel> MedicalRecordDetailFiles { get; set; }

        /// <summary>
        /// Toa thuốc khám bệnh nếu có
        /// </summary>
        public MedicalBillModel MedicalBills { get; set; }
    }
}
