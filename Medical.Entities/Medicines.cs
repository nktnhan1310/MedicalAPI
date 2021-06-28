using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Thuốc trong toa
    /// </summary>
    public class Medicines : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Giá thuốc
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Số lượng thuốc
        /// </summary>
        public int? TotalAmount { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        public int? MedicalBillId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        [NotMapped]
        public string MedicalBillCode { get; set; }

        #endregion
    }
}
