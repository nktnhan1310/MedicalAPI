using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Thông tin người thân hồ sơ khám bệnh
    /// </summary>
    [Table("MedicalRecordAdditions")]
    public class MedicalRecordAdditions : MedicalAppDomain
    {
        /// <summary>
        /// Tên
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Họ
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Họ tên
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Quan hệ (anh/chị/em)
        /// </summary>
        public int? RelationId { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
