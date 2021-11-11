using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Models
{
    public class VaccineTypeModel : MedicalCatalogueAppDomainHospitalModel
    {
        /// <summary>
        /// Giá theo chuyên khoa
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// Loại vaccine
        /// </summary>
        //public int? VaccineTypeId { get; set; }

        ///// <summary>
        ///// Khoảng cách mỗi liều
        ///// 0 => Ngày
        ///// 1 => Tuần
        ///// 2 => Tháng
        ///// 3 => Năm
        ///// </summary>
        //public int? DateTypeId { get; set; }

        ///// <summary>
        ///// Giá trị
        ///// </summary>
        //public int? NumberOfDateTypeValue { get; set; }

        /// <summary>
        /// Số liều thuốc
        /// </summary>
        public int? NumberOfDoses { get; set; }

        /// <summary>
        /// Lưu id đối tượng được tiêm của loại vaccine
        /// </summary>
        public string TargetIdValues { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách chi tiết của loại vaccine
        /// </summary>
        public IList<VaccineTypeDetailModel> VaccineTypeDetails { get; set; }

        /// <summary>
        /// Danh sách đối tượng tiêm chủng
        /// </summary>
        public List<int> TargetIds { get; set; }

        #endregion

    }
}
