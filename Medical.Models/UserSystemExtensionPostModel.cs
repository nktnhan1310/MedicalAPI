using Medical.Models.DomainModel;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserSystemExtensionPostModel : MedicalAppDomainModel
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

        #region Extension Properties

        /// <summary>
        /// Tên đối tượng
        /// </summary>
        public string TargetTypeName
        {
            get
            {
                if (TargetTypeId.HasValue)
                {
                    switch (TargetTypeId.Value)
                    {
                        case (int)CatalogueUtilities.TargetType.Child:
                            return "Trẻ em";
                        case (int)CatalogueUtilities.TargetType.Adult:
                            return "Người lớn";
                        case (int)CatalogueUtilities.TargetType.Elder:
                            return "Người già";
                        case (int)CatalogueUtilities.TargetType.Pregnant:
                            return "Phụ nữ mang thai";
                        case (int)CatalogueUtilities.TargetType.Youth:
                            return "Thanh thiếu niên";
                        default:
                            break;
                    }

                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Tên file tạm của file background
        /// </summary>
        public string BackGroundImgTempName { get; set; }

        /// <summary>
        /// Tên file tạm của file logo
        /// </summary>
        public string LogoImgTempName { get; set; }

        #endregion

    }
}
