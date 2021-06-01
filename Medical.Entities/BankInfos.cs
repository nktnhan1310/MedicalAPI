using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Thông tin ngân hàng thanh toán
    /// </summary>
    public class BankInfos : MedicalCatalogueAppDomainHospital
    {
       
        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        [StringLength(50)]
        public string BankNo { get; set; }
        /// <summary>
        /// Chi nhánh ngân hàng
        /// </summary>
        [StringLength(1000)]
        public string BankBranch { get; set; }

        /// <summary>
        /// Tên chủ sở hữu tài khoản
        /// </summary>
        [StringLength(500)]
        public string OwnerName { get; set; }
        /// <summary>
        /// Thông tin chi tiết của ngân hàng
        /// </summary>
        [StringLength(1000)]
        public string BankDescription { get; set; }

        /// <summary>
        /// Cú pháp soạn tin nhắn
        /// </summary>
        [StringLength(1000)]
        public string BankSyntax { get; set; }
    }
}
