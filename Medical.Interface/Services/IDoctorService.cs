using Medical.Entities;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IDoctorService : IDomainService<Doctors, SearchDoctor>
    {
        Task<PagedList<DoctorDetails>> GetListDoctorExaminations(SearchExaminationScheduleDetailV2 searchExaminationScheduleDetailV2);
    }
}
