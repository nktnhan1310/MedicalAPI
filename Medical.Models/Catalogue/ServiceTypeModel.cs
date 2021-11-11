using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Dịch vụ đăng ký khám bệnh
    /// </summary>
    public class ServiceTypeModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Cờ check có bảo hiểm y tế hay ko
        /// </summary>
        public bool IsBHYT { get; set; }
    }
}
