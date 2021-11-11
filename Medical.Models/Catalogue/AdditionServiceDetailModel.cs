using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class AdditionServiceDetailModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Tên chi tiết dịch vụ phát sinh
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả chi tiết chi phí phát sinh
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Mã dịch vụ phát sinh
        /// </summary>
        public int? AdditionServiceId { get; set; }

        /// <summary>
        /// Chi phí của chi tiết dịch vụ phát sinh
        /// </summary>
        public double? Price { get; set; }
    }
}
