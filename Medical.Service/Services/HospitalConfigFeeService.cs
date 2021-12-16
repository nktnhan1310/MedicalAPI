using AutoMapper;
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
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
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
                // Lấy ra thông tin chi phí của chuyên khoa
                var specialistTypeInfo = await this.unitOfWork.Repository<SpecialistTypes>().GetQueryable().Where(e => !e.Deleted && e.Active
                    && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                    && e.Id == feeCaculateExaminationRequest.SpecialistTypeId
                    ).FirstOrDefaultAsync();
                if (specialistTypeInfo != null)
                    price = (specialistTypeInfo.Price ?? 0);
                // Lấy ra thông tin chi phí của dịch vụ (nếu có)
                var examinationInfo = await this.unitOfWork.Repository<ExaminationForms>().GetQueryable()
                    .Where(e => e.Id == feeCaculateExaminationRequest.ExaminationFormId).FirstOrDefaultAsync();
                // NẾU LÀ KHÁM THƯỜNG
                if (feeCaculateExaminationRequest.AdditionServiceIds != null && feeCaculateExaminationRequest.AdditionServiceIds.Any())
                {
                    // Lấy danh sách dịch vụ phát sinh
                    var additionServiceInfos = await this.unitOfWork.Repository<AdditionServices>().GetQueryable()
                        .Where(e => feeCaculateExaminationRequest.AdditionServiceIds.Contains(e.Id)).ToListAsync();
                    //var additionServiceInfos = await this.unitOfWork.Repository<ExaminationFormAdditionServiceMappings>().GetQueryable()
                    //    .Where(e => !e.Deleted && e.ExaminationFormId == examinationInfo.Id).ToListAsync();
                    //if (additionServiceInfos != null && additionServiceInfos.Any())
<<<<<<< HEAD
                    price += additionServiceInfos.Sum(e => e.Price ?? 0);
=======
                        price += additionServiceInfos.Sum(e => e.Price ?? 0);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                }
                // Lấy ra thông tin chi phí của loại vaccine (nếu có)
                if (feeCaculateExaminationRequest.VaccineTypeId.HasValue)
                {
                    var vaccineTypeInfo = await this.unitOfWork.Repository<VaccineTypes>().GetQueryable()
                    .Where(e => e.Id == feeCaculateExaminationRequest.VaccineTypeId.Value).FirstOrDefaultAsync();
                    if (vaccineTypeInfo != null && vaccineTypeInfo.Price.HasValue && vaccineTypeInfo.Price.Value > 0)
                        price += (vaccineTypeInfo.Price ?? 0);
                }
<<<<<<< HEAD

                // Tính toán dịch vụ chi tiết phát sinh
                if (feeCaculateExaminationRequest.AdditionServiceDetailIds != null && feeCaculateExaminationRequest.AdditionServiceDetailIds.Any())
                {
                    // Lấy ra danh sách chi tiết dịch vụ phát sinh
                    var additionServiceDetailInfos = await this.unitOfWork.Repository<AdditionServiceDetails>().GetQueryable()
                        .Where(e => !e.Deleted && feeCaculateExaminationRequest.AdditionServiceDetailIds.Contains(e.Id)).ToListAsync();
                    if (additionServiceDetailInfos != null && additionServiceDetailInfos.Any())
                        price += additionServiceDetailInfos.Sum(e => e.Price ?? 0);
                }
