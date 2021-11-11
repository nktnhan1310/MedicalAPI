using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class UpdateMedicalRecordDetailNoteModel
    {
        /// <summary>
        /// Mã tiểu sử
        /// </summary>
        public int MedicalRecordDetailId { get; set; }
        /// <summary>
        /// Thông tin ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
