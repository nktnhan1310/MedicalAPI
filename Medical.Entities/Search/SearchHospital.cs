using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    public class SearchHospital : BaseSearch
    {
        private string _Email { get; set; }
        /// <summary>
        /// Tìm kiếm theo Email
        /// </summary>
        [StringLength(50, ErrorMessage = "Email không vượt quá 50 kí tự")]
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { 
            get { return _Email; } 
            set { _Email = string.IsNullOrWhiteSpace(value) ? null : value; }
        }
        /// <summary>
        /// Tìm kiếm theo số điện thoại
        /// </summary>
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        /// <summary>
        /// Tổng số lượt khám trong ngày
        /// </summary>
        public int? TotalVisitNo { get; set; }

    }
}
