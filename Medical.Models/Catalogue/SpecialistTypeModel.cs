using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Chuyên khoa
    /// </summary>
    public class SpecialistTypeModel : MedicalCatalogueAppDomainHospitalModel
    {
        /// <summary>
        /// Giá theo chuyên khoa
        /// </summary>
        public double? Price { get; set; }

        #region Extension Properties

        /// <summary>
        /// Số lượng bác sĩ
        /// </summary>
        public int TotalDoctors { get; set; }
        /// <summary>
        /// Số lượng phiếu khám bệnh trong ngày
        /// </summary>
        public int TotalExaminationForms { get; set; }

        #endregion
    }
}
