using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    public class SearchHospital : BaseSearch
    {
        /// <summary>
        /// Tìm kiếm theo Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Tìm kiếm theo số điện thoại
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Tổng số lượt khám trong ngày
        /// </summary>
        public int? TotalVisitNo { get; set; }

    }
}
