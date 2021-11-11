using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities.Extensions
{
    /// <summary>
    /// Cập nhật trạng thái phiếu khám bệnh (lịch hẹn)
    /// </summary>
    public class UpdateExaminationStatus
    {
        /// <summary>
        /// Mã phiếu
        /// </summary>
        public int ExaminationFormId { get; set; }

        /// <summary>
        /// Mã thanh toán
        /// </summary>
        public int? PaymentMethodId { get; set; }

        /// <summary>
        /// Mã ngân hàng
        /// </summary>
        public int? BankInfoId { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime? ReExaminationDate { get; set; }

        /// <summary>
        /// Ca khám
        /// </summary>
        public int? ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Chọn lại phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int? DoctorId { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public int? VaccineTypeId { get; set; }

        /// <summary>
        /// Chuẩn đoán của bác sĩ
        /// </summary>
        public string DoctorComment { get; set; }

        /// <summary>
        /// Loại chuẩn đoán
        /// </summary>
        public int? DiagnoticTypeId { get; set; }

        /// <summary>
        /// Tên bệnh
        /// </summary>
        public string DiagnoticSickName { get; set; }

        /// <summary>
        /// Nhập comment xác nhận/hủy
        /// </summary>
        [StringLength(1000)]
        public string Comment { get; set; }

        /// <summary>
        /// Mã phiếu đổi
        /// </summary>
        public int? ExaminationFormChangedId { get; set; }

        /// <summary>
        /// Người thao tác
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Mã toa thuốc
        /// </summary>
        public string PrescriptionCode { get; set; }

        /// <summary>
        /// Toa thuốc (list chữ)
        /// </summary>
        public string Prescription { get; set; }

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
        /// List file toa thuốc/xét nghiệm/ siêu âm...
        /// </summary>
        public IList<UserFiles> MedicalRecordDetailFiles { get; set; }

        [DefaultValue(false)]
        public bool HasMedicalBill { get; set; }

        ///// <summary>
        ///// Toa thuốc
        ///// </summary>
        //public MedicalBills MedicalBills { get; set; }
    }
}
