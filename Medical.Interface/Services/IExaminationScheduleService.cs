using Medical.Entities;
using Medical.Interface.Services.Base;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IExaminationScheduleService : IDomainService<ExaminationSchedules, SearchExaminationSchedule>
    {
        Task<IList<ExaminationSchedules>> GetExaminationSchedules(SearchExaminationScheduleForm searchExaminationScheduleDetail);

        Task<PagedList<ExaminationSchedules>> GetAllExaminationSchedules(SearchExaminationScheduleDetailV2 searchExaminationScheduleDetailV2);
    }
}
