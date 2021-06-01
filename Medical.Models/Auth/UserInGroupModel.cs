﻿using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Người dùng thuộc nhóm
    /// </summary>
    public class UserInGroupModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Người dùng
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public int UserGroupId { get; set; }


        #region Extension Properties

        /// <summary>
        /// Lấy thông tin Người dùng
        /// </summary>
        public UserModel Users { get; set; }

        /// <summary>
        /// Lấy thông tin Nhóm người dùng
        /// </summary>
        public UserGroupModel UserGroups { get; set; }

        #endregion
    }
}
