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
        /// Họ và tên lót
        /// </summary>
        [StringLength(200, ErrorMessage = "Số kí tự của Họ và tên lót phải nhỏ hơn 200!")]
        public string FirstName { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(200, ErrorMessage = "Số kí tự của Tên phải nhỏ hơn 200!")]
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

        [StringLength(20, ErrorMessage = "Số kí tự của Số điện thoại phải nhỏ hơn 20!")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không hợp lệ")]
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


        #region Extension Properties

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
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
        public IList<MedicalRecordAdditionModel> MedicalRecordAdditions { get; set; }


        #endregion
    }
}
