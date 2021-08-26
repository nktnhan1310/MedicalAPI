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
        public IList<ConfigTimeExaminationDayOfWeekModel> ConfigTimeExaminationDayOfWeeks { get; set; }

        public IList<DayOfWeekDisplayModel> DayOfWeekDisplays
        {
            get
            {
                if(ConfigTimeExaminationDayOfWeeks != null && ConfigTimeExaminationDayOfWeeks.Any())
                {
                    IList<DayOfWeekDisplayModel> dayOfWeekDisplays = new List<DayOfWeekDisplayModel>(); 
                    dayOfWeekDisplays = ConfigTimeExaminationDayOfWeeks
                        .GroupBy(e => e.SessionTypeName)
                        .Select(e => new DayOfWeekDisplayModel()
                        {
                            SessionTypeName = e.FirstOrDefault().SessionTypeName,
                            DayOfWeekName = string.Join(", ", e.Select(x => x.DayOfWeekName).Distinct().ToList())
                        }).ToList();
                    return dayOfWeekDisplays;
                }
                return null;
            }
        }

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
        /// Danh sách ca làm việc
        /// </summary>
        public IList<ExaminationScheduleDetailModel> ExaminationScheduleDetails { get; set; }

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
        /// <summary>
        /// Mã ca khám
        /// </summary>
        public int ConfigTimeExaminationId { get; set; }

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
    }
}
