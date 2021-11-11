using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class UserFolderExtensions
    {
        public int? Month { get; set; }
        public int? Year { get; set; }

        /// <summary>
        /// Danh sách folder
        /// </summary>
        public IList<UserFolders> UserFolders { get; set; }
    }
}
