using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Hồ sơ khám bệnh
    /// </summary>
    public class MedicalRecordModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Mã bệnh nhân
        /// </summary>
        [StringLength(50, ErrorMessage = "Số kí tự của Mã bệnh viện phải nhỏ hơn 50!")]
        public string Code { get; set; }

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        [StringLength(1000, ErrorMessage = "Số kí tự của họ tên phải nhỏ hơn 1000!")]
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
        [StringLength(20, ErrorMessage = "Số kí tự của Chứng minh nhân dân phải nhỏ hơn 20!")]
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
        [StringLength(1000, ErrorMessage = "Số kí tự của Địa chỉ phải nhỏ hơn 1000!")]
        public string Address { get; set; }

        [StringLength(12, ErrorMessage = "Số kí tự của số điện thoại phải lớn hơn 8 và nhỏ hơn 12!", MinimumLength = 9)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        [StringLength(50, ErrorMessage = "Số kí tự của Email phải nhỏ hơn 50!")]
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        /// <summary>
        /// Id bệnh nhân
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Id bệnh viện
        /// </summary>
        public new int HospitalId { get; set; }

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
        public string DistrictName { get; set; }

        /// <summary>
        /// Tên phường
        /// </summary>
        public string WardName { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Tên quốc gia
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Tên nghề nghiệp
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Tên dân tộc
        /// </summary>
        public string NationName { get; set; }

        /// <summary>
        /// Trả ra số tuổi của user
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Thông tin người thân (nếu có)
        /// </summary>
        public IList<MedicalRecordAdditionModel> MedicalRecordAdditions { get; set; }

        /// <summary>
        /// Thông tin file của hồ sơ bệnh án
        /// </summary>
        //public IList<MedicalRecordFileModel> MedicalRecordFiles { get; set; }
        public IList<UserFileModel> UserFiles { get; set; }


        /// <summary>
        /// Lịch sử khám
        /// </summary>
        public IList<MedicalRecordDetailModel> MedicalRecordDetails { get; set; }


        #endregion
    }
}
