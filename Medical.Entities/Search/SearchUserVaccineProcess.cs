using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchUserVaccineProcess : BaseSearch
    {
        /// <summary>
        /// Tìm kiếm theo loại vaccine
        /// </summary>
        public int? VaccineTypeId { get; set; }

        /// <summary>
        /// Tìm theo tên user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Tìm theo mã hồ sơ
        /// </summary>
        public int? MedicalRecordId { get; set; }
        /// <summary>
        /// Tìm theo loại quy trình
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }


        /// <summary>
        /// Ngày tiêm
        /// </summary>
        public DateTime? InjectDate { get; set; }
    }
}
