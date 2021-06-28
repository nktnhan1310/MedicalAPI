using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IRoomExaminationService : ICatalogueHospitalService<RoomExaminations, SearchHopitalExtension>
    {
        /// <summary>
        /// Lấy thông tin lịch trực theo phòng
        /// </summary>
        /// <param name="searchHopitalExtension"></param>
        /// <returns></returns>
        Task<IList<ExaminationScheduleDetails>> GetRoomDetail(SearchHopitalExtension searchHopitalExtension);
    }
}
