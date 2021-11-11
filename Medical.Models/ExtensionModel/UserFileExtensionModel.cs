using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserFileExtensionModel
    {
        public int? Month { get; set; }

        public int? Year { get; set; }

        /// <summary>
        /// Danh sách file của user
        /// </summary>
        public IList<UserFileModel> UserFiles { get; set; }
    }
}
