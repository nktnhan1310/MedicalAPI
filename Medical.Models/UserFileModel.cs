using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserFileModel : MedicalAppDomainFileModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Loại file
        /// 0 => avatar;
        /// 1 => khác
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
        /// Mã folder
        /// </summary>
        public int? FolderId { get; set; }

        /// <summary>
        /// Tháng tạo
        /// </summary>
        public int Month
        {
            get
            {
                return Created.Month;
            }
        }

        /// <summary>
        /// Tháng tạo
        /// </summary>
        public int Year
        {
            get
            {
                return Created.Year;
            }
        }
    }
}
