using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Models
{
    public class OneSignalDataModel
    {
        /// <summary>
        /// Mã divice Id của người dùng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng điền thông tin PlayerId")]
        public string PlayerId { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại thiết bị")]
        public int? TypeId { get; set; }

        /// <summary>
        /// Trạng thái Subscribe/Unsubscribe của PlayerId
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn trạng thái Subscribe/Unsubscribe")]
        public bool? Active { get; set; }
    }
}
