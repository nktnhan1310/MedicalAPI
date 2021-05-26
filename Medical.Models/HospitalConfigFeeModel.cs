using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class HospitalConfigFeeModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
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
        public string HospitalName { get; set; }
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Tên dịch vụ
        /// </summary>
        public string ServiceTypeName { get; set; }
        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }

        #endregion
    }
}
