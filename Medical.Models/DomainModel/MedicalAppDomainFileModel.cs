using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models.DomainModel
{
    public class MedicalAppDomainFileModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Tên file
        /// </summary>
        [StringLength(500, ErrorMessage = "Tên file không được dài quá 500 kí tự")]
        public string FileName { get; set; }
        /// <summary>
        /// Nội dung file
        /// </summary>
        public byte[] FileContent { get; set; }
        /// <summary>
        /// Loại file
        /// </summary>
        [StringLength(100, ErrorMessage = "Tên file không được dài quá 100 kí tự")]
        public string ContentType { get; set; }
        /// <summary>
        /// Đuôi file
        /// </summary>
        [StringLength(50, ErrorMessage = "Tên file không được dài quá 50 kí tự")]
        public string FileExtension { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [StringLength(1000, ErrorMessage = "Tên file không được dài quá 1000 kí tự")]
        public string Description { get; set; }

    }
}
