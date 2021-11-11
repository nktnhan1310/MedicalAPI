using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserFolderModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Tên folder
        /// </summary>
        public string FolderName { get; set; }
        /// <summary>
        /// Iconfolder
        /// </summary>
        public string FolderIcon { get; set; }

        /// <summary>
        /// Loại folder
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }


        #region Extension Properties

        /// <summary>
        /// Số lượng hình ảnh trong thư mục
        /// </summary>
        public int? TotalImageInFolder { get; set; }

        /// <summary>
        /// File của folder
        /// </summary>
        public IList<UserFileModel> UserFiles { get; set; }

        #endregion
    }
}
