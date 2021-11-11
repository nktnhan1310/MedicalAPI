using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class SystemAdvertisementModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Mô tả khuyến mãi
        /// </summary>
        public string PromotionDescription { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách file của bài viết quảng cáo
        /// </summary>
        public IList<SystemFileModel> SystemFiles { get; set; }

        #endregion


    }
}
