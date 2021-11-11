using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    [Table("Doctors")]
    public class Doctors : MedicalAppDomainHospital
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
        /// Học vấn (GS,PGS,ThS,...)
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

        /// <summary>
        /// Loại nhân viên
        /// </summary>
        public int TypeId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên học vị
        /// </summary>
        [NotMapped]
        public string DegreeTypeName { get; set; }
        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }
        
        /// <summary>
        /// Chuyên khoa theo từng bác sĩ
        /// </summary>
        [NotMapped]
        public IList<DoctorDetails> DoctorDetails { get; set; }

        #endregion

    }
}
