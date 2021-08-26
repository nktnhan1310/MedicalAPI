using Medical.Models.DomainModel;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Phiếu khám bệnh
    /// </summary>
    public class ExaminationFormModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Mã phiếu khám bệnh
        /// </summary>
        [StringLength(50, ErrorMessage = "Mã phiếu khám bệnh phải nhỏ hơn 50 kí tự!")]
        public string Code { get; set; }
        /// <summary>
        /// Hồ sơ khám bệnh
        /// </summary>
        [Required]
        public int RecordId { get; set; }
        /// <summary>
        /// Loại khám (theo ngày/theo bác sĩ)
        /// </summary>
        public int? TypeId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }

        /// <summary>
        /// Mã người khám
        /// </summary>
        public int? ClientId { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }
        /// <summary>
        /// Có BHYT?
        /// </summary>
        public bool IsBHYT { get; set; }
        /// <summary>
        /// phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }
        /// <summary>
        /// (Tái khám/Dịch vụ/...)
        /// </summary>
        public int ServiceTypeId { get; set; }
        /// <summary>
        /// Ca khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Trạng thái phiếu (Chờ xác nhận lịch hẹn/Đã xác nhận lịch hẹn/Chờ xác nhận hủy/Đã hủy/Chờ xác nhận tái khám/Đã xác nhận tái khám)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Chi phí khám
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Số thứ tự khám bệnh
        /// </summary>
        public string ExaminationIndex { get; set; }

        /// <summary>
        /// Mô tả lịch hẹn
        /// </summary>
        public string Note { get; set; }

        #region Extension Properties

        /// <summary>
        /// Phí khám bệnh nếu có
        /// </summary>
        public double? FeeExamination { get; set; }

        /// <summary>
        /// Thông tin chi nhánh ngân hàng thanh toán
        /// </summary>
        public int? BankInfoId { get; set; }

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
                        return "Đã xác nhận thanh toán";
                    case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                        return "Đã xác nhận thanh toán tái khám";
                    case (int)CatalogueUtilities.ExaminationStatus.FinishExamination:
                        return "Hoàn thành";
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentFailed:
                        return "Thanh toán thất bại";
                    case (int)CatalogueUtilities.ExaminationStatus.PaymentReExaminationFailed:
                        return "Thanh toán tái khám thất bại";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Comment khi duyệt phiếu khám
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Địa chỉ bệnh viện
        /// </summary>
        public string HospitalAddress { get; set; }

        /// <summary>
        /// Tên dịch vụ khám
        /// </summary>
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        public string MedicalRecordCode { get; set; }

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
        public string DoctorDisplayName { get; set; }

        /// <summary>
        /// Lịch sử tạo phiếu khám bệnh (lịch hẹn)
        /// </summary>
        public IList<ExaminationHistoryModel> ExaminationHistories { get; set; }

        /// <summary>
        /// Lịch sử thanh toán
        /// </summary>
        public IList<PaymentHistoryModel> PaymentHistories { get; set; }

        /// <summary>
        /// Chi tiết dịch vụ phát sinh
        /// </summary>
        public IList<ExaminationFormDetailModel> ExaminationFormDetails { get; set; }

        public ExaminationScheduleDetailModel ExaminationScheduleDetail { get; set; }

        public IList<HospitalFileModel> HospitalFiles { get; set; }


        #endregion
    }
}
