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
    }
}
