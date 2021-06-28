﻿using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    [Table("Users")]
    public class Users : MedicalAppDomainHospital
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(100)]
        public string FirstName { get; set; }
        /// <summary>
        /// Họ
        /// </summary>
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000)]
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
        [StringLength(4000)]
        public string Password { get; set; }

        /// <summary>
        /// Token đăng nhập
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Thời gian hết hạn token
        /// </summary>
        public DateTime? ExpiredDate { get; set; }

        /// <summary>
        /// Họ tên người dùng
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// Số lần vi phạm
        /// </summary>
        public int? TotalViolations { get; set; }

        /// <summary>
        /// Cờ khóa tài khoản
        /// </summary>
        [DefaultValue(false)]
        public bool IsLocked { get; set; }

        /// <summary>
        /// Khóa đến ngày
        /// </summary>
        public DateTime? LockedDate { get; set; }

        /// <summary>
        /// Giới tính
        /// 0 => Nữ
        /// 1 => Nam
        /// </summary>
        [DefaultValue(false)]
        public bool Gender { get; set; }


        #region Extension Properties

        /// <summary>
        /// Những nhóm người dùng thuộc
        /// </summary>
        [NotMapped]
        public IList<UserInGroups> UserInGroups { get; set; }

        /// <summary>
        /// Danh mục quyền ứng với chức năng người dùng
        /// </summary>
        [NotMapped]
        public IList<PermitObjectPermissions> PermitObjectPermissions { get; set; }

        /// <summary>
        /// List file của người dùng
        /// </summary>
        [NotMapped]
        public IList<UserFiles> UserFiles { get; set; }


        #endregion

    }
}
