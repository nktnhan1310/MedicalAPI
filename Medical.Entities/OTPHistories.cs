﻿using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch sử OTP
    /// </summary>
    public class OTPHistories : MedicalAppDomain
    {
        /// <summary>
        /// Thông tin người dùng
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// SDT gửi OTP
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Email gửi OTP
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Mã OTP
        /// </summary>
        public string OTPValue { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        public DateTime? ExpiredDate { get; set; }

        [DefaultValue(false)]
        public bool IsEmail { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set; }
    }
}
