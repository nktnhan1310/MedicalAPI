using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Dịch vụ phát sinh lúc khám (xét nghiệm/x-quang)
    /// </summary>
    public class AdditionServices : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Giá dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Cờ check cho chọn loại vaccine hay ko
        /// </summary>
        [DefaultValue(false)]
        public bool IsVaccineSelected { get; set; }
    }
}
