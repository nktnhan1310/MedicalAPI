using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserVaccineProcessModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Loại vaccine
        /// </summary>
        public int? VaccineTypeId { get; set; }

        /// <summary>
        /// Ngày tiêm
        /// </summary>
        public DateTime InjectDate { get; set; }

        /// <summary>
        /// Hình thức tiêm
        /// 0 => trước sinh
        /// 1 => trong thai kì
        /// 2 => sau sinh (< 12 tuổi)
        /// 3 => khác
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Mô tả nơi tiêm (nếu có)
        /// </summary>
        public string InjectPlaceDescription { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Mã bệnh viện nếu có
        /// </summary>
        public int? HospitalId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// Tên loại vaccine
        /// </summary>
        public string VaccineTypeName { get; set; }

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }

        #endregion
    }
}
