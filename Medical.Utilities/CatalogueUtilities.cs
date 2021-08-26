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
            PaymentReExaminationFailed = 8
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

    }
}
