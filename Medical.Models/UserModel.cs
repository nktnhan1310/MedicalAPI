using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Người dùng
    /// </summary>
    public class UserModel : MedicalAppDomainModel
    {
        [Required(ErrorMessage = "Vui lòng nhập User Name!")]
        public string UserName { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(100, ErrorMessage = "Số kí tự của tên phải nhỏ hơn 100!")]
        public string FirstName { get; set; }
        /// <summary>
        /// Họ
        /// </summary>
        [StringLength(100, ErrorMessage = "Số kí tự của họ phải nhỏ hơn 100!")]
        public string LastName { get; set; }
        [StringLength(20, ErrorMessage = "Số kí tự của số điện thoại phải nhỏ hơn 20!")]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại!")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        [StringLength(50, ErrorMessage = "Số kí tự của email phải nhỏ hơn 50!")]
        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000, ErrorMessage = "Số kí tự của email phải nhỏ hơn 1000!")]
        public string Address { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Tuổi
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Phải là admin không
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        [StringLength(255, ErrorMessage = "Must be between 8 and 255 characters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        #region Extension Properties

        /// <summary>
        /// Những nhóm người dùng thuộc
        /// </summary>
        public IList<UserInGroupModel> UserInGroups { get; set; }

        /// <summary>
        /// Danh mục quyền ứng với chức năng người dùng
        /// </summary>
        public IList<PermitObjectPermissionModel> PermitObjectPermissions { get; set; }


        #endregion
    }
}
