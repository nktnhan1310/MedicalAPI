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
        /// Bác sĩ
        /// </summary>
        [Required]
        public new int HospitalId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }

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

        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        public string DoctorName { get; set; }

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
