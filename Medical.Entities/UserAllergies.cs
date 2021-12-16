﻿using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    public class UserAllergies : MedicalAppDomain
    {
        /// <summary>
        /// Mã loại dị ứng
        /// </summary>
        public int? AllergyTypeId { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
<<<<<<< HEAD
        /// Tên dị ứng khác (USER tự nhập)
        /// </summary>
        public string OtherName { get; set; }

        /// <summary>
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

<<<<<<< HEAD
        /// <summary>
        /// Danh sách mô tả dị ứng mà user check
        /// </summary>
        [NotMapped]
        public IList<UserAllergyDetails> UserAllergyDetails { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách mô tả dị ứng mà user đã chọn
        /// </summary>
        [NotMapped]
        public List<int> DescriptionTypeIds { get; set; }

        /// <summary>
=======
        #region Extension Properties

        /// <summary>
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Tên loại dị ứng
        /// </summary>
        [NotMapped]
        public string AllergyTypeName { get; set; }

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        [NotMapped]
        public string UserFullName { get; set; }

        #endregion
    }
}