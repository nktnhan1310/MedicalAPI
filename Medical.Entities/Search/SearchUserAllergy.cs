using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchUserAllergy : BaseSearch
    {
        /// <summary>
        /// Loại dị ứng
        /// </summary>
        public int? AllergyTypeId { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
    }
}
