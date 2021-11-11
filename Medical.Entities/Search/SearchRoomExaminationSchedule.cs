using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Entities
{
    public class SearchRoomExaminationSchedule : BaseSearch
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }
        /// <summary>
        /// Mã phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Mã ca trực
        /// </summary>
        public int? SessionTypeId { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        [DefaultValue("RoomExaminationName desc")]
        public new string OrderBy { get; set; }
    }
}
