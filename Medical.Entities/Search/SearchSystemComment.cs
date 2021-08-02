using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchSystemComment : BaseSearch
    {
        /// <summary>
        /// Tìm kiếm theo email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Tìm kiếm theo SDT
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Tìm kiếm theo mã người dủng
        /// </summary>
        public int? UserId { get; set; }
    }
}
