using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class UserGeneralInfo
    {
        /// <summary>
        /// Thông tin account
        /// </summary>
        public Users User { get; set; }
        /// <summary>
        /// Thông tin của hồ sơ người bệnh
        /// </summary>
        public MedicalRecords MedicalRecord { get; set; }

        /// <summary>
        /// Danh sách file của user
        /// </summary>
        public IList<UserFiles> UserFiles { get; set; }
    }
}
