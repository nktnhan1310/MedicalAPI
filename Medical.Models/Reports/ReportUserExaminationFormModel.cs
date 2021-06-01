using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class ReportUserExaminationFormModel : ReportAppDomainModel
    {
        public DateTime? ExaminationDate { get; set; }
        public int? MonthValue { get; set; }
        public int? YearValue { get; set; }
        public int TotalUser { get; set; }
        /// <summary>
        /// Ngày hiển thị
        /// </summary>
        public string ValueDisplay
        {
            get
            {
                if (ExaminationDate.HasValue)
                    return ExaminationDate.Value.ToString("dd/MM/yyyy");
                else if (MonthValue.HasValue)
                    return string.Format("Tháng {0}/{1}", MonthValue.Value.ToString(), YearValue.HasValue ? YearValue.Value.ToString() : string.Empty);
                else if (YearValue.HasValue)
                    return YearValue.ToString();
                return string.Empty;
            }
        }
    }
}
