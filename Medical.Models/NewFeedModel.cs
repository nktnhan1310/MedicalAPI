using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Models
{
    public class NewFeedModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        [StringLength(500, ErrorMessage = "Tiêu đề phải nhỏ hơn 500 kí tự")]
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

        #region Extension Properties

        /// <summary>
        /// Tên file logo
        /// </summary>
        public string LogoFileName { get; set; }

        /// <summary>
        /// Tên file banner
        /// </summary>
        public string BannerFileName { get; set; }

        /// <summary>
        /// Tên file background
        /// </summary>
        public string BackGroundImgFileName { get; set; }

        #endregion
    }
}
