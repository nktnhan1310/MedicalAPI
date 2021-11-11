using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Loại dị ứng
    /// </summary>
    public class AllergyTypes : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Danh sách mô tả chi tiết của loại dị ứng
        /// </summary>
        [NotMapped]
        public IList<AllergyDescriptionTypes> AllergyDescriptionTypes { get; set; }
    } 
}
