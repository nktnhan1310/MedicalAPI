using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class ReportUserExaminationForm : ReportAppDomain
    {
        public DateTime? ExaminationDate { get; set; }
        public int? MonthValue { get; set; }
        public int? YearValue { get; set; }
        public int TotalUser { get; set; }
    }
}
