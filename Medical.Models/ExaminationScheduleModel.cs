using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Lịch khám theo từng bác sĩ
    /// </summary>
    public class ExaminationScheduleModel : MedicalAppDomainHospitalModel
    {
        /// <summary>
        /// Bác sĩ
        /// </summary>
        [Required]
        public int DoctorId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        [Required]
        public DateTime ExaminationDate { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        [Required]
        public int SpecialistTypeId { get; set; }

        /// <summary>
        /// Số ca khám tối đa trong buổi sáng
        /// </summary>
        public int? MaximumMorningExamination { get; set; }
        /// <summary>
        /// Số ca khám tối đa trong buổi chiều
        /// </summary>
        public int? MaximumAfternoonExamination { get; set; }
        /// <summary>
        /// Số ca khám tối đa trong buổi khác
        /// </summary>
        public int? MaximumOtherExamination { get; set; }

        /// <summary>
        /// Mã bác sĩ thay thế
        /// </summary>
        public int? ReplaceDoctorId { get; set; }

        /// <summary>
        /// Cờ check sử dụng cấu hình số phút khám của bệnh viện
        /// </summary>
        public bool IsUseHospitalConfig { get; set; }

        #region Extension Properties


        /// <summary>
        /// Học vị bác sĩ
        /// </summary>
        public string DegreeTypeName { get; set; }

        /// <summary>
        /// Giới tính bác sĩ
        /// </summary>
        public string DoctorGender { get; set; }

        /// <summary>
        /// Giới tính bác sĩ
        /// </summary>
        public string DoctorGenderName { get; set; }

        /// <summary>
        /// Giá khám
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Giá hiển thị
        /// </summary>
        public string PriceDisplay
        {
            get
            {
                return Price.HasValue ? Price.Value.ToString("#,###") : string.Empty;
            }
        }

        /// <summary>
        /// Danh sách lịch khám theo bác sĩ và chuyên khoa
        /// </summary>
        //public IList<ConfigTimeExaminationDayOfWeekModel> ConfigTimeExaminationDayOfWeeks { get; set; }

        //public IList<DayOfWeekDisplayModel> DayOfWeekDisplays
        //{
        //    get
        //    {
        //        if(ConfigTimeExaminationDayOfWeeks != null && ConfigTimeExaminationDayOfWeeks.Any())
        //        {
        //            IList<DayOfWeekDisplayModel> dayOfWeekDisplays = new List<DayOfWeekDisplayModel>(); 
        //            dayOfWeekDisplays = ConfigTimeExaminationDayOfWeeks
        //                .GroupBy(e => e.SessionTypeName)
        //                .Select(e => new DayOfWeekDisplayModel()
        //                {
        //                    SessionTypeName = e.FirstOrDefault().SessionTypeName,
        //                    DayOfWeekName = string.Join(", ", e.Select(x => x.DayOfWeekName).Distinct().ToList())
        //                }).ToList();
        //            return dayOfWeekDisplays;
        //        }
        //        return null;
        //    }
        //}

        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        public string DoctorCode { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Cờ check tối đa
        /// </summary>
        public bool IsMaximumMorning { get; set; }

        /// <summary>
        /// Cờ check tối đa
        /// </summary>
        public bool IsMaximumAfternoon { get; set; }

        /// <summary>
        /// Cờ check tối đa
        /// </summary>
        public bool IsMaximumOther { get; set; }

        /// <summary>
        /// Tên bác sĩ thay thế
        /// </summary>
        public string ReplaceDoctorName { get; set; }

        /// <summary>
        /// Giới tính bác sĩ thay thế
        /// </summary>
        public string ReplaceDoctorGenderName { get; set; }

        /// <summary>
        /// Học vị bác sĩ thay thế
        /// </summary>
        public string DegreeTypeReplaceDoctorName { get; set; }


        /// <summary>
        /// Danh sách ca làm việc
        /// </summary>
        public IList<ExaminationScheduleDetailModel> ExaminationScheduleDetails { get; set; }

        /// <summary>
        /// Danh sách lịch ca khám theo buổi
        /// </summary>
        public IList<ConfigTimeExaminationBySessionModel> ConfigTimeExaminationBySessions { get; set; }

        /// <summary>
        /// Danh sách ca khám theo buổi (dành cho khám thường)
        /// </summary>
        public IList<ConfigTimeExaminationBySessionModel> ConfigTimeExaminationByNormalSession { get; set; }

        /// <summary>
        /// Danh sách y tá/điều dưỡng của ca trực
        /// </summary>
        //public IList<ExaminationScheduleMappingUserModel> ExaminationScheduleMappingUsers { get; set; }

        /// <summary>
        /// Danh sách id của y tá/điều dưỡng
        /// </summary>
        public List<int> NurseIds { get; set; }

        #endregion
    }

    public class DayOfWeekDisplayModel
    {
        /// <summary>
        /// Tên buổi
        /// </summary>
        public string SessionTypeName { get; set; }
        /// <summary>
        /// Tên ngày
        /// </summary>
        public string DayOfWeekName { get; set; }
    }

    /// <summary>
    /// Danh sách lịch khám theo chuyên khoa
    /// </summary>
    public class ConfigTimeExaminationDayOfWeekModel
    {
        public int ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Mã ca khám
        /// </summary>
        //public int ConfigTimeExaminationId { get; set; }

        /// <summary>
        /// Mốc thời gian làm việc
        /// </summary>
        public string ConfigTimeExaminationValue { get; set; }

        /// <summary>
        /// Id phòng khám
        /// </summary>
        public int? RoomExaminationId { get; set; }

        /// <summary>
        /// Tên phòng
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }

        /// <summary>
        /// Tên buổi khám
        /// </summary>
        public string SessionTypeName { get; set; }

        /// <summary>
        /// Ngày khám trong tuần
        /// </summary>
        public string DayOfWeekName { get; set; }
        /// <summary>
        /// Mã buổi khám
        /// </summary>
        public string SessionTypeCode { get; set; }

        /// <summary>
        /// Tổng số ca khám tối đa theo giờ
        /// </summary>
        public int? MaximumExamination { get; set; }

        /// <summary>
        /// Mã bác sĩ thay thế
        /// </summary>
        public int? ReplaceDoctorId { get; set; }

        /// <summary>
        /// Tên đầy đủ của bác sĩ thay thế
        /// </summary>
        public string ReplaceDoctorFullName { get; set; }

        /// <summary>
        /// Từ giờ
        /// </summary>
        public int? ExaminationFromTime { get; set; }
        /// <summary>
        /// Đến giờ
        /// </summary>
        public int? ExaminationToTime { get; set; }
        /// <summary>
        /// Cờ check tối đa
        /// </summary>
        public bool IsMaximum { get; set; }
    }

    /// <summary>
    /// Danh sách buổi khám theo ngày chọn
    /// </summary>
    public class ConfigTimeExaminationBySessionModel
    {
        /// <summary>
        /// Tên buổi
        /// </summary>
        public string SessionTypeName { get; set; }
        /// <summary>
        /// Mã buổi
        /// </summary>
        public string SessionTypeCode { get; set; }
        /// <summary>
        /// Mã phòng
        /// </summary>
        public int? RoomExaminationId { get; set; }
        /// <summary>
        /// Tên phòng
        /// </summary>
        public string RoomExaminationName { get; set; }
        public IList<ConfigTimeExaminationDayOfWeekModel> ConfigTimeExaminationDayOfWeeks { get; set; }
    }

}
