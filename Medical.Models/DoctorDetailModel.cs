using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Chi tiết chuyên khoa từng bác sĩ
    /// </summary>
    public class DoctorDetailModel: MedicalAppDomainModel
    {
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public int SpecialistTypeId { get; set; }
        /// <summary>
        /// Chi phí khám theo bác sĩ
        /// </summary>
        public double? Price { get; set; }

        #region Extesion Properties

        /// <summary>
        /// Học vị bác sĩ
        /// </summary>
        public string DegreeTypeName { get; set; }

        /// <summary>
        /// Tên đầy đủ của bác sĩ
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// Giới tính bác sĩ
        /// </summary>
        public int DoctorGender { get; set; }

        public string DoctorGenderName
        {
            get
            {
                return DoctorGender == 0 ? "Nam" : "Nữ";
            }
        }

        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Danh sách lịch khám theo bác sĩ và chuyên khoa
        /// </summary>
        public IList<ConfigTimeExaminationDayOfWeekModel> ConfigTimeExaminationDayOfWeeks { get; set; }

        /// <summary>
        /// Lấy ra ca trực bác sĩ 
        /// </summary>
        public IList<DayOfWeekDisplayModel> DayOfWeekDisplays
        {
            get
            {
                if (ConfigTimeExaminationDayOfWeeks != null && ConfigTimeExaminationDayOfWeeks.Any())
                {
                    IList<DayOfWeekDisplayModel> dayOfWeekDisplays = new List<DayOfWeekDisplayModel>();
                    dayOfWeekDisplays = ConfigTimeExaminationDayOfWeeks
                        .GroupBy(e => e.SessionTypeName)
                        .Select(e => new DayOfWeekDisplayModel()
                        {
                            SessionTypeName = e.FirstOrDefault().SessionTypeName,
                            DayOfWeekName = string.Join(", ", e.Select(x => x.DayOfWeekName).OrderBy(x => x).Distinct().ToList())
                        }).ToList();
                    return dayOfWeekDisplays;
                }
                return null;
            }
        }

        /// <summary>
        /// Lấy ra danh sách ngày trực của bác sĩ
        /// </summary>
        public IList<DateTime> DateExaminations { get; set; }

        #endregion


    }
}
