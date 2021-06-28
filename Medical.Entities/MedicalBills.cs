using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Đơn thuốc
    /// </summary>
    public class MedicalBills : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Số thứ tự chờ lấy thuốc khám bệnh
        /// </summary>
        public string MedicalBillIndex { get; set; }
        /// <summary>
        /// Tổng tiền thuốc
        /// </summary>
        public double? TotalPrice { get; set; }
        /// <summary>
        /// Mã hồ sơ chi tiết bệnh án
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Trạng thái toa thuốc
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Mã phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Mã ngân hàng thanh toán (nếu có)
        /// </summary>
        public int? BankInfoId { get; set; }

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách thuốc trong toa
        /// </summary>
        [NotMapped]
        public IList<Medicines> Medicines { get; set; }

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        [NotMapped]
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Tên ngân hàng thanh toán
        /// </summary>
        [NotMapped]
        public string BankInfo { get; set; }

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
        /// Tên bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorFullName { get; set; }

        #endregion

    }
}
