using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Chuyên khoa của bác sĩ
    /// </summary>
    [Table("DoctorDetails")]
    public class DoctorDetails : MedicalAppDomain
    {
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public int SpecialistTypeId { get; set; }
        /// <summary>
        /// Chi phí khám theo bác sĩ
        /// </summary>
        public double? Price { get; set; }
    }
}
