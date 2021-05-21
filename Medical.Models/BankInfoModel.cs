using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Models
{
    /// <summary>
    /// Thông tin ngân hàng thanh toán
    /// </summary>
    public class BankInfoModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        [StringLength(50, ErrorMessage = "STK vui lòng nhỏ hơn {0} kí tự")]
        public string BankNo { get; set; }
        /// <summary>
        /// Chi nhánh ngân hàng
        /// </summary>
        [StringLength(1000, ErrorMessage = "Tên chi nhánh ngân hàng phải nhỏ hơn {0} kí tự")]
        public string BankBranch { get; set; }

        /// <summary>
        /// Tên chủ sở hữu tài khoản
        /// </summary>
        [StringLength(500, ErrorMessage = "Tên chủ sở hữu tài khoản phải nhỏ hơn {0} kí tự")]
        public string OwnerName { get; set; }
        /// <summary>
        /// Thông tin chi tiết của ngân hàng
        /// </summary>
        [StringLength(1000, ErrorMessage = "Thông tin chi tiết của ngân hàng phải nhỏ hơn {0} kí tự")]
        public string BankDescription { get; set; }

        /// <summary>
        /// Cú pháp soạn tin nhắn
        /// </summary>
        [StringLength(1000, ErrorMessage = "Cú pháp soạn tin nhắn phải nhỏ hơn {0} kí tự")]
        public string BankSyntax { get; set; }
    }
}
