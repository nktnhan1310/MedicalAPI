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
    public class PaymentHistories : MedicalAppDomainHospital
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
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Chi phí khám bệnh
        /// </summary>
        public double? ExaminationFee { get; set; }

        /// <summary>
        /// Phí dịch vụ
        /// </summary>
        public double? ServiceFee { get; set; }

        /// <summary>
        /// Mã chi tiết dịch vụ phát sinh
        /// </summary>
        public int? ExaminationFormDetailId { get; set; }

        /// <summary>
        /// Mã dịch vụ phát sinh
        /// </summary>
        public int? AdditionServiceId { get; set; }

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        public int? MedicalBillId { get; set; }

        /// <summary>
        /// Trạng thái
        /// 0 => Thanh toán đơn
        /// 1 => Hoàn tiền
        /// </summary>
        public int? Status { get; set; }

        #region Extension Properties

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        [NotMapped]
        public string MedicalRecordCode { get; set; }

        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        [NotMapped]
        public string ExaminationFormCode { get; set; }

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        [NotMapped]
        public string PaymentMethodName { get; set; }
        /// <summary>
        /// Thông tin ngân hàng thanh toán
        /// </summary>
        [NotMapped]
        public string BankInfo { get; set; }

        /// <summary>
        /// Tên dịch vụ phát sinh
        /// </summary>
        [NotMapped]
        public string AdditionServiceName { get; set; }

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        [NotMapped]
        public string MedicalBillCode { get; set; }

        #endregion

    }
}
