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
        /// Số thứ tự phòng
        /// </summary>
        public string RoomIndex { get; set; }

        /// <summary>
        /// Mã khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Mô tả khu vực khám
        /// </summary>
        public string ExaminationAreaDescription { get; set; }

        /// <summary>
        /// Số lượng bác sĩ ở phòng
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        #endregion

        #region Extension Properties

        /// <summary>
        /// Tên chuyên khoa của phòng
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        #endregion

    }
}
