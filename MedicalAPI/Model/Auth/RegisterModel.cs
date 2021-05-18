using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    public class RegisterModel
    {
        /// <summary>
        /// Email hoặc số điện thoại
        /// </summary>
        [MaxLength(128, ErrorMessage = "Tên đăng nhập tối đa 128 ký tự")]
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc nhập")]
        public string UserName { set; get; }

        [StringLength(128, ErrorMessage = "Mật khẩu phải có ít nhất 8 kí tự và tối đa 128 ký tự", MinimumLength = 8)]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc nhập")]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu")]
        [StringLength(128, ErrorMessage = "Mật khẩu xác nhận phải có ít nhất 8 kí tự và tối đa 128 ký tự", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
