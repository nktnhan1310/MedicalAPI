using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Utilities
{
    public class CatalogueUtilities
    {
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
            /// Đã xác nhận
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
            /// Đã xác nhận tái khám
            /// </summary>
            ConfirmedReExamination = 5
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
            ConfirmReExamination = 4
            
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
    }
}
