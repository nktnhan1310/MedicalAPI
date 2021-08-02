﻿using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Chuyên khoa của bác sĩ
    /// </summary>
    [Table("DoctorDetails")]
    public class DoctorDetails : MedicalAppDomain
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
        [NotMapped]
        public string DegreeTypeName { get; set; }

        /// <summary>
        /// Tên đầy đủ của bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorName { get; set; }

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
        /// Tên chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

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
                                int roomExaminationTryParseId = 0;
                                int? roomExaminationId = null;
                                DateTime examinationDate;
                                if (!DateTime.TryParse(configTimeExaminationProperties[1], out examinationDate))
                                    continue;
                                int.TryParse(configTimeExaminationProperties[0], out configTimeExaminationId);
                                if (int.TryParse(configTimeExaminationProperties[6], out roomExaminationTryParseId))
                                    roomExaminationId = roomExaminationTryParseId;
                                results.Add(new ConfigTimeExaminationDayOfWeek()
                                {
                                    ConfigTimeExaminationId = configTimeExaminationId,
                                    ExaminationDate = examinationDate,
                                    SessionTypeName = configTimeExaminationProperties[2],
                                    DayOfWeekName = configTimeExaminationProperties[3],
                                    ConfigTimeExaminationValue = configTimeExaminationProperties[4],
                                    RoomName = configTimeExaminationProperties[5],
                                    RoomExaminationId = roomExaminationId
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
        /// Lấy ra danh sách ngày trực của bác sĩ
        /// </summary>
        public IList<DateTime> DateExaminations
        {
            get
            {
                if (ConfigTimeExaminationDayOfWeeks != null && ConfigTimeExaminationDayOfWeeks.Any())
                {
                    return ConfigTimeExaminationDayOfWeeks.Select(e => e.ExaminationDate).Distinct().ToList();
                }
                return null;
            }
        }

        #endregion


    }
}
