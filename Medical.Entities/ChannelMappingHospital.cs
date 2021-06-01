using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Kênh đăng ký khám bệnh của bệnh viện
    /// </summary>
    [Table("ChannelMappingHospital")]
    public class ChannelMappingHospital : MedicalAppDomainHospital
    {
        /// <summary>
        /// Kênh
        /// </summary>
        public int ChannelId { get; set; }

    }
}
