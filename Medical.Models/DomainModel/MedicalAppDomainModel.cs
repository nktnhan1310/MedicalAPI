using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models.DomainModel
{
    public class MedicalAppDomainModel
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public long RowNumber { get; set; }
        /// <summary>
        /// Khóa chính
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Tạo bởi
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? Updated { get; set; }
        /// <summary>
        /// Người cập nhật
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Cờ xóa
        /// </summary>
        [DefaultValue(false)]
        public bool Deleted { get; set; }
        /// <summary>
        /// Cờ active
        /// </summary>
        public bool Active { get; set; }

    }
}
