using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class AppPolicyModel : MedicalAppDomainHospitalModel
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
        public IList<AppPolicyDetailModel> AppPolicyDetails { get; set; }

        #endregion
    }
}
