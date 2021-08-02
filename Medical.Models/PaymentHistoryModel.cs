using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Lịch sử thanh toán theo phiếu khám
    /// </summary>
    public class PaymentHistoryModel : MedicalAppDomainHospitalModel
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

        #region Extension Properties

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public string MedicalRecordCode { get; set; }

        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public string ExaminationFormCode { get; set; }

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }
        /// <summary>
        /// Thông tin ngân hàng thanh toán
        /// </summary>
        public string BankInfo { get; set; }

        /// <summary>
        /// Tên dịch vụ phát sinh
        /// </summary>
        public string AdditionServiceName { get; set; }

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        public string MedicalBillCode { get; set; }

        #endregion
    }
}
