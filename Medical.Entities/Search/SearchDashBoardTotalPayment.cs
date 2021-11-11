using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchDashBoardTotalPayment
    {
        /// <summary>
        /// Theo bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// Theo phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Trạng thái giao dịch
        /// </summary>
        public int? Status { get; set; }
    }
}
