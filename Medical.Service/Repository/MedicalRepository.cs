using Medical.Entities.DomainEntity;
using Medical.Interface.DbContext;
using Medical.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class MedicalRepository<T>: DomainRepository<T>, IDomainRepository<T> where T : MedicalAppDomain
    {
        public MedicalRepository(IMedicalDbContext context) : base(context)
        {

        }

    }
}
