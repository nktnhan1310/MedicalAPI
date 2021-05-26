using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch sử thanh toán theo phiếu khám
    /// </summary>
    public class PaymentHistories : MedicalAppDomain
    {
        /// <summary>
        /// Theo phương thức thanh toán
        /// </summary>
        public int PaymentMethodId { get; set; }
        /// <summary>
        /// Thông tin ngân hàng thanh toán
        /// </summary>
        public int? BankInfoId { get; set; }
        /// <summary>
        /// Theo mã lịch khám
        /// </summary>
        public int ExaminationFormId { get; set; }

        /// <summary>
        /// Chi phí khám bệnh
        /// </summary>
        public double? ExaminationFee { get; set; }

        /// <summary>
        /// Phí dịch vụ
        /// </summary>
        public double? ServiceFee { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }
        /// <summary>
        /// Thông tin ngân hàng thanh toán
        /// </summary>
        public string BankInfo { get; set; }

        #endregion

    }
}
