using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchPaymentHistory : BaseHospitalSearch
    {
        /// <summary>
        /// Tìm theo user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tìm theo hồ sơ
        /// </summary>
        public int? RecordId { get; set; }

        /// <summary>
        /// Tìm theo phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Tìm theo thông tin ngân hàng
        /// </summary>
        public int? BankInfoId { get; set; }

        /// <summary>
        /// Tìm theo mã phiếu khám
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Tìm theo chi tiết dịch vụ phát sinh
        /// </summary>
        public int? ExaminationFormDetailId { get; set; }

        /// <summary>
        /// Dịch vụ phát sinh
        /// </summary>
        public int? AdditionServiceTypeId { get; set; }

        /// <summary>
        /// Tìm theo mã toa thuốc
        /// </summary>
        public int? MedicalBillId { get; set; }

        /// <summary>
        /// Tìm theo ngày thanh toán
        /// </summary>
        public DateTime? PaymentDate { get; set; }

    }
}
