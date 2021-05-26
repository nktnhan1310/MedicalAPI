using Medical.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    /// <summary>
    /// Lịch sử của lịch hẹn (phiếu khám)
    /// </summary>
    public class ExaminationHistoryModel : MedicalAppDomainModel
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
