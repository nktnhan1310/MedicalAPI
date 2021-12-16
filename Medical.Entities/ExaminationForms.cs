using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Phiếu khám bệnh
    /// </summary>
    [Table("ExaminationForm")]
    public class ExaminationForms : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã phiếu khám bệnh
        /// </summary>
        [StringLength(50)]
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
        /// Ca khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Trạng thái phiếu (Chờ xác nhận lịch hẹn/Đã xác nhận lịch hẹn/Chờ xác nhận hủy/Đã hủy/Chờ xác nhận tái khám/Đã xác nhận tái khám)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Chi phí khám
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Loại vaccine
        /// </summary>
        public int? VaccineTypeId { get; set; }



        /// <summary>
        /// Số thứ tự khám bệnh
        /// </summary>
        public string ExaminationIndex { get; set; }

        /// <summary>
        /// Số thứ tự đóng tiền khám bệnh nếu thanh toán qua app
        /// </summary>
        public string ExaminationPaymentIndex { get; set; }

        /// <summary>
        /// Mô tả lịch hẹn
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Huyết áp
        /// </summary>
        public string BloodPressure { get; set; }

        /// <summary>
        /// Nhịp tim
        /// </summary>
        public string HeartBeat { get; set; }

        /// <summary>
        /// Đường huyết
        /// </summary>
        public string BloodSugar { get; set; }

        /// <summary>
        /// Loại BHYT
        /// </summary>
        public int? BHYTType { get; set; }

        /// <summary>
        /// Cờ check có tái khám hay ko
        /// </summary>
        public bool IsReExamination { get; set; }

        /// <summary>
        /// Từ giờ
        /// </summary>
        public int FromTimeExamination { get; set; }

        /// <summary>
        /// Từ giờ hiển thị
        /// </summary>
        public string FromTimeExaminationText { get; set; }

        /// <summary>
        /// Đến giờ
        /// </summary>
        public int ToTimeExamination { get; set; }

        /// <summary>
        /// Đến giờ hiển thị
        /// </summary>
        public string ToTimeExaminationText { get; set; }

        /// <summary>
        /// Số thứ tự khám theo từng khung giờ
        /// </summary>
        public int? SystemIndex { get; set; }

        #region Extension Properties

        /// <summary>
        /// Số lần phải tiêm của loại vaccine
        /// </summary>
        [NotMapped]
        public int? NumberOfDoses { get; set; }

        /// <summary>
        /// Địa chỉ bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalAddress { get; set; }

        /// <summary>
        /// Số điện thoại bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalPhone { get; set; }

        /// <summary>
        /// Link Url Website của bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalWebSite { get; set; }

        /// <summary>
        /// Phí khám bệnh nếu có
        /// </summary>
        [NotMapped]
        public double? FeeExamination { get; set; }

        /// <summary>
        /// Thông tin chi nhánh ngân hàng thanh toán
        /// </summary>
        [NotMapped]
        public int? BankInfoId { get; set; }

        /// <summary>
        /// Comment khi duyệt phiếu khám
        /// </summary>
        [NotMapped]
        public string Comment { get; set; }

        ///// <summary>
        ///// Tên dịch vụ khám
        ///// </summary>
        //[NotMapped]
        //public string ServiceTypeName { get; set; }

        ///// <summary>
        ///// Mã dịch vụ khám
        ///// </summary>
        //[NotMapped]
        //public string ServiceTypeCode { get; set; }

        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        [NotMapped]
        public string MedicalRecordCode { get; set; }
        
        /// <summary>
        /// Mã người bệnh
        /// </summary>
        [NotMapped]
        public int? ClientId { get; set; }

        /// <summary>
        /// Tên bệnh nhân
        /// </summary>
        [NotMapped]
        public string ClientName { get; set; }

        /// <summary>
        /// Phòng khám
        /// </summary>
        [NotMapped]
        public string RoomExaminationName { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Tên học vị + tên bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorDisplayName { get; set; }

        /// <summary>
        /// Tên loại vaccine
        /// </summary>
        [NotMapped]
        public string VaccineTypeName { get; set; }

        /// <summary>
        /// Ngày tiêm vaccine tiếp theo
        /// </summary>
        [NotMapped]
        public DateTime? NextInjectionDate { get; set; }

        /// <summary>
        /// Từ giờ
        /// </summary>
        [NotMapped]
        public string ExaminationScheduleDetailFromTimeText { get; set; }
        /// <summary>
        /// Đến giờ
        /// </summary>
        [NotMapped]
        public string ExaminationScheduleDetailToTimeText { get; set; }

        /// <summary>
        /// Khung thời gian khám bệnh
        /// </summary>
        [NotMapped]
        public string ConfigTimeExaminationValue
        {
            get
            {
                return string.Format("{0} - {1}", ExaminationScheduleDetailFromTimeText, ExaminationScheduleDetailToTimeText);
            }
        }

        /// <summary>
        /// Lịch sử tạo phiếu khám bệnh (lịch hẹn)
        /// </summary>
        [NotMapped]
        public IList<ExaminationHistories> ExaminationHistories { get; set; }

        /// <summary>
        /// Lịch sử thanh toán
        /// </summary>
        [NotMapped]
        public IList<PaymentHistories> PaymentHistories { get; set; }

        /// <summary>
        /// Dịch vụ phát sinh
        /// </summary>
        [NotMapped]
        public IList<ExaminationFormAdditionServiceMappings> ExaminationFormAdditionServiceMappings { get; set; }

        /// <summary>
        /// Danh sách dịch vụ chi tiết dịch vụ phát sinh
        /// </summary>
        [NotMapped]
        public IList<ExaminationFormAdditionServiceDetailMappings> ExaminationFormAdditionServiceDetailMappings { get; set; }

        /// <summary>
        /// Dịch vụ phát sinh (nếu có)
        /// </summary>
        [NotMapped]
        public List<int> AdditionServiceIds { get; set; }

        /// <summary>
        /// Danh sách mã dịch vụ phát sinh chi tiết (nếu có)
        /// </summary>
        [NotMapped]
        public List<int> AdditionServiceDetailIds { get; set; }

        #endregion


    }
}
