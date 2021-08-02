using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class ContentReplaceCharacterModel : MedicalCatalogueAppDomainHospitalModel
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
