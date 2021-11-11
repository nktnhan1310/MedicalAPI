using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class ExaminationFormAdditionServiceMappings : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã phiếu khám
        /// </summary>
        public int ExaminationFormId { get; set; }
        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public int AdditionServiceId { get; set; }

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
