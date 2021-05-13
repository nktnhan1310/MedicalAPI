using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Cấu hình ca khám
    /// </summary>
    public class ConfigTimeExamniationModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Buổi (sáng/chiều/tối)
        /// </summary>
        public int SessionId { get; set; }
        /// <summary>
        /// Bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
    }
}
