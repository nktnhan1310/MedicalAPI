using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class MedicalRecordDetailFiles : MedicalAppDomainFile
    {
        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Loại file
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Mã thư mục hình ảnh
        /// </summary>
        public int? FolderId { get; set; }
        /// <summary>
        /// Loại file
        /// 0 => HÌNH TOA THUỐC
        /// 1 => HÌNH XN
        /// 2 => HÌNH SIÊU ÂM
        /// 3 => HÌNH CHỤP X QUANG
        /// </summary>
        //public int AdditionServiceId { get; set; }
    }
}
