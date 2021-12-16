using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Utilities
{
    public class CatalogueUtilities
    {
        /// <summary>
        /// Loại dịch vụ phát sinh
        /// </summary>
        public enum AdditionServiceType
        {
            /// <summary>
            /// Xét nghiệm
            /// </summary>
            XN = 0,
            /// <summary>
            /// Chụp x-quang
            /// </summary>
            XQ = 1,
            /// <summary>
            /// Siêu âm
            /// </summary>
            SA = 2
        }

        /// <summary>
        /// Trạng thái toa thuốc
        /// </summary>
        public enum MedicalBillStatus
        {
            /// <summary>
            /// Mới tạo
            /// </summary>
            New = 0,
            /// <summary>
            /// Chờ xác nhận thanh toán
            /// </summary>
            WaitPayment = 1,
            /// <summary>
            /// Chờ lấy thuốc
            /// </summary>
            Wait = 2,
            /// <summary>
            /// Hoàn thành đơn thuốc
            /// </summary>
            Finished = 3,
            /// <summary>
            /// Thanh toán thất bại
            /// </summary>
            PaymentFailed = 4,
        }

        /// <summary>
        /// Loại dịch vụ
        /// </summary>
        public enum ServiceType
        {
            /// <summary>
            /// Khám thường
            /// </summary>
            KT = 1,
            /// <summary>
            /// Khám bảo hiểm
            /// </summary>
            KBH = 2,
            /// <summary>
            /// Khám dịch vụ
            /// </summary>
            KDV = 3,
            /// <summary>
            /// Khám theo yêu cầu
            /// </summary>
            KTYC = 4,
            /// <summary>
            /// Khám ngoài giờ
            /// </summary>
            KNG = 5,
            /// <summary>
            /// Chích ngừa
            /// </summary>
            CN = 6,
            /// <summary>
            /// Khám chuyên khoa
            /// </summary>
            KCK = 7
        }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public enum PaymentMethod
        {
            /// <summary>
            /// Thanh toán COD (trực tiếp)
            /// </summary>
            COD = 0,
            /// <summary>
            /// Thanh toán qua APP
            /// </summary>
            APP = 1,
            /// <summary>
            /// Thanh toán qua MOMO
            /// </summary>
            MOMO = 2,
            /// <summary>
            /// Thanh toán qua VNPay
            /// </summary>
            VNPAY = 3,
<<<<<<< HEAD
            /// <summary>
            /// Chuyển khoản
            /// </summary>
            TRANSFER = 4
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        public enum AdditionServiceStatus
        {
            /// <summary>
            /// Trạng thái mới
            /// </summary>
            New = 0,
            /// <summary>
            /// Chờ xác nhận thanh toán
            /// </summary>
            WaitConfirmPayment = 1,
            /// <summary>
            /// Chờ thực hiện dịch vụ
            /// </summary>
            WaitForService = 2,
            /// <summary>
            /// Hoàn thành dịch vụ
            /// </summary>
            Finish = 3,
            /// <summary>
            /// Thanh toán thất bại
            /// </summary>
            PaymentFailed = 4,
        }

        /// <summary>
        /// Trạng thái của phiếu khám bệnh
        /// </summary>
        public enum ExaminationStatus
        {
            /// <summary>
            /// Phiếu mới (Lưu nháp)
            /// </summary>
            New = 0,
            /// <summary>
            /// Chờ xác nhận
            /// </summary>
            WaitConfirm = 1,
            /// <summary>
            /// Đã xác nhận thanh toán
            /// </summary>
            Confirmed = 2,
            /// <summary>
            /// Đã hủy
            /// </summary>
            Canceled = 3,
            /// <summary>
            /// Chờ xác nhận tái khám
            /// </summary>
            WaitReExamination = 4,
            /// <summary>
            /// Đã xác nhận thanh toán tái khám
            /// </summary>
            ConfirmedReExamination = 5,
            /// <summary>
            /// Khám hoàn tất
            /// </summary>
            FinishExamination = 6,
            /// <summary>
            /// Thanh toán thất bại
            /// </summary>
            PaymentFailed = 7,
            /// <summary>
            /// Thanh toán tái khám thất bại
            /// </summary>
            PaymentReExaminationFailed = 8,
            /// <summary>
            /// Chờ hoàn tiền
            /// </summary>
            WaitRefund = 9,
            /// <summary>
            /// Đã hoàn tiền
            /// </summary>
            RefundSuccess = 10,
            /// <summary>
            /// Hoàn tiền thất bại
            /// </summary>
            RefundFailed = 11
        }

        /// <summary>
        /// Action trên phiếu khám bệnh
        /// </summary>
        public enum ExaminationAction
        {
            /// <summary>
            /// Tạo phiếu
            /// </summary>
            Create = 0,
            /// <summary>
            /// Cập nhật thông tin phiếu
            /// </summary>
            Update = 1,
            /// <summary>
            /// Xác nhận phiếu
            /// </summary>
            Confirm = 2,
            /// <summary>
            /// Hủy phiếu
            /// </summary>
            Cancel = 3,
            /// <summary>
            /// Xác nhận tái khám
            /// </summary>
            ConfirmReExamination = 4,
            /// <summary>
            /// Hoàn thành khám
            /// </summary>
            FinishExamination = 5,
            /// <summary>
            /// Trả lại phiếu
            /// </summary>
            Return = 6,
            /// <summary>
            /// Trả lại phiếu tái khám
            /// </summary>
            ReturnReExamination = 7,
            /// <summary>
            /// Hoàn tiền
            /// </summary>
            Refund = 8,
            /// <summary>
            /// Xóa
            /// </summary>
            Delete = 9,
        }

        public enum ExaminationType
        {
            /// <summary>
            /// Theo ngày
            /// </summary>
            Date = 0,
            /// <summary>
            /// Theo bác sĩ
            /// </summary>
            Doctor = 1,
        }

        /// <summary>
        /// Loại file của bệnh viện
        /// </summary>
        public enum HospitalFileType
        {
            /// <summary>
            /// Logo
            /// </summary>
            Logo = 0,
            /// <summary>
            /// Sơ đồ bệnh viện
            /// </summary>
            Mapping = 1,
            /// <summary>
            /// Chuyên khoa
            /// </summary>
            SpecialList = 2
        }

        public enum UserFileType
        {
            /// <summary>
            /// Avatar User
            /// </summary>
            AVATAR = 0,
            /// <summary>
            /// AVATAR HỒ SƠ
            /// </summary>
            AvatarMedicalRecord = 1,
            /// <summary>
            /// HÌNH TOA THUỐC
            /// </summary>
            MedicalBill = 2,
            /// <summary>
            /// HÌNH XÉT NGHIỆM
            /// </summary>
            Experiment = 3,
            /// <summary>
            /// HÌNH SIÊU ÂM
            /// </summary>
            SuperSonic = 4,
            /// <summary>
            /// X-quang
            /// </summary>
            Xray = 5
<<<<<<< HEAD

=======
            
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }

        /// <summary>
        /// Loại thông báo
        /// </summary>
        public enum NotificationType
        {
            /// <summary>
            /// Hệ thống
            /// </summary>
            SYS = 0,
            /// <summary>
            /// Bệnh viện
            /// </summary>
            HOS = 1,
            /// <summary>
            /// Người dùng
            /// </summary>
            USER = 2,
            /// <summary>
            /// Nhóm người dùng
            /// </summary>
            USERGROUP = 3,
        }

        /// <summary>
        /// Loại hình thông báo
        /// </summary>
        public enum NotificationCatalogueType
        {
            /// <summary>
            /// Thông báo hủy lịch
            /// </summary>
            CancelExamination = 0,
            /// <summary>
            /// Thông báo STT khám bệnh
            /// </summary>
            ExaminationIndex = 1,
            /// <summary>
            /// Thông báo nhắc lịch khám
            /// </summary>
            RemindExamination = 2,
            /// <summary>
            /// Thông báo thanh toán
            /// </summary>
            Payment = 3,
            /// <summary>
            /// Thông báo hoàn tiền
            /// </summary>
            Refund = 4,
            /// <summary>
            /// Thông báo thời gian khám kế tiếp (10,15,30 phút đên luợt khám của bạn)
            /// </summary>
            NextExamination = 5,
            /// <summary>
            /// Thông báo đối soát
            /// </summary>
            Control = 6,
            /// <summary>
            /// Thông báo tin tức mới
            /// </summary>
            NewFeeds = 7,
            /// <summary>
            /// Thông báo khuyến mãi
            /// </summary>
            Promotion = 8,
            /// <summary>
            /// Thông báo tiền viện phí
            /// </summary>
            HospitalFee = 9,
            /// <summary>
            /// Thông báo chúc mừng ra viện
            /// </summary>
            OutOfHospital = 10,
            /// <summary>
            /// Thông báo chúc sức khỏe
            /// </summary>
            HappyHealth = 11,
            /// <summary>
            /// Chúc mừng sinh nhật
            /// </summary>
            HappyBirthDate = 12,
            /// <summary>
            /// Thông báo đổi mật khẩu
            /// </summary>
            ChangePassword = 13,
            /// <summary>
            /// Thông báo tài khoản được đăng nhập trên thiết bị khác
            /// </summary>
            SignInAnotherDevice = 14
        }

        /// <summary>
        /// Thông tin buổi khám
        /// </summary>
        public enum SessionType
        {
            /// <summary>
            /// Buổi sáng
            /// </summary>
            BS = 0,
            /// <summary>
            /// Buổi chiều
            /// </summary>
            BC = 1,
            /// <summary>
            /// Buổi trưa
            /// </summary>
            BTR = 2,
            /// <summary>
            /// Buổi tối
            /// </summary>
            BT = 3
        }

        /// <summary>
        /// Loại thai kỳ
        /// </summary>
        public enum PregnancyType
        {
            /// <summary>
            /// Theo ngày
            /// </summary>
            Day = 0,
            /// <summary>
            /// Theo tuần
            /// </summary>
            Week = 1,
            /// <summary>
            /// Theo tháng
            /// </summary>
            Month = 2
        }

        public enum TargetType
        {
            /// <summary>
            /// Vaccine cho trẻ em
            /// </summary>
            Child = 0,
            /// <summary>
            /// Vaccine cho thanh thiếu niên
            /// </summary>
            Youth = 1,
            /// <summary>
            /// Vaccine cho người lớn
            /// </summary>
            Adult = 2,
            /// <summary>
            /// Vaccine cho người giá
            /// </summary>
            Elder = 3,
            /// <summary>
            /// Vaccine cho bà bầu
            /// </summary>
            Pregnant = 4,
        }

        /// <summary>
        /// Phân loại file của hệ thống
        /// </summary>
        public enum SystemFileType
        {
            /// <summary>
            /// Hình ảnh slidebar của app
            /// </summary>
            AppImageSlideBar = 0,
            /// <summary>
            /// Banner quảng cáo của hệ thống
            /// </summary>
            Banner = 1,
            /// <summary>
            /// Loại file khác
            /// </summary>
            Other = 2,
        }

        /// <summary>
        /// Tên chức danh của nhân viên bệnh viện
        /// </summary>
        public enum DoctorType
        {
            /// <summary>
            /// Bác sĩ
            /// </summary>
            Doctor = 0,
            /// <summary>
            /// Y tá
            /// </summary>
            Nurse = 1,
            /// <summary>
            /// Điều dưỡng
            /// </summary>
            Nursing = 2
        }

        /// <summary>
<<<<<<< HEAD
        /// Loại chính sách
        /// </summary>
        public enum PolicyType
        {
            /// <summary>
            /// Chính sách chung
            /// </summary>
            General = 0,
            /// <summary>
            /// Chính sách hủy phiếu
            /// </summary>
            CancelExamination = 1,
            /// <summary>
            /// Chính sách thay đổi phiếu
            /// </summary>
            UpdateExamination = 2,
            /// <summary>
            /// Chính sách của bệnh viện
            /// </summary>
            Hospital = 3,
            /// <summary>
            /// Chính sách khác
            /// </summary>
            Other = 4
        }

        /// <summary>
        /// Nhóm dị ứng thuốc
        /// </summary>
        public enum AllergyType
        {
            /// <summary>
            /// Dị ứng thuốc
            /// </summary>
            DUT = 1,
            /// <summary>
            /// Dị ứng mỹ phẩm
            /// </summary>
            DUMP = 2,
            /// <summary>
            /// Dị ứng thời tiết
            /// </summary>
            DUTT = 3,
            /// <summary>
            /// Dị ứng thực phẩm
            /// </summary>
            DUTP = 4,
            /// <summary>
            /// Khác
            /// </summary>
            OTHER = 5,
        }

        /// <summary>
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Loại folder
        /// </summary>
        public enum FolderType
        {
            /// <summary>
            /// Toa thuốc
            /// </summary>
            Prescription = 1,
            /// <summary>
            /// Xét nghiệm
            /// </summary>
            Analysys = 2,
            /// <summary>
            /// Hồ sơ bệnh án
            /// </summary>
            MedicalRecord = 3,
            /// <summary>
            /// Khác
            /// </summary>
            Other = 4
        }

    }
}
