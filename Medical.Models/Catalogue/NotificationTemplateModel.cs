using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Models
{
    public class NotificationTemplateModel : MedicalCatalogueAppDomainHospitalModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Template mặc định
        /// </summary>
        [DefaultValue(false)]
        public bool IsTemplateDefault { get; set; }
    }
}
