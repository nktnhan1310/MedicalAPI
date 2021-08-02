using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SMSEmailTemplates : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Mẫu là SMS
        /// </summary>
        public bool IsSMS { get; set; }
    }
}
