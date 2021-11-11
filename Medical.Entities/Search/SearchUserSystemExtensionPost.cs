using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchUserSystemExtensionPost : BaseSearch
    {
        /// <summary>
        /// Đối tượng của bài viết
        /// </summary>
        public int? TargetTypeId { get; set; }

        /// <summary>
        /// Loại bài viết
        /// </summary>
        public int? PostTypeId { get; set; }
    }
}
