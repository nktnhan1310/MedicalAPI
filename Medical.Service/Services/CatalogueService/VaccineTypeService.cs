using AutoMapper;
using Medical.Entities;
using Medical.Interface;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class VaccineTypeService : CatalogueHospitalService<VaccineTypes, BaseHospitalSearch>, IVaccineTypeService
    {
        public VaccineTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
