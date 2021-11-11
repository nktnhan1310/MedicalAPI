using Medical.Entities.DomainEntity;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Buổi khám
    /// </summary>
    [Table("SessionTypes")]
    public class SessionTypes : MedicalCatalogueAppDomainHospital
    {
        /// <summary>
        /// Số thứ tự buổi
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Từ giờ
        /// </summary>
        public int? FromTime { get; set; }
        /// <summary>
        /// Đến giờ
        /// </summary>
        public int? ToTime { get; set; }

        #region Extension Properties

        /// <summary>
        /// Từ giờ hiển thị
        /// </summary>
        [NotMapped]
        public string FromTimeDisplayValue
        {
            get
            {
                string result = string.Empty;
                if (FromTime.HasValue) return DateTimeUtilities.ConvertTotalMinuteToString(FromTime.Value);
                return result;
            }
        }

        /// <summary>
        /// Đến giờ hiển thị
        /// </summary>
        [NotMapped]
        public string ToTimeDisplayValue
        {
            get
            {
                string result = string.Empty;
                if (ToTime.HasValue) return DateTimeUtilities.ConvertTotalMinuteToString(ToTime.Value);
                return result;
            }
        }



        #endregion

    }
}
