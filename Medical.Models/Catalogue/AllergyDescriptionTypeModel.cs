using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class AllergyDescriptionTypeModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Mã loại dị ứng
        /// </summary>
        public int? AllergyTypeId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên loại dị ứng
        /// </summary>
        public string AllergyTypeName { get; set; }

        #endregion
    }
}
