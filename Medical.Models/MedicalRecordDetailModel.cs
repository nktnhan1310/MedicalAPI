using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Medical.Models
{
    public class MedicalRecordDetailModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Mã hồ sơ khám bệnh
        /// </summary>
        public int? MedicalRecordId { get; set; }

        /// <summary>
        /// Mã lịch hẹn
        /// </summary>
        public int? ExaminationFormId { get; set; }

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
        /// Chuyên khoa khám
        /// </summary>
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Dịch vụ khám
        /// </summary>
        public int? ServiceType { get; set; }

        /// <summary>
        /// Link file qrcode thông tin
        /// </summary>
        public string QrCodeUrlFile { get; set; }

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
        /// Thông tin giờ khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }

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

        #region Extension Properties

        /// <summary>
        /// Mã hồ sơ khám
        /// </summary>
        public string MedicalRecordCode { get; set; }

        /// <summary>
        /// Địa chỉ bệnh viện
        /// </summary>
        public string HospitalAddress { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Tên dịch vụ khám
        /// </summary>
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// File toa thuốc/xét nghiệm/siêu âm/....
        /// </summary>
        public IList<MedicalRecordDetailFileModel> MedicalRecordDetailFiles { get; set; }

        /// <summary>
        /// Toa thuốc
        /// </summary>
        public IList<MedicalBillModel> MedicalBills { get; set; }

        /// <summary>
        /// Giá trị cấu hình giờ khám
        /// </summary>
        public string ConfigTimeValue { get; set; }

        /// <summary>
        /// Phòng khám
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// Tên phương thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Tên đầy đủ của bác sĩ
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// Học hàm học vị của bác sĩ
        /// </summary>
        public string DoctorDegreeTypeName { get; set; }

        /// <summary>
        /// Tên hiển thị của bác sĩ
        /// </summary>
        public string DoctorDislayName { get; set; }


        #endregion
    }
}
