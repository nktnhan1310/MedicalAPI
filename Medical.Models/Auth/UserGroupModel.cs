using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Nhóm người dùng
    /// </summary>
    public class UserGroupModel : MedicalCatalogueAppDomainModel
    {

        #region Extension Properties

        /// <summary>
        /// Người dùng thuộc nhóm
        /// </summary>
        public IList<UserInGroupModel> UserInGroups { get; set; }

        /// <summary>
        /// Chức năng + quyền của nhóm
        /// </summary>
        public IList<PermitObjectPermissionModel> PermitObjectPermissions { get; set; }

        #endregion
    }
}
