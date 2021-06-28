using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class MedicalRecordDetailFileModel : MedicalAppDomainFileModel
    {
        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Mã thông tin chi tiết dịch vụ phát sinh
        /// </summary>
        public int? ExaminationFormDetailId { get; set; }

        /// <summary>
        /// Loại file
        /// 0 => AVATAR CỦA HỒ SƠ
        /// </summary>
        public int AdditionServiceId { get; set; }
    }
}
