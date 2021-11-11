using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bảng mapper excel của danh mục import phòng khám
    /// </summary>
    public class RoomExaminationMapper
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        [Column(1)]
        public string HospitalCode { get; set; }
        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        [Column(2)]
        public string SpecialistTypeCode { get; set; }
        /// <summary>
        /// Tên phòng
        /// </summary>
        [Column(3)]
        public string Name { get; set; }

        /// <summary>
        /// Mô tả khu của phòng
        /// </summary>
        [Column(4)]
        public string ExaminationAreaDescription { get; set; }

        /// <summary>
        /// Số thứ tự phòng
        /// </summary>
        [Column(5)]
        public string RoomIndex { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [Column(6)]
        public string Description { get; set; }

        /// <summary>
        /// Kết quả trả về
        /// </summary>
        [Column(7)]
        public string ResultMessage { get; set; }
    }
}
