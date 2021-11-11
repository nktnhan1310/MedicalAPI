using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// DANH MỤC MÔ TẢ THEO LOẠI DỊ ỨNG
    /// </summary>
    public class AllergyDescriptionTypes : MedicalCatalogueAppDomain
    {
        /// <summary>
        /// Mô tả chi tiết cho từng loại dị ứng
        /// </summary>
        public int? AllergyTypeId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên của loại dị ứng
        /// </summary>
        [NotMapped]
        public string AllergyTypeName { get; set; }

        #endregion
    }
}
