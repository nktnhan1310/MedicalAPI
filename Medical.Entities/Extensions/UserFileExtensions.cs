using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class UserFileExtensions
    {
        public int? Month { get; set; }

        public int? Year { get; set; }

        /// <summary>
        /// Danh sách file của user
        /// </summary>
        public IList<UserFiles> UserFiles { get; set; }
    }
}
