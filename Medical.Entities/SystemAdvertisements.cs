using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Quảng cáo hệ thống
    /// </summary>
    public class SystemAdvertisements : MedicalAppDomainHospital
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
        [NotMapped]
        public IList<SystemFiles> SystemFiles { get; set; }

        #endregion

    }
}
