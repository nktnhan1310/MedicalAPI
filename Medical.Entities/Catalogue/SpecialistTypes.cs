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
    public class SpecialistTypes : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Giá theo chuyên khoa
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Trưởng khoa
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// Danh sách bác sĩ/y tá/điều dưỡng thuộc chuyên khoa
        /// </summary>
        [NotMapped]
        public IList<Doctors> Doctors { get; set; }

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

        /// <summary>
        /// Tên trưởng khoa
        /// </summary>
        [NotMapped]
        public string ManagerFullName { get; set; }

        #endregion

    }
}
