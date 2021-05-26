using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchHospitalConfigFee : BaseSearch
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }
        /// <summary>
        /// Mã phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int? ServiceTypeId { get; set; }
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }
    }
}
