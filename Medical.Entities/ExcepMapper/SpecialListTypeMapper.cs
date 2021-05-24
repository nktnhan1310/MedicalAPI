using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SpecialListTypeMapper
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
        public string Code { get; set; }
        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        [Column(3)]
        public string Name { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        [Column(4)]
        public string Description { get; set; }

        /// <summary>
        /// Giá khám theo chuyên khoa
        /// </summary>
        [Column(5)]
        public string Price { get; set; }

        /// <summary>
        /// Kết quả trả về
        /// </summary>
        [Column(6)]
        public string ResultMessage { get; set; }
    }
}
