using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchHopitalExtension : BaseHospitalSearch
    {
        /// <summary>
        /// Mã phòng
        /// </summary>
        public int? RoomExaminationId { get; set; }
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromExaminationDate { get; set; }
        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToExaminationDate { get; set; }

    }
}
