using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class DashBoardSynthesisByHospitalResponse
    {
        public int? HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int? TotalUser { get; set; }
        public int? TotalCancelExamination { get; set; }
        public int? TotalExamination { get; set; }

    }
}
