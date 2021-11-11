using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Các hình thức khám thai trong quá trình thai kỳ
    /// </summary>
    public class UserPregnancyDetails : MedicalAppDomain
    {
        /// <summary>
        /// Mã thai kỳ
        /// </summary>
        public int? UserPregnancyId { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Kết quả
        /// </summary>
        public string ResultDescription { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
