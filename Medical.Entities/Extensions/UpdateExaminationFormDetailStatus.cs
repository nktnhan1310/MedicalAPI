﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class UpdateExaminationFormDetailStatus
    {
        /// <summary>
        /// Trạng thái cập nhật
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Mã chi tiết dịch vụ phát sinh
        /// </summary>
        public int ExaminationFormDetailId { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
