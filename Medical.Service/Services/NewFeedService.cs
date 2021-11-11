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
    public class NewFeedService : CoreHospitalService<NewFeeds, BaseHospitalSearch>, INewFeedService
    {
        public NewFeedService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "NewFeed_GetPagingData";
        }
    }
}
