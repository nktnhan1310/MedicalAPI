using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Medical.Entities.DomainEntity
{
    public class MedicalAppDomain
    {
        public MedicalAppDomain()
        {
            Created = DateTime.Now;
        }

        /// <summary>
        /// STT
        /// </summary>
        [NotMapped]
        public long RowNumber { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        /// <summary>
        /// Khóa chính
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        [Required]
        public DateTime Created { get; set; }
        /// <summary>
        /// Tạo bởi
        /// </summary>
        [StringLength(50)]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? Updated { get; set; }
        /// <summary>
        /// Người cập nhật
        /// </summary>
        [StringLength(50)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Cờ xóa
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// Cờ active
        /// </summary>
        public bool Active { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tổng số item của trang danh sách
        /// </summary>
        [NotMapped]
        public int TotalItem { get; set; }

        #endregion

    }
}
