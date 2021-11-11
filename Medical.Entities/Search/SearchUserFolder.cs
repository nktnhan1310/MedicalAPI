using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities.Search
{
    public class SearchUserFolder: BaseSearch
    {
        public int? UserId { get; set; }
        public int? TypeId { get; set; }
        /// <summary>
        /// Search theo tháng
        /// </summary>
        public int? Month { get; set; }
        /// <summary>
        /// Filter theo năm
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// 0 => Theo tháng
        /// 1 => Theo năm
        /// </summary>
        public int? FilterType { get; set; }
    }
}
