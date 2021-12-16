﻿using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Bệnh viện
    /// </summary>
    public class HospitalModel: MedicalAppDomainModel
    {
        /// <summary>
        /// Mã bệnh viện
        /// </summary>
        [StringLength(20, ErrorMessage = "Mã bệnh viện tối đa 20 kí tự")]
        public string Code { get; set; }
        /// <summary>
        /// Tên bệnh viện
        /// </summary>
        [StringLength(200, ErrorMessage = "Tên bệnh viện tối đa 200 kí tự")]
        public string Name { get; set; }
        [StringLength(500, ErrorMessage = "Địa chỉ bệnh viện tối đa 500 kí tự")]
        public string Address { get; set; }
        [StringLength(12, ErrorMessage = "Số kí tự của số điện thoại phải lớn hơn 8 và nhỏ hơn 12!", MinimumLength = 9)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        /// <summary>
        /// Đường dẫn website (nếu có)
        /// </summary>
        [StringLength(50)]
        public string WebSiteUrl { get; set; }

        private string _Email { get; set; }
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email
        {
            get { return _Email; }
            set { _Email = string.IsNullOrWhiteSpace(value) ? null : value; }
        }
        /// <summary>
        /// Khẩu hiệu
        /// </summary>
        [StringLength(500, ErrorMessage = "Slogan tối đa 500 kí tự")]
        public string Slogan { get; set; }
        /// <summary>
        /// Cung cấp thông tin bệnh viện
        /// </summary>
        [DefaultValue(false)]
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
        [DefaultValue(false)]

        public bool IsHasItExpert { get; set; }
        /// <summary>
        /// Tên chuyên gia
        /// </summary>
        [StringLength(500, ErrorMessage = "Tên chuyên gia tối đa 1000 kí tự")]
        public string ExpertFullName { get; set; }
        /// <summary>
        /// Số điện thoại chuyên gia
        /// </summary>
        [StringLength(20, ErrorMessage = "Số điện thoại chuyên gia tối đa 20 kí tự")]
        public string ExpertPhone { get; set; }

        //-------------------------------------------- THÔNG TIN PHẦN MỀM GỌI SỐ
        [DefaultValue(false)]
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
        public long? TickEndReceiveExamination
        {
            get
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0);
                var dateTimeCheck = DateTime.Now.Date + ts;
                if(!string.IsNullOrEmpty(TickEndReceiveExaminationValue) && TimeSpan.TryParse(TickEndReceiveExaminationValue, out ts))
                {
                    dateTimeCheck = dateTimeCheck.Date + ts;
                    return dateTimeCheck.Ticks;
                }
                return null;
            }
            set
            {
                
            }
        }

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

<<<<<<< HEAD
        #region Manager INFO

        /// <summary>
        /// Tên giám đốc
        /// </summary>
        public string ManagerName { get; set; }

        /// <summary>
        /// Địa chỉ giám đốc
        /// </summary>
        public string ManagerAddress { get; set; }

        /// <summary>
        /// Số điện thoại giám đốc
        /// </summary>
        public string ManagerPhone { get; set; }

        /// <summary>
        /// Email giám đốc
        /// </summary>
        public string ManagerEmail { get; set; }

        /// <summary>
        /// Ngày tham gia MrAPP
        /// </summary>
        public DateTime? JoinInDate { get; set; }

        #endregion

=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        #region Extension Properties

        /// <summary>
        /// Tên loại bệnh viện
        /// </summary>
        public string HospitalTypeName { get; set; }

        /// <summary>
        /// Tên chức năng bệnh viện
        /// </summary>
        public string HospitalFunctionTypeName { get; set; }

        /// <summary>
        /// Giá trị thời gian kết thúc khám bệnh (00:00:00)
        /// </summary>
        public string TickEndReceiveExaminationValue { get; set; }

        /// <summary>
        /// Bảng mapping dịch vụ khám của bệnh viện
        /// </summary>
        public IList<ServiceTypeMappingHospitalModel> ServiceTypeMappingHospitals { get; set; }

        /// <summary>
        /// Bảng mapping kênh khám của bệnh viện
        /// </summary>
        //public IList<ChannelMappingHospitalModel> ChannelMappingHospitals { get; set; }

        /// <summary>
        /// Danh sách kênh được chọn
        /// </summary>
        public List<int> ChannelIds { get; set; }

        /// <summary>
        /// File của thông tin bệnh viện (logo/sơ đồ bệnh viện/ danh sách chuyên khoa)
        /// </summary>
        public IList<HospitalFileModel> HospitalFiles { get; set; }

        /// <summary>
        /// Thông tin ngân hàng liên kết bệnh viện
        /// </summary>
        public IList<BankInfoModel> BankInfos { get; set; }

        #endregion




    }
}
