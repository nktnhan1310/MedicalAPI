using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bảng loại vaccine
    /// </summary>
    public class VaccineTypes : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Giá
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Số thứ tự của vaccine
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// Khoảng cách mỗi liều
        /// 0 => Ngày
        /// 1 => Tuần
        /// 2 => Tháng
        /// 3 => Năm
        /// </summary>
        public int? DateTypeId { get; set; }

        /// <summary>
        /// Giá trị
        /// </summary>
        public int? NumberOfDateTypeValue { get; set; }

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
        /// Danh sách đối tượng tiêm chủng
        /// </summary>
        [NotMapped]
        public List<int> TargetIds
        {
            get
            {
                if(!string.IsNullOrEmpty(TargetIdValues))
                    return TargetIdValues.Split(';').Select(e => Convert.ToInt32(e)).ToList();
                return null;
            }
        }

        #endregion

    }
}
