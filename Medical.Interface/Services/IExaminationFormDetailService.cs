using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IExaminationFormDetailService : ICoreHospitalService<ExaminationFormDetails, SearchExaminationFormDetail>
    {
        Task<bool> UpdateExaminationFormDetailStatus(UpdateExaminationFormDetailStatus updateExaminationFormDetailStatus);
    }
}
