using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IRoomExaminationService : ICatalogueHospitalService<RoomExaminations, SearchRoomExamination>
    {
        /// <summary>
        /// Lấy thông tin lịch trực theo phòng
        /// </summary>
        /// <param name="searchHopitalExtension"></param>
        /// <returns></returns>
        Task<IList<ExaminationScheduleDetails>> GetRoomDetail(SearchHopitalExtension searchHopitalExtension);


        /// <summary>
        /// Lấy thông tin mã phòng theo chuyên khoa thứ tự phòng, nếu có
        /// </summary>
        /// <param name="roomIndex"></param>
        /// <param name="roomName"></param>
        /// <param name="specialistTypeCode"></param>
        /// <returns></returns>
        string GenerateRoomCode(string roomIndex, string roomName, string specialistTypeCode);
    }
}
