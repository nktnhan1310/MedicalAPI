using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface.Repository
{
    public interface IMedicalRepository<T> : IDomainRepository<T> where T : MedicalAppDomain
    {
    }
}
