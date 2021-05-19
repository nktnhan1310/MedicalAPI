using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Lịch khám theo từng bác sĩ
    /// </summary>
    public class ExaminationScheduleModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Bác sĩ
        /// </summary>
        public int DoctorId { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ExaminationDate { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }
        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// Danh sách ca làm việc
        /// </summary>
        public IList<ExaminationScheduleDetailModel> ExaminationScheduleDetails { get; set; }

        #endregion
    }
}
