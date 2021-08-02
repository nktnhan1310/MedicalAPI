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
    public class DoctorModel : MedicalAppDomainHospitalModel
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
        /// Mô tả thêm
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Thông tin user
        /// </summary>
        public int? UserId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên đầy đủ của bác sĩ
        /// </summary>
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        /// <summary>
        /// Tên học vị
        /// </summary>
        public string DegreeTypeName { get; set; }
        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        public string SpecialistTypeName { get; set; }
       
        /// <summary>
        /// Chuyên khoa theo từng bác sĩ
        /// </summary>
        public IList<DoctorDetailModel> DoctorDetails { get; set; }

        #endregion

    }
}
