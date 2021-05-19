using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Chức năng người dùng
    /// </summary>
    public class PermitObjectModel : MedicalCatalogueAppDomainModel
    {
        /// <summary>
        /// Tên controller
        /// </summary>
        public string ControllerNames { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách tên controller
        /// </summary>
        public IList<string> Controllers { get; set; }


        public void ToModel()
        {
            ControllerNames = string.Join(";", Controllers);
        }

        public void ToView()
        {
            if (!string.IsNullOrEmpty(ControllerNames))
            {
                Controllers = ControllerNames.Split(";");
            }
        }


        #endregion

    }
}
