using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    public class MedicalRecordHistories : MedicalAppDomain
    {
        /// <summary>
        /// Mô tả/ ghi chú
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Bộ phận phẫu thuật
        /// </summary>
        public string SurgeryPart { get; set; }

        /// <summary>
        /// Năm phẫu thuật
        /// </summary>
        public int? SurgeryYear { get; set; }

        /// <summary>
        /// Chỉ định phẫu thuật
        /// </summary>
        public string SurgeryIndication { get; set; }

        /// <summary>
        /// Ngày phẫu thuật
        /// </summary>
        public DateTime? SurgeryDate { get; set; }

        /// <summary>
        /// Nơi phẫu thuật
        /// </summary>
        public string SurgeryPlace { get; set; }

        /// <summary>
        /// Kết quả phẫu thuật
        /// </summary>
        public string SurgeryResult { get; set; }

        /// <summary>
        /// Biến chứng
        /// </summary>
        public string SympToms { get; set; }

        /// <summary>
        /// Loại
        /// 0 => tiền sử bệnh
        /// 1 => tiền sử phẫu thuật
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Mã hồ sơ bệnh nhân
        /// </summary>
        public int? MedicalRecordId { get; set; }
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        [NotMapped]
        public string UserFullName { get; set; }
        /// <summary>
        /// Số điện thoại của user
        /// </summary>
        [NotMapped]
        public string UserPhone { get; set; }
        /// <summary>
        /// Email của user
        /// </summary>
        [NotMapped]
        public string UserEmail { get; set; }

        /// <summary>
        /// Danh sách file của user
        /// </summary>
        [NotMapped]
        public IList<UserFiles> UserFiles { get; set; }

        #endregion

    }
}
