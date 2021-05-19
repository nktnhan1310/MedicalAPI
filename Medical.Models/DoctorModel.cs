using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Bác sĩ
    /// </summary>
    public class DoctorModel : MedicalAppDomainModel
    {
        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(500)]
        public string FirstName { get; set; }
        /// <summary>
        /// Họ
        /// </summary>
        [StringLength(500)]
        public string LastName { get; set; }
        /// <summary>
        /// Giới tính
        /// 0 => Nam
        /// 1 => Nữ
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// Trình độ (GS,PGS,ThS,...)
        /// </summary>
        public int? DegreeId { get; set; }

        /// <summary>
        /// Giấy chứng chỉ hành nghề bác sĩ
        /// </summary>
        [StringLength(50)]
        public string PracticingCertificate { get; set; }

        /// <summary>
        /// Nơi đào tạo
        /// </summary>
        [StringLength(500)]
        public string TrainingPlace { get; set; }

        /// <summary>
        /// Bệnh viện
        /// </summary>
        public int HospitalId { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên học vị
        /// </summary>
        public string DegreeTypeName { get; set; }
        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }
        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        public string HospitalName { get; set; }
        /// <summary>
        /// Chuyên khoa theo từng bác sĩ
        /// </summary>
        public IList<DoctorDetailModel> DoctorDetails { get; set; }

        #endregion

    }
}
