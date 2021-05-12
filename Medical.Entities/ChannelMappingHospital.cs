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
    public class ChannelMappingHospital : MedicalAppDomain
    {
        /// <summary>
        /// Bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Kênh
        /// </summary>
        public int ChannelId { get; set; }

    }
}
