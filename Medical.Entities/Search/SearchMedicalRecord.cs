using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchMedicalRecord : BaseSearch
    {
        /// <summary>
        /// Mã hsnb
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Theo bệnh nhân
        /// </summary>
        public int? UserId { get; set; }
       
        /// <summary>
        /// Giới tính
        /// </summary>
        public int? Gender { get; set; }
        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public int? JobId { get; set; }
        /// <summary>
        /// Quốc gia
        /// </summary>
        public int? CountryId { get; set; }
        /// <summary>
        /// Dân tộc
        /// </summary>
        public int? NationId { get; set; }
        /// <summary>
        /// Thành phố
        /// </summary>
        public int? CityId { get; set; }
        /// <summary>
        /// Quận
        /// </summary>
        public int? DistrictId { get; set; }
        /// <summary>
        /// Phường
        /// </summary>
        public int? WardId { get; set; }



    }
}
