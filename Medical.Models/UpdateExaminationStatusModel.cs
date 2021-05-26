using System;
using System.Collections.Generic;
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
    }
}
