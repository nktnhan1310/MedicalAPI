using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
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
<<<<<<< HEAD

    
    public class UserFileRequestUpdateModel
    {
        /// <summary>
        /// Mã của user file
        /// </summary>
        public List<int> UserFileIds { get; set; }

        /// <summary>
        /// Mã của folder
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn folder cần cập nhật")]
        public int? UpdateFolderId { get; set; }
    }
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
}
