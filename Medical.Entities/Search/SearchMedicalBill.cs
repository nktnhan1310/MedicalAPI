using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchMedicalBill : BaseHospitalSearch
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int? ExaminationFormId { get; set; }
        /// <summary>
        /// Trạng thái khám
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Mã ngân hàng thanh toán
        /// </summary>
        public int? BankInfoId { get; set; }
        /// <summary>
        /// Mã chi tiết hồ sơ thanh toán
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        public int? MedicalBillId { get; set; }

        /// <summary>
        /// Tìm toa thuốc theo ngày
        /// </summary>
        public DateTime? CreatedDate { get; set; }
    }
}
