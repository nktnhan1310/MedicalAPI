using Medical.Models.DomainModel;
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
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không hợp lệ")]
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

        #region Extension Properties

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
        public IList<ChannelMappingHospitalModel> ChannelMappingHospitals { get; set; }

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
