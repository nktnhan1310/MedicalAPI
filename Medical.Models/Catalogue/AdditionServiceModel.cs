using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Models
{
    /// <summary>
    /// Dịch vụ phát sinh 
    /// </summary>
    public class AdditionServiceModel : MedicalCatalogueAppDomainHospitalModel
    {
        /// <summary>
        /// Giá dịch vụ phát sinh
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Cờ check có cho chọn loại vaccine ko
        /// </summary>
        [DefaultValue(false)]
        public bool IsVaccineSelected { get; set; }

        /// <summary>
        /// Danh sách chi tiết dịch vụ phát sinh
        /// </summary>
        public IList<AdditionServiceDetailModel> AdditionServiceDetails { get; set; }

    }
}
