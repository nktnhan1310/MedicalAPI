using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class SystemFileModel : MedicalAppDomainFileModel
    {
        /// <summary>
        /// Loại file
        /// 0 => Image Slidebar
        /// 1 => Banner
        /// 2 => Other
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// STT hiển thị nếu có
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// Chiều rộng hình ảnh
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Chiều cao hình ảnh
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Mã của bài viết quảng cáo nếu có
        /// </summary>
        public int? SystemAdvertisementId { get; set; }

        /// <summary>
        /// Mã bệnh viện (nếu có)
        /// </summary>
        public int? HospitalId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }

        #endregion
    }
}
