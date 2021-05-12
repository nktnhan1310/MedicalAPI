using Medical.Service.Factory;
using Medical.Entities.DomainEntity;
using Medical.Interface.DbContext;
using Medical.Interface.Repository;
using Medical.Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class MedicalUnitOfWork : UnitOfWork, IMedicalUnitOfWork
    {
        readonly IMedicalDbContext medicalDbContext;
        public MedicalUnitOfWork(IMedicalDbContext context) : base(context)
        {
            medicalDbContext = context;
        }

        public override ICatalogueRepository<T> CatalogueRepository<T>()
        {
            return new CatalogueRepository<T>(medicalDbContext);
        }

        public override IDomainRepository<T> Repository<T>()
        {
            return new MedicalRepository<T>(medicalDbContext);
        }
    }
}
