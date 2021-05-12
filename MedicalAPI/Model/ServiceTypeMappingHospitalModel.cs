using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Mapping dữ liệu dịch vụ đăng ký bệnh viện
    /// </summary>
    public class ServiceTypeMappingHospitalModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Dịch vụ
        /// </summary>
        public int ServiceTypeId { get; set; }
        /// <summary>
        /// Giá khám theo từng dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Tổng số lượt khám/ngày
        /// </summary>
        public int TotalVisitNo { get; set; }
    }
}
