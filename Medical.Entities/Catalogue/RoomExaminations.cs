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
    public class RoomExaminations : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Số lượng bác sĩ khám ở phòng
        /// </summary>
        public int TotalDoctor { get; set; }

    }
}
