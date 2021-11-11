using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    public class UserAllergyDetails : MedicalAppDomain
    {
        /// <summary>
        /// Mã dị ứng của user
        /// </summary>
        public int? UserAllergyId { get; set; }

        /// <summary>
        /// Mã mô tả dị ứng
        /// </summary>
        public int? DescriptionTypeId { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên mô tả dị ứng
        /// </summary>
        [NotMapped]
        public string DescriptionTypeName { get; set; }

        #endregion
    }
}
