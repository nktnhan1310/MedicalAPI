using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserFileModel : MedicalAppDomainFileModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Loại file
        /// 0 => avatar;
        /// </summary>
        public int FileType { get; set; }
    }
}
