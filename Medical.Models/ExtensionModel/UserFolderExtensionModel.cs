using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserFolderExtensionModel
    {
        public int? Month { get; set; }
        public int? Year { get; set; }

        /// <summary>
        /// Danh sách folder
        /// </summary>
        public IList<UserFolderModel> UserFolders { get; set; }
    }
}
