using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities.DomainEntity
{
    public class MedicalCatalogueAppDomainHospital : MedicalAppDomainHospital
    {
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(500)]
        [Required]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
