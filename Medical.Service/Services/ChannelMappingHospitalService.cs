using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class ChannelMappingHospitalService : DomainService<ChannelMappingHospital, BaseSearch>, IChannelMappingHospitalService
    {
        public ChannelMappingHospitalService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
