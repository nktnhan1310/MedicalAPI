using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Tin tức của mỗi bệnh viện
    /// </summary>
    public class NewFeeds : MedicalAppDomainHospital
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        [StringLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Đường dẫn logo
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Đường dẫn logo
        /// </summary>
        public string BannerUrl { get; set; }

        /// <summary>
        /// Đường dẫn file background
        /// </summary>
        public string BackGroundImgUrl { get; set; }
    }
}
