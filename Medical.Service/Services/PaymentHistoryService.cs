using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service
{
    public class PaymentHistoryService : DomainService<PaymentHistories, BaseSearch>, IPaymentHistoryService
    {
        public PaymentHistoryService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return base.GetStoreProcName();
        }

        protected override SqlParameter[] GetSqlParameters(BaseSearch baseSearch)
        {
            return base.GetSqlParameters(baseSearch);
        }

    }
}
