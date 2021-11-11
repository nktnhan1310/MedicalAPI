using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    public class SearchDashBoardUserSystem
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// Mã nhóm người dùng
        /// </summary>
        public int? UserGroupId { get; set; }

        /// <summary>
        /// Cờ check tài khoản có bị khóa không?
        /// </summary>
        public bool? IsLocked { get; set; }

        /// <summary>
        /// Cờ check tài số tài khoản bị hủy?
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Cờ check tài khoản có đang hoạt động không?
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
