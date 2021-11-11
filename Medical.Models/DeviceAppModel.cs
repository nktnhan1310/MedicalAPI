using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class DeviceAppModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Id của thiết bị
        /// </summary>
        public string PlayerId { get; set; }
        /// <summary>
        /// Mã loại thiết bị (browser/android/ios)
        /// </summary>
        public int DeviceTypeId { get; set; }

        /// <summary>
        /// Mã user
        /// </summary>
        public int UserId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên loại thiết bị
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// Tên của user
        /// </summary>
        public string UserFullName { get; set; }

        #endregion
    }
}