=======


                //var configFeeHospital = await this.Queryable.Where(e => !e.Deleted && e.Active
                //&& e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //&& e.PaymentMethodId == feeCaculateExaminationRequest.PaymentMethodId
                //&& e.ServiceTypeId == feeCaculateExaminationRequest.ServiceTypeId
                //&& (!feeCaculateExaminationRequest.SpecialistTypeId.HasValue || e.SpecialistTypeId == feeCaculateExaminationRequest.SpecialistTypeId.Value)
                //).FirstOrDefaultAsync();

                // Phí tiện ích hệ thống
                //var configFee = await this.unitOfWork.Repository<SystemConfigFee>().GetQueryable()
                //    .Where(e => !e.Deleted && e.Active).FirstOrDefaultAsync();

                //if (feeCaculateExaminationRequest.SpecialistTypeId.HasValue && feeCaculateExaminationRequest.SpecialistTypeId.Value > 0)
                //{
                //    var specialistTypeInfo = await this.unitOfWork.Repository<SpecialistTypes>().GetQueryable().Where(e => !e.Deleted && e.Active
                //    && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //    && e.Id == feeCaculateExaminationRequest.SpecialistTypeId
                //    ).FirstOrDefaultAsync();
                //    if (specialistTypeInfo != null)
                //        price = specialistTypeInfo.Price;
                //}

                //if (feeCaculateExaminationRequest.VaccineTypeId.HasValue && feeCaculateExaminationRequest.VaccineTypeId.Value > 0)
                //{
                //    var vaccineTypeInfo = await this.unitOfWork.Repository<VaccineTypes>().GetQueryable().Where(e => !e.Deleted && e.Active
                //    && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //    && e.Id == feeCaculateExaminationRequest.VaccineTypeId
                //    ).FirstOrDefaultAsync();
                //    if (vaccineTypeInfo != null)
                //        price = vaccineTypeInfo.Price;
                //}


                //if (!price.HasValue || price.Value <= 0)
                //{
                //    var serviceTypeMappingInfo = await this.unitOfWork.Repository<ServiceTypeMappingHospital>().GetQueryable().Where(e => !e.Deleted && e.Active
                //        && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //        && e.ServiceTypeId == feeCaculateExaminationRequest.ServiceTypeId
                //        ).FirstOrDefaultAsync();
                //    if (serviceTypeMappingInfo != null)
                //        price = serviceTypeMappingInfo.Price;
                //}

                // Lấy phí của dịch vụ phát sinh (nếu có)
                //if (feeCaculateExaminationRequest.ExaminationFormId.HasValue && feeCaculateExaminationRequest.ExaminationFormId.Value > 0)
                //{
                //    var currentExaminationFormDetails = await this.unitOfWork.Repository<ExaminationFormDetails>().GetQueryable().Where(e => e.ExaminationFormId == feeCaculateExaminationRequest.ExaminationFormId.Value && !e.Deleted && !e.MedicalRecordDetailId.HasValue).ToListAsync();
                //    if (currentExaminationFormDetails != null && currentExaminationFormDetails.Any())
                //    {
                //        price += currentExaminationFormDetails.Sum(e => e.Price ?? 0);
                //    }
                //}

                //if (configFee != null)
                //{
                //    // Xét TH.a: Tính theo phần trăm
                //    if (configFee.IsCheckRate)
                //    {
                //        if (price.HasValue)
                //            fee = (price ?? 0) * (configFee.Rate / 100);
                //    }
                //    // Xét TH.b: Lấy phí cấu hình
                //    else
                //        fee = configFee.Fee;
                //}
                //if (configFeeHospital != null)
                //{
                //    // Xét TH.a: Tính theo phần trăm
                //    if (configFeeHospital.IsRate)
                //    {
                //        if (price.HasValue)
                //            fee += (price ?? 0) * (configFeeHospital.FeeRate / 100);
                //    }
                //    // Xét TH.b: Lấy phí cấu hình
                //    else
                //        fee += configFeeHospital.Fee;
                //}
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608


                //var configFeeHospital = await this.Queryable.Where(e => !e.Deleted && e.Active
                //&& e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //&& e.PaymentMethodId == feeCaculateExaminationRequest.PaymentMethodId
                //&& e.ServiceTypeId == feeCaculateExaminationRequest.ServiceTypeId
                //&& (!feeCaculateExaminationRequest.SpecialistTypeId.HasValue || e.SpecialistTypeId == feeCaculateExaminationRequest.SpecialistTypeId.Value)
                //).FirstOrDefaultAsync();

                // Phí tiện ích hệ thống
                //var configFee = await this.unitOfWork.Repository<SystemConfigFee>().GetQueryable()
                //    .Where(e => !e.Deleted && e.Active).FirstOrDefaultAsync();

                //if (feeCaculateExaminationRequest.SpecialistTypeId.HasValue && feeCaculateExaminationRequest.SpecialistTypeId.Value > 0)
                //{
                //    var specialistTypeInfo = await this.unitOfWork.Repository<SpecialistTypes>().GetQueryable().Where(e => !e.Deleted && e.Active
                //    && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //    && e.Id == feeCaculateExaminationRequest.SpecialistTypeId
                //    ).FirstOrDefaultAsync();
                //    if (specialistTypeInfo != null)
                //        price = specialistTypeInfo.Price;
                //}

                //if (feeCaculateExaminationRequest.VaccineTypeId.HasValue && feeCaculateExaminationRequest.VaccineTypeId.Value > 0)
                //{
                //    var vaccineTypeInfo = await this.unitOfWork.Repository<VaccineTypes>().GetQueryable().Where(e => !e.Deleted && e.Active
                //    && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //    && e.Id == feeCaculateExaminationRequest.VaccineTypeId
                //    ).FirstOrDefaultAsync();
                //    if (vaccineTypeInfo != null)
                //        price = vaccineTypeInfo.Price;
                //}


                //if (!price.HasValue || price.Value <= 0)
                //{
                //    var serviceTypeMappingInfo = await this.unitOfWork.Repository<ServiceTypeMappingHospital>().GetQueryable().Where(e => !e.Deleted && e.Active
                //        && e.HospitalId == feeCaculateExaminationRequest.HospitalId
                //        && e.ServiceTypeId == feeCaculateExaminationRequest.ServiceTypeId
                //        ).FirstOrDefaultAsync();
                //    if (serviceTypeMappingInfo != null)
                //        price = serviceTypeMappingInfo.Price;
                //}

                // Lấy phí của dịch vụ phát sinh (nếu có)
                //if (feeCaculateExaminationRequest.ExaminationFormId.HasValue && feeCaculateExaminationRequest.ExaminationFormId.Value > 0)
                //{
                //    var currentExaminationFormDetails = await this.unitOfWork.Repository<ExaminationFormDetails>().GetQueryable().Where(e => e.ExaminationFormId == feeCaculateExaminationRequest.ExaminationFormId.Value && !e.Deleted && !e.MedicalRecordDetailId.HasValue).ToListAsync();
                //    if (currentExaminationFormDetails != null && currentExaminationFormDetails.Any())
                //    {
                //        price += currentExaminationFormDetails.Sum(e => e.Price ?? 0);
                //    }
                //}

                //if (configFee != null)
                //{
                //    // Xét TH.a: Tính theo phần trăm
                //    if (configFee.IsCheckRate)
                //    {
                //        if (price.HasValue)
                //            fee = (price ?? 0) * (configFee.Rate / 100);
                //    }
                //    // Xét TH.b: Lấy phí cấu hình
                //    else
                //        fee = configFee.Fee;
                //}
                //if (configFeeHospital != null)
                //{
                //    // Xét TH.a: Tính theo phần trăm
                //    if (configFeeHospital.IsRate)
                //    {
                //        if (price.HasValue)
                //            fee += (price ?? 0) * (configFeeHospital.FeeRate / 100);
                //    }
                //    // Xét TH.b: Lấy phí cấu hình
                //    else
                //        fee += configFeeHospital.Fee;
                //}

            }
            feeCaculateExamination.ExaminationPrice = price;
            feeCaculateExamination.ExaminationFee = fee;
            return feeCaculateExamination;
        }
    }
}
