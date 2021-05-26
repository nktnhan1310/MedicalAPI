using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Lịch sử của lịch hẹn
    /// </summary>
    public class ExaminationHistories : MedicalAppDomain
    {
        /// <summary>
        /// Mã lịch hẹn
        /// </summary>
        public int ExaminationFormId { get; set; }
        /// <summary>
        /// Hành động (Tạo lịch hẹn,...)
        /// </summary>
        public int Action { get; set; }
        /// <summary>
        /// Trạng thái lịch hẹn (Chờ xác nhận,....)
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Comment khi duyệt phiếu khám
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Mô tả lịch hẹn
        /// </summary>
        public string Note { get; set; }
    }
}
