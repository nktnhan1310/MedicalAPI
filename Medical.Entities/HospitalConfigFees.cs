using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    public class HospitalConfigFees : MedicalAppDomainHospital
    {
        /// <summary>
        /// Theo phương thức thanh toán
        /// </summary>
        public int PaymentMethodId { get; set; }
        
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int ServiceTypeId { get; set; }
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }
        /// <summary>
        /// Phí khám bệnh cộng thêm
        /// </summary>
        public double Fee { get; set; }
        /// <summary>
        /// Theo phần trăm
        /// </summary>
        public bool IsRate { get; set; }
        /// <summary>
        /// Phần trăm Phí khám
        /// </summary>
        public double FeeRate { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalName { get; set; }
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        [NotMapped]
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Tên dịch vụ
        /// </summary>
        [NotMapped]
        public string ServiceTypeName { get; set; }
        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        #endregion

    }
}
