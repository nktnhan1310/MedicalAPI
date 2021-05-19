using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Loại khám
    /// </summary>
    public class ExaminationTypeModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Theo bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
    }
}
