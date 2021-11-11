using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class VNPayPaymentHistoryService : DomainService<VNPayPaymentHistories, BaseSearch>, IVNPayPaymentHistoryService
    {
        public VNPayPaymentHistoryService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
