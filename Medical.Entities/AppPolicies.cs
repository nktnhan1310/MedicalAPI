using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Chính sách
    /// </summary>
    public class AppPolicies : MedicalAppDomainHospital
    {
        /// <summary>
        /// Tiêu đề chính sách
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Nội dung chính
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Loại chính sách
        /// </summary>
        public int TypeId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách nội dung chính sách
        /// </summary>
        [NotMapped]
        public IList<AppPolicyDetails> AppPolicyDetails { get; set; }

        #endregion
    }
}
