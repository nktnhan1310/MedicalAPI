using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bệnh viện
    /// </summary>
    [Table("Hospitals")]
    public class Hospitals : MedicalAppDomain
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        [StringLength(20)]
        public string Code { get; set; }
        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        /// <summary>
        /// Đường dẫn website (nếu có)
        /// </summary>
        [StringLength(50)]
        public string WebSiteUrl { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// Khẩu hiệu
        /// </summary>
        [StringLength(500)]
        public string Slogan { get; set; }
        
        /// <summary>
        /// Cung cấp thông tin bệnh viện
        /// </summary>
        public bool IsProvideInformation { get; set; }

        /// <summary>
        /// Ngày cung cấp thông tin
        /// </summary>
        public DateTime? ProvideDate { get; set; }

        /// <summary>
        /// Số phút trung bình khám mỗi ca
        /// </summary>
        public int MinutePerPatient { get; set; }
        //----------------------------------------------- THÔNG TIN CHUYÊN GIA
        /// <summary>
        /// Có bộ phận IT ko?
        /// </summary>
        public bool IsHasItExpert { get; set; }
        /// <summary>
        /// Tên chuyên gia
        /// </summary>
        [StringLength(500)]
        public string ExpertFullName { get; set; }
        /// <summary>
        /// Số điện thoại chuyên gia
        /// </summary>
        [StringLength(20)]
        public string ExpertPhone { get; set; }

        //-------------------------------------------- THÔNG TIN PHẦN MỀM GỌI SỐ
        public bool IsHasCallPort { get; set; }
        /// <summary>
        /// Mô tả cổng thông tin nếu có phần mệm gọi số
        /// </summary>
        public string CallPortDescription { get; set; }

        /// <summary>
        /// Mô tả cổng thông tin nếu không có phần mềm gọi số
        /// </summary>
        public string NoCallPortDescription { get; set; }

        /// <summary>
        /// Thời gian đóng nhận bệnh
        /// </summary>
        public long? TickEndReceiveExamination { get; set; }

        /// <summary>
        /// Loại bệnh viện
        /// </summary>
        public int? HospitalTypeId { get; set; }

        /// <summary>
        /// Chức năng của bệnh viện
        /// </summary>
        public int? HospitalFunctionTypeId { get; set; }

        /// <summary>
        /// Cò check làm việc thứ 7
        /// </summary>
        public bool IsSaturdayWorking { get; set; }

        /// <summary>
        /// Cờ check làm việc chủ nhật
        /// </summary>
        public bool IsSundayWorking { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên loại bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalTypeName { get; set; }

        /// <summary>
        /// Tên chức năng bệnh viện
        /// </summary>
        [NotMapped]
        public string HospitalFunctionTypeName { get; set; }

        /// <summary>
        /// Giá trị thời gian kết thúc khám bệnh (00:00:00)
        /// </summary>
        [NotMapped]
        public string TickEndReceiveExaminationValue { get; set; }

        [NotMapped]
        public int TotalVisitNo { get; set; }
        /// <summary>
        /// Bảng mapping dịch vụ khám của bệnh viện
        /// </summary>
        [NotMapped]
        public IList<ServiceTypeMappingHospital> ServiceTypeMappingHospitals { get; set; }

        /// <summary>
        /// Bảng mapping kênh khám của bệnh viện
        /// </summary>
        [NotMapped]
        public IList<ChannelMappingHospital> ChannelMappingHospitals { get; set; }

        /// <summary>
        /// Danh sách kênh được chọn
        /// </summary>
        [NotMapped]
        public List<int> ChannelIds { get; set; }

        /// <summary>
        /// File của thông tin bệnh viện (logo/sơ đồ bệnh viện/ danh sách chuyên khoa)
        /// </summary>
        [NotMapped]
        public IList<HospitalFiles> HospitalFiles { get; set; }

        /// <summary>
        /// Thông tin ngân hàng liên kết bệnh viện
        /// </summary>
        [NotMapped]
        public IList<BankInfos> BankInfos { get; set; }


        #endregion

    }
}
