using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    /// <summary>
    /// Dịch vụ phát sinh 
    /// </summary>
    public class AdditionServiceModel : MedicalCatalogueAppDomainHospitalModel
    {
        /// <summary>
        /// Giá dịch vụ phát sinh
        /// </summary>
        public double? Price { get; set; }
    }
}
