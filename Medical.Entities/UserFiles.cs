using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class UserFiles : MedicalAppDomainFile
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
