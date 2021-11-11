using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class MedicalRecordFiles : MedicalAppDomainFile
    {
        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Mã thông tin thêm
        /// </summary>
        public int? MedicalRecordAdditionId { get; set; }

        /// <summary>
        /// Loại file
        /// 0 => AVATAR CỦA HỒ SƠ
        /// </summary>
        public int FileType { get; set; }

        /// <summary>
        /// Mã thư mục hình ảnh
        /// </summary>
        public int? FolderId { get; set; }
    }
}
