using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch khám theo từng bác sĩ
    /// </summary>
    [Table("ExaminationSchedules")]
    public class ExaminationSchedules : MedicalAppDomainHospital
    {
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        public int SpecialistTypeId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalName { get; set; }

        /// <summary>
        /// Học vị bác sĩ
        /// </summary>
        [NotMapped]
        public string DegreeTypeName { get; set; }

        /// <summary>
        /// Giới tính bác sĩ
        /// </summary>
        [NotMapped]
        public int DoctorGender { get; set; }

        [NotMapped]
        public string DoctorGenderName
        {
            get
            {
                return DoctorGender == 0 ? "Nam" : "Nữ";
            }
        }

        [NotMapped]
        public double? Price { get; set; }

        /// <summary>
        /// Cấu hình lịch khám theo bác sĩ và chuyên khoa
        /// <para>[ConfigTimeExaminationId]_[SessionTypeName]_[DayOfWeek]</para>
        /// </summary>
        [NotMapped]
        public string ConfigExaminationTimes { get; set; }

        /// <summary>
        /// Danh sách lịch khám theo bác sĩ và chuyên khoa
        /// </summary>
        [NotMapped]
        public IList<ConfigTimeExaminationDayOfWeek> ConfigTimeExaminationDayOfWeeks
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigExaminationTimes))
                {
                    var configTimeExaminations = ConfigExaminationTimes.Split(';').ToArray();
                    if (configTimeExaminations != null && configTimeExaminations.Any())
                    {
                        IList<ConfigTimeExaminationDayOfWeek> results = new List<ConfigTimeExaminationDayOfWeek>();
                        foreach (var configTimeExamination in configTimeExaminations)
                        {
                            var configTimeExaminationProperties = configTimeExamination.Split('_').ToArray();
                            if (configTimeExaminationProperties != null && configTimeExaminationProperties.Any())
                            {
                                int configTimeExaminationId = 0;
                                DateTime examinationDate;
                                if (!DateTime.TryParse(configTimeExaminationProperties[1], out examinationDate))
                                    continue;
                                int.TryParse(configTimeExaminationProperties[0], out configTimeExaminationId);
                                results.Add(new ConfigTimeExaminationDayOfWeek()
                                {
                                    ConfigTimeExaminationId = configTimeExaminationId,
                                    ExaminationDate = examinationDate,
                                    SessionTypeName = configTimeExaminationProperties[2],
                                    DayOfWeekName = configTimeExaminationProperties[3]
                                });
                            }

                        }
                        return results;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorName { get; set; }

        /// <summary>
        /// Chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Danh sách ca làm việc
        /// </summary>
        [NotMapped]
        public IList<ExaminationScheduleDetails> ExaminationScheduleDetails { get; set; }

        #endregion
    }

    /// <summary>
    /// Danh sách lịch khám theo chuyên khoa
    /// </summary>
    public class ConfigTimeExaminationDayOfWeek
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
