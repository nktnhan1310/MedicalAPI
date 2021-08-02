using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class SystemCommentModel : MedicalAppDomainModel
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
