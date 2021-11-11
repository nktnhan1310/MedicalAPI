using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bảng lưu thông tin thiết bị connect tới app
    /// </summary>
    public class DeviceApps : MedicalAppDomain
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
        [NotMapped]
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// Tên của user
        /// </summary>
        [NotMapped]
        public string UserFullName { get; set; }

        #endregion
    }
}
