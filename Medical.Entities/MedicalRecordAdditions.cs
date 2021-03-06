using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// Mã hồ sơ
        /// </summary>
        public int MedicalRecordId { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(200)]
        public string FirstName { get; set; }
        /// <summary>
        /// Họ
        /// </summary>
        [StringLength(200)]
        public string LastName { get; set; }
        
        /// <summary>
        /// Quan hệ (anh/chị/em)
        /// </summary>
        public int? RelationId { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }

        #region Extension Properties

        /// <summary>
        /// Họ tên
        /// </summary>
        [NotMapped]
        public string FullName
        {
            get
            {
                return LastName + " " + FirstName;
            }
        }

        #endregion

    }
}
