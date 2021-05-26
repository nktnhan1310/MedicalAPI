﻿using AutoMapper;
using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class HospitalConfigFeeService : DomainService<HospitalConfigFees, SearchHospitalConfigFee>, IHospitalConfigFeeService
    {
        public HospitalConfigFeeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "HospitalConfigFee_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchHospitalConfigFee baseSearch)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@PaymentMethodId", baseSearch.PaymentMethodId),
                new SqlParameter("@ServiceTypeId", baseSearch.ServiceTypeId),
                new SqlParameter("@SpecialistTypeId", baseSearch.SpecialistTypeId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return sqlParameters;
        }

        public override async Task<string> GetExistItemMessage(HospitalConfigFees item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistConfigFee = await this.Queryable.AnyAsync(e => !e.Deleted && e.Active && e.Id != item.Id
            && e.HospitalId == item.HospitalId
            && e.PaymentMethodId == item.PaymentMethodId
            && e.ServiceTypeId == item.ServiceTypeId
            && (!item.SpecialistTypeId.HasValue || e.SpecialistTypeId == item.SpecialistTypeId.Value)
            );
            if (isExistConfigFee)
                messages.Add("Cấu hình phí đã tồn tại");
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

        /// <summary>
        /// Tính toán mức giá khám và phí (nếu có)
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="examinationForms"></param>
        /// <returns></returns>
        public async Task<FeeCaculateExaminationResponse> GetFeeExamination(FeeCaculateExaminationRequest feeCaculateExaminationRequest)
        {
            FeeCaculateExaminationResponse feeCaculateExamination = new FeeCaculateExaminationResponse();
            // Giá khám bệnh
            double? price = null;
            // Phí khám
            double? fee = null;
            if (feeCaculateExaminationRequest != null)
            {
                var configFee = await this.Queryable.Where(e => !e.Deleted && e.Active
                && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                && e.PaymentMethodId == feeCaculateExaminationRequest.PaymentMethodId
                && e.ServiceTypeId == feeCaculateExaminationRequest.ServiceTypeId
                && (!feeCaculateExaminationRequest.SpecialistTypeId.HasValue || e.SpecialistTypeId == feeCaculateExaminationRequest.SpecialistTypeId.Value)
                ).FirstOrDefaultAsync();


                if (feeCaculateExaminationRequest.SpecialistTypeId.HasValue && feeCaculateExaminationRequest.SpecialistTypeId.Value > 0)
                {
                    var specialistTypeInfo = await this.unitOfWork.Repository<SpecialistTypes>().GetQueryable().Where(e => !e.Deleted && e.Active
                    && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                    && e.Id == feeCaculateExaminationRequest.SpecialistTypeId
                    ).FirstOrDefaultAsync();
                    if (specialistTypeInfo != null)
                        price = specialistTypeInfo.Price;
                }
                if (!price.HasValue || price.Value <= 0)
                {
                    var serviceTypeMappingInfo = await this.unitOfWork.Repository<ServiceTypeMappingHospital>().GetQueryable().Where(e => !e.Deleted && e.Active
                        && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                        && e.ServiceTypeId == feeCaculateExaminationRequest.ServiceTypeId
                        ).FirstOrDefaultAsync();
                    if (serviceTypeMappingInfo != null)
                        price = serviceTypeMappingInfo.Price;
                }

                if (configFee != null)
                {
                    // Xét TH.a: Tính theo phần trăm
                    if (configFee.IsRate)
                    {
                        if (price.HasValue)
                            fee = (price ?? 0) * (configFee.FeeRate / 100);
                    }
                    // Xét TH.b: Lấy phí cấu hình
                    else
                        fee = configFee.Fee;
                }
            }
            feeCaculateExamination.ExaminationPrice = price;
            feeCaculateExamination.ExaminationFee = fee;
            return feeCaculateExamination;
        }
    }
}
