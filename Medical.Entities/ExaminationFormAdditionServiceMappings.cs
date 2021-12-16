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
<<<<<<< HEAD
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordDetailId { get; set; }

        /// <summary>
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Phí dịch vụ
        /// </summary>
        public double? Amount { get; set; }
    }
}
