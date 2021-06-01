using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Mapping dữ liệu kênh đăng ký bệnh viện
    /// </summary>
    public class ChannelMappingHospitalModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Kênh
        /// </summary>
        public int ChannelId { get; set; }
    }
}
