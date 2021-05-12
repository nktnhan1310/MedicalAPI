using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities.DomainEntity
{
    public class MedicalAppDomainFile : MedicalAppDomain
    {
        /// <summary>
        /// Tên file
        /// </summary>
        [StringLength(500)]
        public string FileName { get; set; }
        /// <summary>
        /// Nội dung file
        /// </summary>
        public byte[] FileContent { get; set; }
        /// <summary>
        /// Loại file
        /// </summary>
        [StringLength(100)]
        public string ContentType { get; set; }
        /// <summary>
        /// Đuôi file
        /// </summary>
        [StringLength(50)]
        public string FileExtension { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
