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
        public string UserName { get; set; }
        public string Password { get; set; }
        [Required]
        public string BrandName { get; set; }
        [Required]
        public int MessageTypeId { get; set; }
        public int ItemSendCount { get; set; }
        public int TimeSend { get; set; }
        /// <summary>
        /// Mã access token
        /// </summary>
        [NotMapped]
        public string AuthorizationCode { get; set; }
        [NotMapped]
        public bool IsAuthorization { get; set; }
        /// <summary>
        /// Template tin nhắn mobifone
        /// </summary>
        [NotMapped]
        public string TemplateTextMobi { get; set; }
        /// <summary>
        /// Template tin nhắn vina
        /// </summary>
        [NotMapped]
        public string TemplateTextVina { get; set; }
        /// <summary>
        /// Template tin nhắn viettel
        /// </summary>
        [NotMapped]
        public string TemplateTextViettel { get; set; }
        /// <summary>
        /// Template tin nhắn khác
        /// </summary>
        [NotMapped]
        public string TemplateTextOther { get; set; }
        /// <summary>
        /// Mã unicode
        /// </summary>
        [NotMapped]
        public bool IsUniCode { get; set; }
        /// <summary>
        /// Có được mã hóa không
        /// </summary>
        [NotMapped]
        public bool IsEncrypted { get; set; }
        /// <summary>
        /// Url web service
        /// </summary>
        [NotMapped]
        public string WebServiceUrl { get; set; }

    }
}
