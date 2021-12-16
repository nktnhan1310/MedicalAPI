using AutoMapper;
using Medical.Entities;
using Medical.Interface;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
<<<<<<< HEAD
using Microsoft.Extensions.Configuration;
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class DiagnoticTypeService : CatalogueHospitalService<DiagnoticTypes, BaseHospitalSearch>, IDiagnoticTypeService
    {
<<<<<<< HEAD
        public DiagnoticTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
=======
        public DiagnoticTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        {
        }
    }
}
