using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchConfigRoomExamination : BaseSearch
    {
        /// <summary>
        /// Mã phòng
        /// </summary>
        public int? RoomExaminationId { get; set; }
    }
}
