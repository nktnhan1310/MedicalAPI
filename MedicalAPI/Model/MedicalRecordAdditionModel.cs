using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    public class MedicalRecordAdditionModel : MedicalAppDomainModel
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
        public string FullName { get; set; }

        #endregion

    }
}
