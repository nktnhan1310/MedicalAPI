using Medical.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface
{
    public interface IHospitalHistoryService : IDomainService<HospitalHistories, BaseHospitalSearch>
    {
    }
}
