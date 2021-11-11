using Medical.Models.DomainModel;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// Mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Ca khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Chọn lại phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }

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

        /// <summary>
        /// Cờ check chỉnh sửa hủy => trạng thái khác
        /// </summary>
        [DefaultValue(false)]
        public bool IsEdit { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        public string HospitalName { get; set; }

        /// <summary>
        /// Địa chỉ bệnh viện
        /// </summary>
        public string HospitalAddress { get; set; }

        /// <summary>
        /// Số điện thoại bệnh viện
        /// </summary>
        public string HospitalPhone { get; set; }

        /// <summary>
        /// Link Url Website của bệnh viện
        /// </summary>
        public string HospitalWebSite { get; set; }

        /// <summary>
        /// Tên dịch vụ khám
        /// </summary>
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Mã dịch vụ khám
        /// </summary>
        public string ServiceTypeCode { get; set; }

        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        public string MedicalRecordCode { get; set; }

        /// <summary>
        /// Mã người bệnh
        /// </summary>
        public int? ClientId { get; set; }

        /// <summary>
        /// Tên bệnh nhân
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Phòng khám
        /// </summary>
        public string RoomExaminationName { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Tên học vị + tên bác sĩ
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// Tên loại vaccine
        /// </summary>
        public string VaccineTypeName { get; set; }

        /// <summary>
        /// Mã của user
        /// </summary>
        public int? UserId { get; set; }


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
                        {
                            //if (ServiceTypeCode == "CN")
                            //    return "Chưa chích";
                            return "Lưu nháp";
                        }
                    case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                        return "Chờ xác nhận";
                    case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                        {
                            //if (ServiceTypeCode == "CN")
                            //    return "Chờ xác nhận đợt chích tiếp theo";
                            return "Chờ xác nhận tái khám";
                        }
                    case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                        return "Đã hủy";
                    case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                        return "Đã xác nhận thanh toán";
                    case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                        {
                            //if (ServiceTypeCode == "CN")
                            //    return "Đã xác nhận thanh toán đợt chích tiếp theo";
                            return "Đã xác nhận thanh toán tái khám";
                        }
                    case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                        {
                            //if (ServiceTypeCode == "CN")
                            //    return "Đã chích";
                            return "Hoàn thành";
                        }
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentFailed:
                        return "Thanh toán thất bại";
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed:
                        {
                            //if (ServiceTypeCode == "CN")
                            //    return "Thanh toán đợt chích tiếp theo thất bại";
                            return "Thanh toán tái khám thất bại";
                        }
                    case (int)CatalogueUtilities.ExaminationStatus.WaitRefund:
                        return "Chờ hoàn tiền";
                    case (int)CatalogueUtilities.ExaminationStatus.RefundSuccess:
                        return "Đã hoàn tiền";
                    case (int)CatalogueUtilities.ExaminationStatus.RefundFailed:
                        return "Hoàn tiền thất bại";
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
                    case (int)CatalogueUtilities.ExaminationAction.Return:
                        return "Trả lại phiếu";
                    case (int)CatalogueUtilities.ExaminationAction.ReturnReExamination:
                        return "Trả lại phiếu tái khám";
                    case (int)CatalogueUtilities.ExaminationAction.Refund:
                        return "Hoàn tiền";
                    case (int)CatalogueUtilities.ExaminationAction.Delete:
                        return "Xóa phiếu";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Từ giờ
        /// </summary>
        public int FromTimeExamination { get; set; }
        public string FromTimeExaminationText { get; set; }

        /// <summary>
        /// Đến giờ
        /// </summary>
        public int ToTimeExamination { get; set; }
        public string ToTimeExaminationText { get; set; }

        #endregion

    }
}
