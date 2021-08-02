using Medical.Entities;
using Medical.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IExaminationFormService : IDomainService<ExaminationForms, SearchExaminationForm>
    {
        Task UpdateCurrentExaminationJob();
        Task<bool> UpdateExaminationStatus(UpdateExaminationStatus updateExaminationStatus, bool isAdmin = false);
        Task<string> GetExaminationFormIndex(SearchExaminationIndex searchExaminationIndex);

        Task<string> GetCheckStatusMessage(int examinationFormId, int statusCheck);
    }
}
