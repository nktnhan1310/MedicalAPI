using Medical.Entities.DomainEntity;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Chi tiết lịch khám
    /// </summary>
    [Table("ExaminationScheduleDetails")]
    public class ExaminationScheduleDetails : MedicalAppDomain
    {
        /// <summary>
        /// Lịch khám
        /// </summary>
        public int ScheduleId { get; set; }

        /// <summary>
        /// Từ giờ 
        /// </summary>
        public int? FromTime { get; set; }

        /// <summary>
        /// Text hiển thị từ giờ
        /// </summary>
        public string FromTimeText { get; set; }

        /// <summary>
        /// Đến giờ
        /// </summary>
        public int? ToTime { get; set; }

        /// <summary>
        /// Text hiển thị đến giờ
        /// </summary>
        public string ToTimeText { get; set; }

        /// <summary>
        /// Phòng khám
        /// </summary>
        public int RoomExaminationId { get; set; }

        /// <summary>
        /// Số ca khám tối đa trong ngày theo khung giờ
        /// </summary>
        public int? MaximumExamination { get; set; }

        /// <summary>
        /// Mã bác sĩ thay thế
        /// </summary>
        public int? ReplaceDoctorId { get; set; }

        /// <summary>
        /// Id của buổi
        /// </summary>
        public int? SessionTypeId { get; set; }

        /// <summary>
        /// Cờ check sử dụng cấu hình số phút khám của bệnh viện
        /// </summary>
        [DefaultValue(false)]
        public bool IsUseHospitalConfig { get; set; }

        /// <summary>
        /// Guid import lịch
        /// </summary>
        public Guid? ImportScheduleId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên buổi khám
        /// </summary>
        [NotMapped]
        public string SessionTypeName { get; set; }

        /// <summary>
        /// Giá trị từ giờ
        /// </summary>
        [NotMapped]
        public string FromTimeDisplay
        {
            get
            {
                string result = string.Empty;
                if (FromTime.HasValue) return DateTimeUtilities.ConvertTotalMinuteToString(FromTime.Value);
                return result;
            }
        }

        /// <summary>
        /// Giá trị đến giờ
        /// </summary>
        [NotMapped]
        public string ToTimeDisplay
        {
            get
            {
                string result = string.Empty;
                if (ToTime.HasValue) return DateTimeUtilities.ConvertTotalMinuteToString(ToTime.Value);
                return result;
            }
        }

        /// <summary>
        /// Mã chuyên khoa
        /// </summary>
        [NotMapped]
        public int? SpecialistTypeId { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [NotMapped]
        public int? DoctorId { get; set; }

        /// <summary>
        /// Ca khám
        /// </summary>
        [NotMapped]
        public string ConfigTimeExaminationValue { get; set; }

        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        [NotMapped]
        public string SpecialistTypeName { get; set; }

        /// <summary>
        /// Chức danh + tên bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorDisplayName { get; set; }

        /// <summary>
        /// Tên hiển thị của bác sĩ thay thế
        /// </summary>
        [NotMapped]
        public string ReplaceDoctorDisplayName { get; set; }

        /// <summary>
        /// Tên hiển thị bác sĩ thay thế của lịch
        /// </summary>
        [NotMapped]
        public string ReplaceDoctorScheduleDisplayName { get; set; }

        /// <summary>
        /// Mã bác sĩ
        /// </summary>
        [NotMapped]
        public string DoctorCode { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        [NotMapped]
        public DateTime? ExaminationDate { get; set; }
        /// <summary>
        /// Phòng khám
        /// </summary>
        [NotMapped]
        public string RoomExaminationName { get; set; }

        /// <summary>
        /// Lấy ra cấu hình thời gian theo ngày khám
        /// </summary>
        [NotMapped]
        public string ExaminationScheduleDetailInfoText { get; set; }

        /// <summary>
        /// Danh sách thời gian trực theo chi tiết ca trực
        /// </summary>
        [NotMapped]
        public IList<ExaminationScheduleDetailInfo> ExaminationScheduleDetailInfos
        {
            get
            {
                if (string.IsNullOrEmpty(ExaminationScheduleDetailInfoText)) return null;
                IList<ExaminationScheduleDetailInfo> examinationScheduleDetailInfos = new List<ExaminationScheduleDetailInfo>();
                var itemArrays = ExaminationScheduleDetailInfoText.Split(';').Distinct().ToArray();
                if (itemArrays == null || !itemArrays.Any()) return null;
                foreach (var itemArray in itemArrays)
                {
                    var propertyArray = itemArray.Split('_').ToArray();
                    if (propertyArray == null || !propertyArray.Any()) continue;

                    int fromTime = 0;
                    int toTime = 0;
                    int examinationScheduleDetailId = 0;
                    int.TryParse(propertyArray[0], out fromTime);
                    int.TryParse(propertyArray[2], out toTime);
                    int.TryParse(propertyArray[4], out examinationScheduleDetailId);

                    ExaminationScheduleDetailInfo examinationScheduleDetailInfo = new ExaminationScheduleDetailInfo()
                    {
                        FromTime = fromTime,
                        ToTime = toTime,
                        FromTimeText = propertyArray[1],
                        ToTimeText = propertyArray[3],
                        ExaminationScheduleDetailId = examinationScheduleDetailId
                    };
                    examinationScheduleDetailInfos.Add(examinationScheduleDetailInfo);
                }

                return examinationScheduleDetailInfos;
            }
        }

        /// <summary>
        /// Thông tin y tá của lịch khám
        /// </summary>
        [NotMapped]
        public string NurseInfoText { get; set; }

        /// <summary>
        /// Danh sách thông tin y tá của ca trực
        /// </summary>
        [NotMapped]
        public IList<NurseInfo> NurseInfos
        {
            get
            {
                if (string.IsNullOrEmpty(NurseInfoText)) return null;
                IList<NurseInfo> nurseInfos = new List<NurseInfo>();
                var itemArrays = NurseInfoText.Split(';').Distinct().ToArray();
                if (itemArrays == null || !itemArrays.Any()) return null;
                foreach (var itemArray in itemArrays)
                {
                    var propertyArray = itemArray.Split('_').ToArray();
                    if (propertyArray == null || !propertyArray.Any()) continue;

                    int nurseId = 0;
                    int.TryParse(propertyArray[0], out nurseId);

                    NurseInfo nurseInfo = new NurseInfo()
                    {
                        NurseId = nurseId,
                        NurseName = propertyArray[1]
                    };
                    nurseInfos.Add(nurseInfo);
                }

                return nurseInfos;
            }
        }

        #endregion

    }

    /// <summary>
    /// Thông tin thời gian khám của ca trực
    /// </summary>
    public class ExaminationScheduleDetailInfo
    {
        /// <summary>
        /// Từ giờ
        /// </summary>
        public int FromTime { get; set; }
        public string FromTimeText { get; set; }
        /// <summary>
        /// Đến giờ
        /// </summary>
        public int ToTime { get; set; }
        public string ToTimeText { get; set; }

        /// <summary>
        /// Mã của chi tiết ca trực
        /// </summary>
        public int ExaminationScheduleDetailId { get; set; }
    }

    /// <summary>
    /// Thông tin y tá
    /// </summary>
    public class NurseInfo
    {
        public int NurseId { get; set; }
        public string NurseName { get; set; }
    }

}
