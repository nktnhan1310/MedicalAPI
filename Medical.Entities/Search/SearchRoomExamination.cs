using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchRoomExamination : BaseHospitalSearch
    {
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }
    }
}
