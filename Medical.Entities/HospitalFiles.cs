using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class HospitalFiles : MedicalAppDomainFile
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int HospitalId { get; set; }

        /// <summary>
        /// Loại file
        /// 0 => Logo;
        /// 1 => Sơ đồ bệnh viện;
        /// 2 => Danh sách chuyên khoa;
        /// </summary>
        public int FileType { get; set; }
    }
}
