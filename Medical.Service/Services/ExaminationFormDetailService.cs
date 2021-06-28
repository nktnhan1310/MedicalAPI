using AutoMapper;
using Medical.Entities;
using Medical.Interface;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class ExaminationFormDetailService : CoreHospitalService<ExaminationFormDetails, SearchExaminationFormDetail>, IExaminationFormDetailService
    {
        private readonly ISMSConfigurationService sMSConfigurationService;
        public ExaminationFormDetailService(IMedicalUnitOfWork unitOfWork, IMapper mapper, ISMSConfigurationService sMSConfigurationService) : base(unitOfWork, mapper)
        {
            this.sMSConfigurationService = sMSConfigurationService;
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationFormDetail_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchExaminationFormDetail baseSearch)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@ExaminationFormId", baseSearch.ExaminationFormId),
                new SqlParameter("@AdditionServiceId", baseSearch.AdditionServiceId),
                new SqlParameter("@MedicalRecordId", baseSearch.MedicalRecordId),
                new SqlParameter("@PaymentMethodId", baseSearch.PaymentMethodId),
                new SqlParameter("@Status", baseSearch.Status),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),


                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),


            };
            return base.GetSqlParameters(baseSearch);
        }

        /// <summary>
        /// Cập nhật trạng thái của dịch vụ phát sinh
        /// </summary>
        /// <param name="updateExaminationFormDetailStatus"></param>
        /// <returns></returns>
        public async Task<bool> UpdateExaminationFormDetailStatus(UpdateExaminationFormDetailStatus updateExaminationFormDetailStatus)
        {
            bool result = false;
            var existExaminationFormDetail = await this.unitOfWork.Repository<ExaminationFormDetails>().GetQueryable()
                .Where(e => e.Id == updateExaminationFormDetailStatus.ExaminationFormDetailId).FirstOrDefaultAsync();
            if(existExaminationFormDetail != null)
            {
                Expression<Func<ExaminationFormDetails, object>>[] includeProperties = new Expression<Func<ExaminationFormDetails, object>>[] 
                {
                    e => e.Status
                };
                existExaminationFormDetail.Status = updateExaminationFormDetailStatus.Status;
                switch (updateExaminationFormDetailStatus.Status)
                {
                    case (int)CatalogueUtilities.AdditionServiceStatus.WaitForService:
                        {
                            // Lấy STT chờ thực hiện dịch vụ
                            existExaminationFormDetail.AdditionExaminationIndex = await this.GetExaminationFormDetailIndex(existExaminationFormDetail);
                            includeProperties = new Expression<Func<ExaminationFormDetails, object>>[]
                            {
                                e => e.Status,
                                e => e.AdditionExaminationIndex
                            };
                            // Lưu lịch sử thanh toán
                            PaymentHistories paymentHistories = new PaymentHistories()
                            {
                                Active = true,
                                Deleted = false,
                                Created = DateTime.Now,
                                CreatedBy = updateExaminationFormDetailStatus.CreatedBy,
                                AdditionServiceId = existExaminationFormDetail.AdditionServiceId,
                                ExaminationFormId = existExaminationFormDetail.ExaminationFormId,
                                ExaminationFormDetailId = existExaminationFormDetail.Id,
                                ExaminationFee = existExaminationFormDetail.Price,
                                PaymentMethodId = existExaminationFormDetail.PaymentMethodId ?? 0,
                            };
                            await this.unitOfWork.Repository<PaymentHistories>().CreateAsync(paymentHistories);
                        }
                        break;
                    default:
                        break;
                }
                await this.unitOfWork.Repository<ExaminationFormDetails>().UpdateFieldsSaveAsync(existExaminationFormDetail, includeProperties);
                await this.unitOfWork.SaveAsync();
                result = true;
            }

            if (result)
            {
                //--------------------------- GỬI SMS STT CHO USER
                //--------------------------- idnex string
                //......................................................
                if (updateExaminationFormDetailStatus.Status == (int)CatalogueUtilities.AdditionServiceStatus.WaitForService)
                {
                    if (existExaminationFormDetail != null)
                    {
                        var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == existExaminationFormDetail.UserId).FirstOrDefaultAsync();
                        if(userInfo != null)
                        {
                            if (!string.IsNullOrEmpty(existExaminationFormDetail.AdditionExaminationIndex))
                                await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", existExaminationFormDetail.AdditionExaminationIndex));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Lấy stt thực hiện dịch vụ
        /// </summary>
        /// <param name="searchExaminationIndex"></param>
        /// <returns></returns>
        public async Task<string> GetExaminationFormDetailIndex(ExaminationFormDetails item)
        {
            string indexString = string.Empty;
            int index = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", item.HospitalId),
                new SqlParameter("@AdditionServiceId", item.AdditionServiceId),
                new SqlParameter("@ExaminationDate", item.ExaminationDate),
                new SqlParameter("@ExaminationIndex", SqlDbType.Int, 0),

            };
            var obj = await this.unitOfWork.Repository<ExaminationForms>().ExcuteStoreGetValue("Index_GetExaminationDetailIndex", parameters, "@ExaminationIndex");
            indexString = obj.ToString();
            if (!string.IsNullOrEmpty(indexString))
            {
                int.TryParse(indexString, out index);
                index += 1;
            }
            indexString = index.ToString("D3");
            return indexString;
        }

        /// <summary>
        /// Kiểm tra trùng dịch vụ phát sinh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(ExaminationFormDetails item)
        {
            List<string> messages = new List<string>();
            bool isExistAdditionServiceId = await this.unitOfWork.Repository<ExaminationFormDetails>().GetQueryable()
                .AnyAsync(e => 
                !e.Deleted 
                && e.Id != item.Id 
                && e.ExaminationFormId == item.ExaminationFormId 
                && e.AdditionServiceId == item.AdditionServiceId);
            if (isExistAdditionServiceId)
                messages.Add("Dịch vụ phát sinh đã tồn tại");
            string result = string.Empty;
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }
    }
}
