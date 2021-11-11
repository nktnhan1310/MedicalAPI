using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserAllergyDetailModel : MedicalAppDomainModel
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
        public string DescriptionTypeName { get; set; }

        #endregion
    }
}
