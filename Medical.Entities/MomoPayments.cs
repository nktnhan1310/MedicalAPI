using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Phiếu thanh toán momo
    /// </summary>
    public class MomoPayments : MedicalAppDomain
    {
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string OrderType { get; set; }
        public long TransId { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public string LocalMessage { get; set; }
        public string PayType { get; set; }
        public long ResponseTime { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Mã dịch vụ phát sinh
        /// </summary>
        public int? ExaminationFormDetailId { get; set; }

        /// <summary>
        /// Mã đơn thuốc
        /// </summary>
        public int? MedicalBillId { get; set; }

    }
}
