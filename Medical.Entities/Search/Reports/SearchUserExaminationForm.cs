using Medical.Entities.DomainEntity.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchUserExaminationForm : ReportBaseSearch
    {
        /// <summary>
        /// Theo bệnh viện
        /// </summary>
        public int HospitalId { get; set; }
        /// <summary>
        /// Theo phân loại
        /// <para>0 => Ngày</para>
        /// <para>1 => Tháng</para>
        /// <para>2 => Năm</para>
        /// </summary>
        public int SelectedType { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }
        /// <summary>
        /// Tháng nếu có
        /// </summary>
        public int? Month { get; set; }
        /// <summary>
        /// Năm
        /// </summary>
        public int? Year { get; set; }
    }
}
