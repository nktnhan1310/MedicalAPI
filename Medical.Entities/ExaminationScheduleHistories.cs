using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch sử chỉnh sửa lịch
    /// </summary>
    public class ExaminationScheduleHistories : MedicalAppDomainHospital
    {
        public int? Action { get; set; }

        /// <summary>
        /// Chuỗi json dữ liệu cũ của lịch trực bác sĩ
        /// </summary>
        public string OldDataJson { get; set; }

        /// <summary>
        /// Chuỗi json dữ liệu mới của lịch trực bác sĩ
        /// </summary>
        public string NewDataJson { get; set; }

        #region Extension Properties

        /// <summary>
        /// Dữ liệu cũ của lịch
        /// </summary>
        public ExaminationSchedules OldData
        {
            get
            {
                if (!string.IsNullOrEmpty(OldDataJson))
                    return JsonSerializer.Deserialize<ExaminationSchedules>(OldDataJson);
                return null;
            }
        }

        /// <summary>
        /// Dữ liệu mới của lịch
        /// </summary>
        public ExaminationSchedules NewData
        {
            get
            {
                if (!string.IsNullOrEmpty(NewDataJson))
                    return JsonSerializer.Deserialize<ExaminationSchedules>(NewDataJson);
                return null;
            }
        }

        #endregion
    }
}
