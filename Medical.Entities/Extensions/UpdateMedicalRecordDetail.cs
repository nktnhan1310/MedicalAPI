using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class UpdateMedicalRecordDetail
    {
        /// <summary>
        /// Mã chi tiết hồ sơ bệnh án
        /// </summary>
        public int MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Danh sách file chi tiết hồ sơ bệnh án
        /// </summary>
        public IList<MedicalRecordDetailFiles> MedicalRecordDetailFiles { get; set; }
    }
}
