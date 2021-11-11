using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchUserFile : BaseSearch
    {
        /// <summary>
        /// Tìm theo folder
        /// </summary>
        public int? FolderId { get; set; }

        /// <summary>
        /// Tìm theo type
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Loại filter
        /// 0 => Tháng
        /// 1 => Năm
        /// </summary>
        public int? FilterType { get; set; }

        /// <summary>
        /// Tìm theo tháng
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Tìm theo năm
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Tìm theo user
        /// </summary>
        public int? UserId { get; set; }
    }
}
