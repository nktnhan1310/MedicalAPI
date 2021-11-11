using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Quá trình chích ngừa của user
    /// </summary>
    public class UserVaccineProcesses : MedicalAppDomain
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
        [NotMapped]
        public string UserFullName { get; set; }

        /// <summary>
        /// Tên loại vaccine
        /// </summary>
        [NotMapped]
        public string VaccineTypeName { get; set; }

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalName { get; set; }

        #endregion

    }
}
