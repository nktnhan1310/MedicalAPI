using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchDoctor : BaseHospitalSearch
    {
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialListTypeId { get; set; }

        /// <summary>
        /// Học vị/học hàm
        /// </summary>
        public int? DegreeId { get; set; }
    }
}
