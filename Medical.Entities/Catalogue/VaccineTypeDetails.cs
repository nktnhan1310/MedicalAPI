using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Chi tiết liều chích cho từng loại vaccine
    /// </summary>
    public class VaccineTypeDetails : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã loại vaccine
        /// </summary>
        public int? VaccineTypeId { get; set; }

        /// <summary>
        /// Số tháng tiêm vaccine
        /// </summary>
        public int? MonthValue { get; set; }

        /// <summary>
        /// Cờ check có tiêm lặp lại không
        /// </summary>
        public bool IsRepeat { get; set; }

        /// <summary>
        /// Số tháng lặp lại
        /// </summary>
        public int? MonthRepeatValue { get; set; }
    }
}
