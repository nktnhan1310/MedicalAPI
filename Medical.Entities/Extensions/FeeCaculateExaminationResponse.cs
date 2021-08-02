using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities.Extensions
{
    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public class FeeCaculateExaminationResponse
    {
        /// <summary>
        /// Giá khám
        /// </summary>
        public double? ExaminationPrice { get; set; }
        /// <summary>
        /// Phí phát sinh (nếu có)
        /// </summary>
        public double? ExaminationFee { get; set; }

        /// <summary>
        /// Giá khám hiển thị
        /// </summary>
        public string ExaminationPriceDisplay
        {
            get
            {
                return ExaminationPrice.HasValue ? string.Format("{0} đ", ExaminationPrice.Value.ToString("#,###"))  : string.Format("{0} đ", 0);
            }
        }

        /// <summary>
        /// Phí khám hiển thị
        /// </summary>
        public string ExaminationFeeDisplay
        {
            get
            {
                return ExaminationFee.HasValue ? string.Format("{0} đ", ExaminationFee.Value.ToString("#,###")) : string.Format("{0} đ", 0);
            }
        }

        /// <summary>
        /// Tổng chi phí chi trả
        /// </summary>
        public double? TotalPayment
        {
            get
            {
                return (ExaminationPrice ?? 0) + (ExaminationFee ?? 0);
            }
        }

        /// <summary>
        /// Tổng chi phí chi trả hiển thị
        /// </summary>
        public string TotalPaymentDisplay
        {
            get
            {
                return TotalPayment.HasValue ? string.Format("{0} đ", TotalPayment.Value.ToString("#,###")) : string.Format("{0} đ", 0);
            }
        }

    }
}
