using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserVaccineTypeModel
    {
        /// <summary>
        /// Loại vaccine
        /// </summary>
        public int? VaccineTypeId { get; set; }

        /// <summary>
        /// Tên vaccine
        /// </summary>
        public string VaccineTypeName { get; set; }

        /// <summary>
        /// Ngày tiêm
        /// </summary>
        public DateTime? InjectDate { get; set; }

        /// <summary>
        /// Ngày tiêm hiển thị
        /// </summary>
        public string InjectDateDisplay
        {
            get
            {
                return InjectDate.HasValue ? InjectDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string StatusName
        {
            get
            {
                if (Status.HasValue)
                {
                    switch (Status.Value)
                    {
                        case 0:
                            return "Chưa chích";
                        case 1:
                            return "Đã chích";

                        default:
                            break;
                    }
                }
                return string.Empty;
            }
        }
    }
}
