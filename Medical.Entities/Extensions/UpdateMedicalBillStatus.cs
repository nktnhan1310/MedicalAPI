using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    public class UpdateMedicalBillStatus
    {
        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        [Required]
        public int MedicalBillId { get; set; }
        /// <summary>
        /// Trạng thái cập nhật
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Tổng số tiền thanh toán (nhập tay nếu có)
        /// </summary>
        public double? TotalPrice { get; set; }

        /// <summary>
        /// Phí (nếu có)
        /// </summary>
        public double? Fee { get; set; }
        
        /// <summary>
        /// Mã phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Mã ngân hàng thanh toán
        /// </summary>
        public int? BankInfoId { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
