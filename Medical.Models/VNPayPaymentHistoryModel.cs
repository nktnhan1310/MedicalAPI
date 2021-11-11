using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class VNPayPaymentHistoryModel : MedicalAppDomainModel
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
    }

    public class VNPaymentRequestModel
    {
        public string vnp_TmnCode { get; set; }

        public long vnp_Amount { get; set; }
        /// <summary>
        /// Mã ngân hàng thanh toán
        /// </summary>
        public string vnp_BankCode { get; set; }
        /// <summary>
        /// Mã giao dịch tại ngân hàng
        /// </summary>
        public string vnp_BankTranNo { get; set; }

        /// <summary>
        /// Loại tài khoản/thẻ
        /// </summary>
        public string vnp_CardType { get; set; }

        /// <summary>
        /// Thời gian thanh toán
        /// </summary>
        public string vnp_PayDate { get; set; }

        /// <summary>
        /// Thông tin mô tả nội dung thanh toán
        /// </summary>
        public string vnp_OrderInfo { get; set; }

        /// <summary>
        /// Mã giao dịch ghi nhận tại hệ thống vnpay
        /// </summary>
        public long vnp_TransactionNo { get; set; }

        /// <summary>
        /// Mã phản hồi kết quả thanh toán
        /// </summary>
        public string vnp_ResponseCode { get; set; }

        /// <summary>
        /// Mã phản hồi kết quả thanh toán. Tình trạng của giao dịch tại Cổng thanh toán VNPAY.
        /// </summary>
        public string vnp_TransactionStatus { get; set; }

        /// <summary>
        /// Giống mã gửi sang VNPAY khi gửi yêu cầu thanh toán. Ví dụ: 23554
        /// </summary>
        public long vnp_TxnRef { get; set; }

        /// <summary>
        /// Loại mã băm sử dụng
        /// </summary>
        public string vnp_SecureHashType { get; set; }

        /// <summary>
        /// Mã kiểm tra (checksum) để đảm bảo dữ liệu của giao dịch không bị thay đổi trong quá trình chuyển từ VNPAY về Website TMĐT. 
        /// </summary>
        public string vnp_SecureHash { get; set; }
    }

    public class VNPayRefundModel
    {
        public string vnp_RequestId { get; set; }
        public string vnp_Version { get; set; }
        public string vnp_Command { get; set; }
        public string vnp_TmnCode { get; set; }
        public string vnp_TransactionType { get; set; }
        public string vnp_TxnRef { get; set; }
        public long vnp_Amount { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_TransactionNo { get; set; }
        public string vnp_TransactionDate { get; set; }
        public string vnp_CreateBy { get; set; }
        public string vnp_CreateDate { get; set; }
        public string vnp_IpAddr { get; set; }
        public string vnp_SecureHash { get; set; }
    }

    public class VNPayRefundResponseModel
    {
        public string vnp_ResponseId { get; set; }

        public string vnp_Command { get; set; }
        public string vnp_TmnCode { get; set; }
        public string vnp_TxnRef { get; set; }
        public long vnp_Amount { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_ResponseCode { get; set; }
        public string vnp_Message { get; set; }
        public string vnp_BankCode { get; set; }
        public string vnp_PayDate { get; set; }
        public string vnp_TransactionNo { get; set; }
        public string vnp_TransactionType { get; set; }
        public string vnp_TransactionStatus { get; set; }
        public string vnp_SecureHash { get; set; }
    }
}
