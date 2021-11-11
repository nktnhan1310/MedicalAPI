using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Danh sách mapping chi tiết dịch vụ của phiếu khám
    /// </summary>
    public class ExaminationFormAdditionServiceDetailMappings : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int ExaminationFormId { get; set; }
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int AdditionServiceDetailId { get; set; }

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
        /// Phí dịch vụ
        /// </summary>
        public double? Amount { get; set; }
    }
}
