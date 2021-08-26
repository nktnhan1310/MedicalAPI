using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class DashBoardSynthesisResponse
    {
        public long? RowNumber { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Ngày khám string
        /// </summary>
        public string ExaminationDate_s
        {
            get
            {
                return ExaminationDate.HasValue ? ExaminationDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        /// <summary>
        /// Giá trị tháng
        /// </summary>
        public int? MonthValue { get; set; }
        /// <summary>
        /// Giá trị năm
        /// </summary>
        public int? YearValue { get; set; }

        /// <summary>
        /// Tổng số phiếu khám
        /// </summary>
        public int? TotalExaminationForm { get; set; }

        /// <summary>
        /// Tổng số phiếu hủy
        /// </summary>
        public int? TotalCancelExamination { get; set; }

        /// <summary>
        /// Tổng số người khám bệnh
        /// </summary>
        public int? TotalUserExamination { get; set; }

    }
}
