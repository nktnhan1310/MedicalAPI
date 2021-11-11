using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Phòng khám bệnh
    /// </summary>
    public class RoomExaminationModel : MedicalCatalogueAppDomainHospitalModel
    {
        /// <summary>
        /// Số thứ tự phòng
        /// </summary>
        public string RoomIndex { get; set; }

        /// <summary>
        /// Mã khoa
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Mô tả khu vực khám
        /// </summary>
        public string ExaminationAreaDescription { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên chuyên khoa của phòng
        /// </summary>
        public string SpecialistTypeName { get; set; }

        #endregion

    }
}
