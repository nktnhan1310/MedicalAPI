using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Phòng khám
    /// </summary>
    [Table("RoomExaminations")]
    public class RoomExaminations : MedicalCatalogueAppDomain
    {
        public int HospitalId { get; set; }
    }
}
