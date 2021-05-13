using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class ExaminationScheduleDetailService : DomainService<ExaminationScheduleDetails, BaseSearch>, IExaminationScheduleDetailService
    {
        public ExaminationScheduleDetailService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
