using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
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
