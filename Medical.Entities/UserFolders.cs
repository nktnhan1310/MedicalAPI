using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    public class UserFolders : MedicalAppDomain
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
        [NotMapped]
        public int? TotalImageInFolder { get; set; }

        /// <summary>
        /// File của folder
        /// </summary>
        [NotMapped]
        public IList<UserFiles> UserFiles { get; set; }

        #endregion


    }
}
