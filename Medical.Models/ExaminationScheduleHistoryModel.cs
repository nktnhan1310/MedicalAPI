using Medical.Models.DomainModel;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class ExaminationScheduleHistoryModel : MedicalAppDomainHospitalModel
    {
        public int? Action { get; set; }


        #region Extension Properties

        /// <summary>
        /// Dữ liệu cũ của lịch
        /// </summary>
        public ExaminationScheduleModel OldData { get; set; }

        /// <summary>
        /// Dữ liệu mới của lịch
        /// </summary>
        public ExaminationScheduleModel NewData { get; set; }

        /// <summary>
        /// Tên của hành động
        /// </summary>
        public string ActionName
        {
            get
            {
                if (!Action.HasValue) return string.Empty;
                switch (Action.Value)
                {
                    case (int)CatalogueUtilities.ExaminationAction.Create:
                            return "Thêm mới";
                    case (int)CatalogueUtilities.ExaminationAction.Update:
                        return "Cập nhật";
                    case (int)CatalogueUtilities.ExaminationAction.Delete:
                        return "Xóa";
                    default:
                        break;
                }
                return string.Empty;
            }
        }

        #endregion
    }
}
