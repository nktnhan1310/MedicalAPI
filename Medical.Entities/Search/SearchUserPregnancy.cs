using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchUserPregnancy : BaseSearch
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        public int? MedicalRecordId { get; set; }
        /// <summary>
        /// Phân loại thay kì
        /// Ngày
        /// Tháng
        /// Tuần
        /// </summary>
        public int? TypeId { get; set; }
    }
}
