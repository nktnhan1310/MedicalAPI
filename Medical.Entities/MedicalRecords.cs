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
    public class MedicalRecords : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        [StringLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        [StringLength(1000)]
        public string UserFullName { get; set; }

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
        /// Tiểu sử bệnh
        /// </summary>
        public string MedicalHistory { get; set; }

        /// <summary>
        /// Thông tin dị ứng
        /// </summary>
        public string AllergyInformation { get; set; }

        /// <summary>
        /// Chiều cao
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Cân nặng
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Nhóm máu
        /// </summary>
        public string BloodType { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên quận
        /// </summary>
        [NotMapped]
        public string DistrictName { get; set; }

        /// <summary>
        /// Tên phường
        /// </summary>
        [NotMapped]
        public string WardName { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        [NotMapped]
        public string CityName { get; set; }

        /// <summary>
        /// Tên quốc gia
        /// </summary>
        [NotMapped]
        public string CountryName { get; set; }

        /// <summary>
        /// Tên nghề nghiệp
        /// </summary>
        [NotMapped]
        public string JobName { get; set; }

        /// <summary>
        /// Tên dân tộc
        /// </summary>
        [NotMapped]
        public string NationName { get; set; }

        /// <summary>
        /// Số tuổi hiện tại của user
        /// </summary>
        [NotMapped]
        public int? Age
        {
            get
            {
                if (BirthDate.HasValue) return DateTime.Now.Year - BirthDate.Value.Year;

                return null;
            }
        }

        /// <summary>
        /// Thông tin người thân (nếu có)
        /// </summary>
        [NotMapped]
        public IList<MedicalRecordAdditions> MedicalRecordAdditions { get; set; }

        /// <summary>
        /// List file theo hồ sơ
        /// </summary>
        //[NotMapped]
        //public IList<MedicalRecordFiles> MedicalRecordFiles { get; set; }

        /// <summary>
        /// File của user
        /// </summary>
        [NotMapped]
        public IList<UserFiles> UserFiles { get; set; }

        /// <summary>
        /// Chi tiết hồ sơ khám bệnh (Hồ sơ bệnh án)
        /// </summary>
        [NotMapped]
        public IList<MedicalRecordDetails> MedicalRecordDetails { get; set; }

        #endregion


    }
}
