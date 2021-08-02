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
                new SqlParameter("@ExaminationFormDetailId", baseSearch.ExaminationFormDetailId),
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
                if ((existExaminationFormDetail.Status == (int)CatalogueUtilities.AdditionServiceStatus.New
                        || existExaminationFormDetail.Status == (int)CatalogueUtilities.AdditionServiceStatus.PaymentFailed
                        )
                        && updateExaminationFormDetailStatus.PaymentMethodId.HasValue && updateExaminationFormDetailStatus.PaymentMethodId.Value > 0)
                {
                    existExaminationFormDetail.PaymentMethodId = updateExaminationFormDetailStatus.PaymentMethodId.Value;
                    includeProperties = new Expression<Func<ExaminationFormDetails, object>>[]
                    {
                            e => e.Status,
                            e => e.PaymentMethodId
                    };
                }
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
                                PaymentMethodId = updateExaminationFormDetailStatus.PaymentMethodId ?? 0,
                                HospitalId = existExaminationFormDetail.HospitalId,
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

        /// <summary>
        /// Check trạng thái cập nhật hiện tại của dịch vụ phát sinh
        /// </summary>
        /// <param name="examinationFormDetailId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public async Task<string> GetCheckStatusMessage(int examinationFormDetailId, int statusCheck)
        {
            string result = string.Empty;
            bool isError = false;
            var examinationFormDetailInfo = await this.unitOfWork.Repository<ExaminationFormDetails>().GetQueryable()
                .Where(e => e.Id == examinationFormDetailId)
                .FirstOrDefaultAsync();
            if (examinationFormDetailInfo != null)
            {
                switch (statusCheck)
                {
                    case (int)CatalogueUtilities.AdditionServiceStatus.WaitConfirmPayment:
                    case (int)CatalogueUtilities.AdditionServiceStatus.New:
                        {
                            if (examinationFormDetailInfo.Status != (int)CatalogueUtilities.AdditionServiceStatus.New
                                && examinationFormDetailInfo.Status != (int)CatalogueUtilities.AdditionServiceStatus.PaymentFailed
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.AdditionServiceStatus.WaitForService:
                        {
                            if (examinationFormDetailInfo.Status != (int)CatalogueUtilities.AdditionServiceStatus.WaitConfirmPayment
                                )
                                isError = true;
                        }
                        break;
                    case (int)CatalogueUtilities.AdditionServiceStatus.Finish:
                        {
                            if (examinationFormDetailInfo.Status != (int)CatalogueUtilities.AdditionServiceStatus.WaitForService
                                )
                                isError = true;
                        }
                        break;
                    default:
                        break;
                }
                if (isError) return "Trạng thái dịch vụ phát sinh hợp lệ! Không thể cập nhật";
                return string.Empty;
            }
            else return "Không tìm thấy thông tin dịch vụ phát sinh";
        }

    }
}
