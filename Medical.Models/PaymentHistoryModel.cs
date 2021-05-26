﻿using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Lịch sử thanh toán theo phiếu khám
    /// </summary>
    public class PaymentHistoryModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Theo phương thức thanh toán
        /// </summary>
        public int PaymentMethodId { get; set; }
        /// <summary>
        /// Thông tin ngân hàng thanh toán
        /// </summary>
        public int BankInfoId { get; set; }
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
