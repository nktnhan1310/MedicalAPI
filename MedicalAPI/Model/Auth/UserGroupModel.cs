using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
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
