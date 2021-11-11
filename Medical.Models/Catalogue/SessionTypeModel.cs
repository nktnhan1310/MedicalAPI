using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Buổi khám (Sáng/Chiều/Tối)
    /// </summary>
    public class SessionTypeModel : MedicalCatalogueAppDomainHospitalModel
    {

        /// <summary>
        /// Từ giờ
        /// </summary>
        public int? FromTime { get; set; }
        /// <summary>
        /// Đến giờ
        /// </summary>
        public int? ToTime { get; set; }

        #region Extension Properties

        /// <summary>
        /// Từ giờ hiển thị
        /// </summary>
        public string FromTimeDisplayValue { get; set; }

        /// <summary>
        /// Đến giờ hiển thị
        /// </summary>
        public string ToTimeDisplayValue { get; set; }

        #endregion

    }
}
