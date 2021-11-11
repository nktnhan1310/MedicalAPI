using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchAppPolicy : BaseHospitalSearch
    {
        /// <summary>
        /// Loại chính sách
        /// </summary>
        public int? TypeId { get; set; }

    }
}
