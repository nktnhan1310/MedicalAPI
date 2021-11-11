using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserAllergyModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã loại dị ứng
        /// </summary>
        public int? AllergyTypeId { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên loại dị ứng
        /// </summary>
        public string AllergyTypeName { get; set; }

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        public string UserFullName { get; set; }

        #endregion
    }
}
