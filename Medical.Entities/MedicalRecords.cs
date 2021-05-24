using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Hồ sơ khám bệnh
    /// </summary>
    [Table("MedicalRecords")]
    public class MedicalRecords : MedicalAppDomain
    {
        /// <summary>
        /// Mã bệnh nhân
        /// </summary>
        [StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(200)]
        public string FirstName { get; set; }
        /// <summary>
        /// Họ và tên lót
        /// </summary>
        [StringLength(200)]
        public string LastName { get; set; }
        /// <summary>
        /// Sinh nhật
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Giới tính
        /// 0 => Nam
        /// 1 => Nữ
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// Chứng minh nhân dân
        /// </summary>
        [StringLength(20)]
        public string CertificateNo { get; set; }
        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public int? JobId { get; set; }
        /// <summary>
        /// Quốc gia
        /// </summary>
        public int? CountryId { get; set; }
        /// <summary>
        /// Dân tộc
        /// </summary>
        public int? NationId { get; set; }
        /// <summary>
        /// Thành phố
        /// </summary>
        public int? CityId { get; set; }
        /// <summary>
        /// Quận
        /// </summary>
        public int? DistrictId { get; set; }
        /// <summary>
        /// Phường/Xã
        /// </summary>
        public int? WardId { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// Id bệnh nhân
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Id bệnh viện
        /// </summary>
        public int HospitalId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        [NotMapped]
        public string FullName
        {
            get
            {
                return LastName + " " + FirstName;
            }
        }

        /// <summary>
        /// Thông tin người thân (nếu có)
        /// </summary>
        [NotMapped]
        public IList<MedicalRecordAdditions> MedicalRecordAdditions { get; set; }

        #endregion


    }
}
