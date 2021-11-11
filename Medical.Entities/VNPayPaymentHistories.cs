using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class VNPayPaymentHistories : MedicalAppDomain
    {
        /// <summary>
        /// Mã order id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Mã giao dịch thanh toán
        /// </summary>
        public long PaymentTranId { get; set; }

        /// <summary>
        /// Thanh toán của phiếu nào
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Mã của user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// Trạng thái giao dịch
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        public string PayStatus { get; set; }

        /// <summary>
        /// Nội dung lỗi nếu có
        /// </summary>
        public string Message { get; set; }
    }
}
