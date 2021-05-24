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
    public class ExaminationScheduleModel : MedicalAppDomainModel
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
        public int HospitalId { get; set; }

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
        /// Tên chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }
        /// <summary>
        /// Danh sách ca làm việc
        /// </summary>
        public IList<ExaminationScheduleDetailModel> ExaminationScheduleDetails { get; set; }

        #endregion
    }
}
