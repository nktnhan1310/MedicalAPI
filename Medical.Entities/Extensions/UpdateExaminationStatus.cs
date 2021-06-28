using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities.Extensions
{
    /// <summary>
    /// Cập nhật trạng thái phiếu khám bệnh (lịch hẹn)
    /// </summary>
    public class UpdateExaminationStatus
    {
        /// <summary>
        /// Mã phiếu
        /// </summary>
        public int ExaminationFormId { get; set; }

        /// <summary>
        /// Mã thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Mã ngân hàng
        /// </summary>
        public int? BankInfoId { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }

        /// <summary>
        /// Nhập comment xác nhận/hủy
        /// </summary>
        [StringLength(1000)]
        public string Comment { get; set; }

        /// <summary>
        /// Người thao tác
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        public string PrescriptionCode { get; set; }

        /// <summary>
        /// Toa thuốc (list chữ)
        /// </summary>
        public string Prescription { get; set; }

        /// <summary>
        /// List file toa thuốc/xét nghiệm/ siêu âm...
        /// </summary>
        public IList<MedicalRecordDetailFiles> MedicalRecordDetailFiles { get; set; }

        [DefaultValue(false)]
        public bool HasMedicalBill { get; set; }

        /// <summary>
        /// Toa thuốc
        /// </summary>
        public MedicalBills MedicalBills { get; set; }
    }
}
