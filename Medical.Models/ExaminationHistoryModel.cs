using Medical.Models.DomainModel;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Lịch sử của lịch hẹn (phiếu khám)
    /// </summary>
    public class ExaminationHistoryModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã lịch hẹn
        /// </summary>
        public int ExaminationFormId { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }

        /// <summary>
        /// STT khám bệnh
        /// </summary>
        public string ExaminationIndex { get; set; }

        /// <summary>
        /// STT chờ khám bệnh
        /// </summary>
        public string ExaminationPaymentIndex { get; set; }

        /// <summary>
        /// Hành động (Tạo lịch hẹn,...)
        /// </summary>
        public int Action { get; set; }
        /// <summary>
        /// Trạng thái lịch hẹn (Chờ xác nhận,....)
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Comment khi duyệt phiếu khám
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Mô tả lịch hẹn
        /// </summary>
        public string Note { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case (int)CatalogueUtilities.ExaminationStatus.New:
                        return "Lưu nháp";
                    case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                        return "Chờ xác nhận";
                    case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                        return "Chờ xác nhận tái khám";
                    case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                        return "Đã hủy";
                    case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                        return "Đã xác nhận";
                    case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                        return "Đã xác nhận tái khám";
                    case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                        return "Hoàn thành";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Tên hành động
        /// </summary>
        public string ActionName
        {
            get
            {
                switch (Action)
                {
                    case (int)CatalogueUtilities.ExaminationAction.Cancel:
                        return "Hủy";
                    case (int)CatalogueUtilities.ExaminationAction.Confirm:
                        return "Xác nhận khám";
                    case (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination:
                        return "Xác nhận tái khám";
                    case (int)CatalogueUtilities.ExaminationAction.Create:
                        return "Tạo mới";
                    case (int)CatalogueUtilities.ExaminationAction.Update:
                        return "Cập nhật";
                    case (int)CatalogueUtilities.ExaminationAction.FinishExamination:
                        return "Hoàn thành";
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion

    }
}
