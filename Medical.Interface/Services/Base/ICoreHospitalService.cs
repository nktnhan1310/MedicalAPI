using AutoMapper;
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
    public interface ICoreHospitalService<E, T> : IDomainService<E, T> where E : MedicalAppDomainHospital where T : BaseHospitalSearch 
    {
    }
}
