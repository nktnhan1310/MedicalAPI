using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Liên hệ của hệ thống
    /// </summary>
    public class SystemComments : MedicalAppDomain
    {
        /// <summary>
        /// Người đăng comment
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Nội dung hỗ trợ
        /// </summary>
        public string Content { get; set; }
    }
}
