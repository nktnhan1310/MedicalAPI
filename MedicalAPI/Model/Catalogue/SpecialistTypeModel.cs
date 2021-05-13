using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    /// <summary>
    /// Chuyên khoa
    /// </summary>
    public class SpecialistTypeModel : MedicalCatalogueAppDomainModel
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
        public int TotalDoctors { get; set; }
        /// <summary>
        /// Số lượng phiếu khám bệnh trong ngày
        /// </summary>
        public int TotalExaminationForms { get; set; }

        #endregion
    }
}
