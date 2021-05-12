using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services.Base
{
    public interface ICatalogueService<T, E> : IDomainService<T, E> where T: MedicalCatalogueAppDomain where E : BaseSearch
    {
        T GetByCode(string code);
    }
}
