using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserGeneralInfoModel
    {
        /// <summary>
        /// Thông tin của account
        /// </summary>
        public UserModel User { get; set; }
        /// <summary>
        /// Thông tin của hồ sơ người bệnh
        /// </summary>
        public MedicalRecordModel MedicalRecord { get; set; }

        /// <summary>
        /// Danh sách file của user
        /// </summary>
        public IList<UserFileModel> UserFiles { get; set; }
    }
}
