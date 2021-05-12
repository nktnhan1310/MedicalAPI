using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface.Repository
{
    public interface ICatalogueRepository<T>: IDomainRepository<T> where T : MedicalCatalogueAppDomain
    {
        T GetByCode(string code);
    }
}
