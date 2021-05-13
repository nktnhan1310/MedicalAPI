using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Model
{
    public class ConfigRoomExaminationModel : MedicalAppDomainModel
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
        public string RoomCode { get; set; }
        /// <summary>
        /// Tên phòng
        /// </summary>
        public string RoomName { get; set; }

        #endregion
    }
}
