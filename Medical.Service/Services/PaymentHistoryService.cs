using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Medical.Service
{
    public class PaymentHistoryService : CoreHospitalService<PaymentHistories, SearchPaymentHistory>, IPaymentHistoryService
    {
        public PaymentHistoryService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "PaymentHistories_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchPaymentHistory baseSearch)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@RecordId", baseSearch.RecordId),
                new SqlParameter("@PaymentMethodId", baseSearch.PaymentMethodId),
                new SqlParameter("@BankInfoId", baseSearch.BankInfoId),
                new SqlParameter("@ExaminationFormId", baseSearch.ExaminationFormId),
                new SqlParameter("@ExaminationFormDetailId", baseSearch.ExaminationFormDetailId),
                new SqlParameter("@AdditionServiceId", baseSearch.AdditionServiceTypeId),
                new SqlParameter("@MedicalBillId", baseSearch.MedicalBillId),
                new SqlParameter("@PaymentDate", baseSearch.PaymentDate),


                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

    }
}
