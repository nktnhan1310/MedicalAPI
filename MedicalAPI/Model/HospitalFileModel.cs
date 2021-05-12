using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
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
