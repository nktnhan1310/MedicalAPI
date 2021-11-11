using Medical.Entities;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IExaminationScheduleDetailService : IDomainService<ExaminationScheduleDetails, SearchExaminationScheduleDetail>
    {
        /// <summary>
        /// Lấy danh sách phòng theo ca trực
        /// </summary>
        /// <param name="searchRoomExaminationSchedule"></param>
        /// <returns></returns>
        Task<PagedList<ExaminationScheduleDetails>> GetRoomExaminationScheduleDetailInfo(SearchRoomExaminationSchedule searchRoomExaminationSchedule);
    }
}
