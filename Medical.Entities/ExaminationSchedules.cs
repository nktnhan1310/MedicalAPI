using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DefaultValue(false)]
        public bool IsUseHospitalConfig { get; set; }

        /// <summary>
        /// Guid import lịch
        /// </summary>
        public Guid? ImportScheduleId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Học vị bác sĩ
        /// </summary>
        [NotMapped]
        public string DegreeTypeName { get; set; }

        /// <summary>
        /// Học vị của bác sĩ thay thế
        /// </summary>
        [NotMapped]
        public string DegreeTypeReplaceDoctorName { get; set; }

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

        /// <summary>
        /// Giới tính bác sĩ thay thế
        /// </summary>
        [NotMapped]
        public int ReplaceDoctorGender { get; set; }

        [NotMapped]
        public string ReplaceDoctorGenderName
        {
            get
            {
                return ReplaceDoctorGender == 0 ? "Nam" : "Nữ";
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
        public IList<ConfigTimeExaminationDayOfWeek> ConfigTimeExaminationDayOfWeekValue
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
                                int examinationScheduleDetailId = 0;
                                int roomExaminationTryParseId = 0;
                                int maximumExaminationTryParseId = 0;
                                int replaceDoctorTryParseId = 0;
                                int fromTime = 0;
                                int toTime = 0;
                                int? replaceDoctorId = null;
                                int? maximumExamination = null;
                                int? roomExaminationId = null;
                                DateTime examinationDate;
                                if (!DateTime.TryParse(configTimeExaminationProperties[1], out examinationDate))
                                    continue;
                                int.TryParse(configTimeExaminationProperties[0], out examinationScheduleDetailId);
                                if (int.TryParse(configTimeExaminationProperties[5], out roomExaminationTryParseId))
                                    roomExaminationId = roomExaminationTryParseId;
                                if (int.TryParse(configTimeExaminationProperties[7], out maximumExaminationTryParseId))
                                    maximumExamination = maximumExaminationTryParseId;

                                if (int.TryParse(configTimeExaminationProperties[8], out replaceDoctorTryParseId))
                                    replaceDoctorId = replaceDoctorTryParseId;
                                int.TryParse(configTimeExaminationProperties[12], out fromTime);
                                int.TryParse(configTimeExaminationProperties[13], out toTime);
                                results.Add(new ConfigTimeExaminationDayOfWeek()
                                {
                                    ExaminationScheduleDetailId = examinationScheduleDetailId,
                                    ExaminationDate = examinationDate,
                                    SessionTypeName = configTimeExaminationProperties[2],
                                    DayOfWeekName = configTimeExaminationProperties[3],
                                    RoomName = configTimeExaminationProperties[4],
                                    RoomExaminationId = roomExaminationId,
                                    SessionTypeCode = configTimeExaminationProperties[6],
                                    MaximumExamination = maximumExamination,
                                    ReplaceDoctorId = replaceDoctorId,
                                    ReplaceDoctorFullName = configTimeExaminationProperties[9],
                                    ExaminationFromTimeText = configTimeExaminationProperties[10],
                                    ExaminationToTimeText = configTimeExaminationProperties[11],
                                    ExaminationFromTime = fromTime,
                                    ExaminationToTime = toTime
                                });
                            }

                        }
                        return results;
                    }
                }
                return null;
            }
        }

        [NotMapped]
        public IList<ConfigTimeExaminationDayOfWeek> ConfigTimeExaminationDayOfWeeks { get; set; }

        /// <summary>
        /// Danh sách lịch ca khám theo buổi
        /// </summary>
        [NotMapped]
        public IList<ConfigTimeExaminationBySession> ConfigTimeExaminationBySessions
        {
            get
            {
                if (ConfigTimeExaminationDayOfWeeks != null && ConfigTimeExaminationDayOfWeeks.Any())
                {
                    IList<ConfigTimeExaminationBySession> configTimeExaminationBySessions = new List<ConfigTimeExaminationBySession>();
                    configTimeExaminationBySessions = ConfigTimeExaminationDayOfWeeks.GroupBy(e => new
                    {
                        e.SessionTypeCode,
                        e.SessionTypeName,
                        e.RoomExaminationId,
                        e.RoomName
                    }).Select(e => new ConfigTimeExaminationBySession()
                    {
                        SessionTypeCode = e.Key.SessionTypeCode,
                        SessionTypeName = e.Key.SessionTypeName,
                        RoomExaminationId = e.Key.RoomExaminationId,
                        RoomExaminationName = e.Key.RoomName,
                        ConfigTimeExaminationDayOfWeeks = e.ToList()
                    }).ToList();
                    return configTimeExaminationBySessions;
                }
                return null;
            }
        }

        /// <summary>
        /// Thời gian khám theo lịch thường
        /// </summary>
        [NotMapped]
        public IList<ConfigTimeExaminationBySession> ConfigTimeExaminationByNormalSession
        {
            get
            {
                if (ConfigTimeExaminationDayOfWeeks != null && ConfigTimeExaminationDayOfWeeks.Any())
                {
                    IList<ConfigTimeExaminationBySession> configTimeExaminationBySessions = new List<ConfigTimeExaminationBySession>();
                    configTimeExaminationBySessions = ConfigTimeExaminationDayOfWeeks.GroupBy(e => new
                    {
                        e.SessionTypeCode,
                        e.SessionTypeName,
                    }).Select(e => new ConfigTimeExaminationBySession()
                    {
                        SessionTypeCode = e.Key.SessionTypeCode,
                        SessionTypeName = e.Key.SessionTypeName,
                        //RoomExaminationId = e.Key.RoomExaminationId,
                        //RoomExaminationName = e.Key.RoomName,
                        ConfigTimeExaminationDayOfWeeks = e.ToList()
                    }).ToList();
                    return configTimeExaminationBySessions;
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
        /// Tên bác sĩ thay thế
        /// </summary>
        [NotMapped]
        public string ReplaceDoctorName { get; set; }

        /// <summary>
        /// Cờ check tối đa
        /// </summary>
        [NotMapped]
        public bool IsMaximumMorning { get; set; }

        /// <summary>
        /// Cờ check tối đa
        /// </summary>
        [NotMapped]
        public bool IsMaximumAfternoon { get; set; }

        /// <summary>
        /// Cờ check tối đa
        /// </summary>
        [NotMapped]
        public bool IsMaximumOther { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorCode { get; set; }

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

        /// <summary>
        /// Danh sách y tá/điều dưỡng của lịch trực
        /// </summary>
        [NotMapped]
        public IList<ExaminationScheduleMappingUsers> ExaminationScheduleMappingUsers { get; set; }

        /// <summary>
        /// Danh sách id của y tá/điều dưỡng
        /// </summary>
        [NotMapped]
        public List<int> NurseIds { get; set; }

        #endregion
    }

    /// <summary>
    /// Danh sách lịch khám theo chuyên khoa
    /// </summary>
    public class ConfigTimeExaminationDayOfWeek
    {
        public int? ExaminationScheduleDetailId { get; set; }

        /// <summary>
        /// Mốc thời gian làm việc
        /// </summary>
        public string ConfigTimeExaminationValue
        {
            get
            {
                return string.Format("{0} - {1}", ExaminationFromTimeText, ExaminationToTimeText);
            }
        }

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
        /// Từ giờ text
        /// </summary>
        public string ExaminationFromTimeText { get; set; }
        /// <summary>
        /// Đến giờ text
        /// </summary>
        public string ExaminationToTimeText { get; set; }

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

    public class ConfigTimeExaminationBySession
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

        public IList<ConfigTimeExaminationDayOfWeek> ConfigTimeExaminationDayOfWeeks { get; set; }
    }
}
