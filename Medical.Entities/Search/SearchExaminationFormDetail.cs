using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchExaminationFormDetail : BaseHospitalSearch
    {
        /// <summary>
        /// Tìm theo mã phiếu
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Tìm theo mã hồ sơ
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Tìm theo mã người dùng
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tìm theo mã dịch vụ phát sinh
        /// </summary>
        public int? AdditionServiceId { get; set; }

        /// <summary>
        /// Tìm theo phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Tìm theo trạng thái
        /// </summary>
        public int? Status { get; set; }
    }
}
