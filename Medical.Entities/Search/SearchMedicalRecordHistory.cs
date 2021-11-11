using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchMedicalRecordHistory : BaseSearch
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Mã hồ sơ người bệnh (nếu có)
        /// </summary>
        public int? MedicalRecordId { get; set; }

        public int? Type { get; set; }
    }
}
