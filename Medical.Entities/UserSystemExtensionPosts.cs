using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bài viết của hệ thống cho người dùng
    /// </summary>
    public class UserSystemExtensionPosts : MedicalAppDomain
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Nội dung bài viết
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Đường dẫn hình ảnh background
        /// </summary>
        public string BackGroundImgUrl { get; set; }

        /// <summary>
        /// Đường dẫn hình ảnh logo
        /// </summary>
        public string LogoImgUrl { get; set; }

        /// <summary>
        /// Đối tượng bài viết
        /// </summary>
        public int? TargetTypeId { get; set; }

        /// <summary>
        /// Loại bài viết
        /// 0 => Lịch theo dõi thai kì
        /// 1 => Chế độ dinh dưỡng
        /// 2 => khác
        /// </summary>
        public int? PostTypeId { get; set; }
    }
}
