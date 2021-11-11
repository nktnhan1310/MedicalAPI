using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Tìm theo loại (bác sĩ/y tá/điều dưỡng)
        /// </summary>
        public int? TypeId { get; set; }
    }
}
