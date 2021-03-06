using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Chuyên khoa
    /// </summary>
    [Table("SpecialistTypes")]
    public class SpecialistTypes : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Giá theo chuyên khoa
        /// </summary>
        public double? Price { get; set; }

        #region Extension Properties

        /// <summary>
        /// Số lượng bác sĩ
        /// </summary>
        [NotMapped]
        public int TotalDoctors { get; set; }
        /// <summary>
        /// Số lượng phiếu khám bệnh trong ngày
        /// </summary>
        [NotMapped]
        public int TotalExaminationForms { get; set; }

        #endregion

    }
}
