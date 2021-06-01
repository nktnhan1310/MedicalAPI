using AutoMapper;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Interface.Services.Base;
using Medical.Interface.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service.Services.DomainService
{
    public abstract class CoreHospitalService<E, T> : DomainService<E, T>, ICoreHospitalService<E, T> where E : MedicalAppDomainHospital, new() where T : BaseHospitalSearch, new()
    {
        protected CoreHospitalService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
