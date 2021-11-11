using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities.Extensions
{
    public class FeeCaculateExaminationRequest
    {
        /// <summary>
        /// Theo bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Theo phương thức thanh toán
        /// </summary>
        public int PaymentMethodId { get; set; }
        /// <summary>
        /// Theo dịch vụ
        /// </summary>
        public int ServiceTypeId { get; set; }
        /// <summary>
        /// Theo chuyên khoa (nếu có)
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Loại vaccine
        /// </summary>
        public int? VaccineTypeId { get; set; }

        /// <summary>
        /// Lấy theo lịch khám
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Danh sách dịch vụ phát sinh
        /// </summary>
        public List<int> AdditionServiceIds { get; set; }

        /// <summary>
        /// Danh sách chi tiết dịch vụ phát sinh
        /// </summary>
        public List<int> AdditionServiceDetailIds { get; set; }
    }
}
