using Medical.Entities;
using Medical.Entities.DomainEntity.Search;
using Medical.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface.Services
{
    public interface IReportUserExaminationService : IReportCoreService<ReportUserExaminationForm, SearchUserExaminationForm>
    {
    }
}
