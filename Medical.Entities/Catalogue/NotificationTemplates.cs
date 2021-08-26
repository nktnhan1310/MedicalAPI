﻿using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    public class NotificationTemplates : MedicalCatalogueAppDomainHospital
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
