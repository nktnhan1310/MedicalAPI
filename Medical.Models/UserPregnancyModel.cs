using Medical.Models.DomainModel;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UserPregnancyModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã user
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Giá trị của tuần/tháng/năm
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// Loại theo dõi
        /// Tuần
        /// Tháng
        /// Ngày
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Các hình thức khám thai
        /// </summary>
        public IList<UserPregnancyDetailModel> UserPregnancyDetails { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên đầy đủ của user
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// Tên khoảng thời gian thai kì
        /// </summary>
        public string TypeName
        {
            get
            {
                if (TypeId.HasValue && Index.HasValue)
                {
                    switch (TypeId.Value)
                    {
                        // Theo ngày
                        case (int)CatalogueUtilities.PregnancyType.Day:
                            return string.Format("Ngày thứ {0}", Index.Value);
                        // Theo tuần
                        case (int)CatalogueUtilities.PregnancyType.Week:
                            return string.Format("Tuần thứ {0}", Index.Value);
                        // Theo tháng
                        case (int)CatalogueUtilities.PregnancyType.Month:
                            return string.Format("Tháng thứ {0}", Index.Value);
                        default:
                            return string.Empty;
                    }

                }
                return string.Empty;
            }
        }

        #endregion
    }
}
