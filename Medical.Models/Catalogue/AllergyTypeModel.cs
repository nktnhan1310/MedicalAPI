using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class AllergyTypeModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Danh sách mô tả loại dị ứng của user
        /// </summary>
        public IList<AllergyDescriptionTypeModel> AllergyDescriptionTypes { get; set; }
    }
}
