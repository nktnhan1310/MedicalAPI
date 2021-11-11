using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Hồ sơ bệnh án
    /// </summary>
    public class MedicalRecordDetails : MedicalAppDomainHospital
    {
        /// <summary>
        /// Mã hồ sơ khám bệnh
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Mã hồ sơ bệnh án
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mã lịch hẹn
        /// </summary>
        public int? ExaminationFormId { get; set; }

        /// <summary>
        /// Thông tin giờ khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Khung giờ tái khám
        /// </summary>
        public int? ReExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Mã phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }

        /// <summary>
        /// Mã phòng tái khám (nếu có)
        /// </summary>
        public int? ReRoomExaminationId { get; set; }

        /// <summary>
        /// Giá khám
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime? ExaminationDate { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }

        /// <summary>
        /// Cờ check tái khám
        /// </summary>
        public bool IsReExamination { get; set; }

        /// <summary>
        /// Chuyên khoa khám
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Dịch vụ khám
        /// </summary>
        public int? ServiceTypeId { get; set; }

        /// <summary>
        /// Link file qrcode thông tin
        /// </summary>
        public string QrCodeUrlFile { get; set; }

        /// <summary>
        /// Link barcode thông tin hồ sơ bệnh án
        /// </summary>
        public string BarCodeUrl { get; set; }

        /// <summary>
        /// Cờ check hiển thị toa thuốc
        /// </summary>
        [DefaultValue(false)]
        public bool HasMedicalBills { get; set; }

        /// <summary>
        /// Chuẩn đoán của bác sĩ
        /// </summary>
        public string DoctorComment { get; set; }

        /// <summary>
        /// Tên bệnh
        /// </summary>
        public string DiagnoticSickName { get; set; }

        /// <summary>
        /// Tên danh sách chuẩn đoán
        /// </summary>
        public int? DiagnoticTypeId { get; set; }

        /// <summary>
        /// STT khám bệnh
        /// </summary>
        public string ExaminationIndex { get; set; }

        /// <summary>
        /// STT chờ thanh toán
        /// </summary>
        public string ExaminationPaymentIndex { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Loại khám
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// ID bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Bác sĩ khi tái khám
        /// </summary>
        public int? ReExaminationDoctorId { get; set; }

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
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Loại vaccine
        /// </summary>
        public int? VaccineTypeId { get; set; }

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
        /// Từ giờ hiển thị
        /// </summary>
        public string ReFromTimeExaminationText { get; set; }
       
        /// <summary>
        /// Đến giờ hiển thị
        /// </summary>
        public string ReToTimeExaminationText { get; set; }


        #region Extension Properties

        /// <summary>
        /// Tên loại chuẩn đoán
        /// </summary>
        [NotMapped]
        public string DiagnoticTypeName { get; set; }

        /// <summary>
        /// Mã hồ sơ khám
        /// </summary>
        [NotMapped]
        public string MedicalRecordCode { get; set; }

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
        /// Website bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalWebSite { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Tên dịch vụ khám
        /// </summary>
        [NotMapped]
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// File toa thuốc/xét nghiệm/siêu âm/....
        /// </summary>
        [NotMapped]
        public IList<UserFiles> UserFiles { get; set; }

        /// <summary>
        /// Toa thuốc
        /// </summary>
        [NotMapped]
        public IList<MedicalBills> MedicalBills { get; set; }

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
        /// Giá trị cấu hình giờ khám
        /// </summary>
        [NotMapped]
        public string ConfigTimeValue
        {
            get
            {
                return string.Format("{0} - {1}", ExaminationScheduleDetailFromTimeText, ExaminationScheduleDetailToTimeText);
            }
        }

        /// <summary>
        /// Phòng khám
        /// </summary>
        [NotMapped]
        public string RoomName { get; set; }

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        [NotMapped]
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Tên đầy đủ của bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorName { get; set; }

        /// <summary>
        /// Học hàm học vị của bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorDegreeTypeName { get; set; }

        /// <summary>
        /// Tên hiển thị của bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorDisplayName { get; set; }

        /// <summary>
        /// Tên loại vaccine
        /// </summary>
        [NotMapped]
        public string VaccineTypeName { get; set; }

        #endregion

    }
}
