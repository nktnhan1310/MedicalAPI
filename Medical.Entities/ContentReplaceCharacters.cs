using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bảng giá trị thay thế nội dung gửi đi
    /// </summary>
    public class ContentReplaceCharacters : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Giá trị muốn thay đổi
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Giá trị thay thế
        /// </summary>
        public string ReplaceValue { get; set; }
    }
}
