using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc nhập")]
        public string UserName { set; get; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc nhập")]
        public string Password { set; get; }
    }
}
