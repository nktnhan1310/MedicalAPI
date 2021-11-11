using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch sử của lịch hẹn
    /// </summary>
    public class ExaminationHistories : MedicalAppDomain
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
        /// Cờ check chỉnh sửa hủy => trạng thái khác
        /// </summary>
        [DefaultValue(false)]
        public bool IsEdit { get; set; }

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
        /// Tên bác sĩ
        /// </summary>
        [NotMapped]
        public string HospitalName { get; set; }

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
        /// Tên dịch vụ khám
        /// </summary>
        [NotMapped]
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Mã dịch vụ khám
        /// </summary>
        [NotMapped]
        public string ServiceTypeCode { get; set; }

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
        public string DoctorName { get; set; }

        /// <summary>
        /// Tên loại vaccine
        /// </summary>
        [NotMapped]
        public string VaccineTypeName { get; set; }

        /// <summary>
        /// Mã của user
        /// </summary>
        [NotMapped]
        public int? UserId { get; set; }

        /// <summary>
        /// Từ giờ
        /// </summary>
        [NotMapped]
        public int FromTimeExamination { get; set; }
        [NotMapped]
        public string FromTimeExaminationText { get; set; }

        /// <summary>
        /// Đến giờ
        /// </summary>
        [NotMapped]
        public int ToTimeExamination { get; set; }
        [NotMapped]
        public string ToTimeExaminationText { get; set; }

        #endregion

    }
}
