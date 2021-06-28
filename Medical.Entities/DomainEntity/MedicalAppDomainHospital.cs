using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities.DomainEntity
{
    public class MedicalAppDomainHospital : MedicalAppDomain
    {
        public int? HospitalId { get; set; }

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalName { get; set; }
    }
}
