using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch theo dõi thai kì
    /// </summary>
    public class UserPregnancies : MedicalAppDomain
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Giá trị của tuần/tháng/năm
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// Loại theo dõi
        /// Tuần
        /// Tháng
        /// Ngày
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Các hình thức khám thai
        /// </summary>
        [NotMapped]
        public IList<UserPregnancyDetails> UserPregnancyDetails { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        [NotMapped]
        public string UserFullName { get; set; }

        #endregion

    }
}
