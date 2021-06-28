using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class MedicineModel : MedicalCatalogueAppDomainHospitalModel
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
        public string MedicalBillCode { get; set; }

        #endregion
    }
}
