using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    public class ExaminationFormDetails : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Mã dịch vụ phát sinh
        /// </summary>
        public int AdditionServiceId { get; set; }

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// STT chờ khám của dịch vụ phát sinh
        /// </summary>
        public string AdditionExaminationIndex { get; set; }

        /// <summary>
        /// Giá dịch vụ phát sinh
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Trạng thái thanh toán dịch vụ
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên dịch vụ phát sinh
        /// </summary>
        [NotMapped]
        public string AdditionServiceName { get; set; }

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        [NotMapped]
        public string PaymentMethodName { get; set; }

        #endregion

    }
}
