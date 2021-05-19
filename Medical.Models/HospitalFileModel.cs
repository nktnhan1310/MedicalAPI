using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// File của thông tin bệnh viện
    /// </summary>
    public class HospitalFileModel : MedicalAppDomainFileModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int HospitalId { get; set; }

        /// <summary>
        /// Loại file
        /// 0 => Logo
        /// 1 => Sơ đồ bệnh viện
        /// 2 => Danh sách chuyên khoa
        /// </summary>
        public int FileType { get; set; }
    }
}
