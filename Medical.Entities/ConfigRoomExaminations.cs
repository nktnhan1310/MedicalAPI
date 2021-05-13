using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Bảng cấu hình số lượng người khám trong ngày theo phòng
    /// </summary>
    public class ConfigRoomExaminations : MedicalAppDomain
    {
        /// <summary>
        /// Mã phòng
        /// </summary>
        public int RoomExaminationId { get; set; }

        /// <summary>
        /// Số lượng bệnh nhân khám bệnh
        /// </summary>
        public int TotalPatient { get; set; }

        #region Extension Properties

        /// <summary>
        /// Mã phòng
        /// </summary>
        [NotMapped]
        public string RoomCode { get; set; }
        /// <summary>
        /// Tên phòng
        /// </summary>
        [NotMapped]
        public string RoomName { get; set; }

        #endregion
    }
}
