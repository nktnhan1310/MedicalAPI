using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class UserFiles : MedicalAppDomainFile
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Loại file
        /// 0 => avatar;
        /// </summary>
        public int FileType { get; set; }

        /// <summary>
        /// Mã hồ sơ nếu có
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Mã tiểu sử bệnh án (nếu có)
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Mã tiền sử bệnh/ tiền sử phẫu thuật
        /// </summary>
        public int? MedicalRecordHistoryId { get; set; }

        /// <summary>
        /// Mã thư mục hình ảnh
        /// </summary>
        public int? FolderId { get; set; }
    }
}
