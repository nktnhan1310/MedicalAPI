using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    public class SMSConfiguration : MedicalAppDomain
    {
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public string BrandName { get; set; }
        public int SMSType { get; set; }
        /// <summary>
        /// Cú pháp tin nhắn mẫu
        /// </summary>
        public string TemplateText { get; set; }
        /// <summary>
        /// Url web service
        /// </summary>
        public string WebServiceUrl { get; set; }

    }
}
