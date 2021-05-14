using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Lịch sử thanh toán theo phiếu khám
    /// </summary>
    public class PaymentHistoryModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Thông tin phương thức thanh toán
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
    }
}
