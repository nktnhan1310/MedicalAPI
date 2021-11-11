using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserFileInFolderModel
    {
        /// <summary>
        /// Mã file của user
        /// </summary>
        public int? UserFileId { get; set; }
        /// <summary>
        /// Mã file của hồ sơ
        /// </summary>
        public int? MedicalRecordFileId { get; set; }
        /// <summary>
        /// Mã file của tiểu sử
        /// </summary>
        public int? MedicalRecordDetaiFilelId { get; set; }

        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Đường dẫn file
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Loại file
        /// </summary>
        public int? TypeId { get; set; }
        
        /// <summary>
        /// Mã folder
        /// </summary>
        public int? FolderId { get; set; }
    }
}
