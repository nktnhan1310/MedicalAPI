using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models.DomainModel
{
    public class MedicalAppDomainHospitalModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }
    }
}
