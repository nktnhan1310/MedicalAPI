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
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class MedicalBillService : CatalogueHospitalService<MedicalBills, SearchMedicalBill>, IMedicalBillService
    {
        private readonly ISMSConfigurationService sMSConfigurationService;

        public MedicalBillService(IMedicalUnitOfWork unitOfWork, IMapper mapper, ISMSConfigurationService sMSConfigurationService) : base(unitOfWork, mapper)
        {
            this.sMSConfigurationService = sMSConfigurationService;
        }

        protected override string GetStoreProcName()
        {
            return "MedicalBill_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchMedicalBill baseSearch)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@ExaminationFormId", baseSearch.ExaminationFormId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@Status", baseSearch.Status),
                new SqlParameter("@PaymentMethodId", baseSearch.PaymentMethodId),
                new SqlParameter("@BankInfoId", baseSearch.BankInfoId),
                new SqlParameter("@MedicalRecordDetailId", baseSearch.MedicalRecordDetailId),
                new SqlParameter("@DoctorId", baseSearch.DoctorId),
                new SqlParameter("@MedicalBillId", baseSearch.MedicalBillId),
                new SqlParameter("@CreatedDate", baseSearch.CreatedDate),

                

                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0)
            };
            return sqlParameters;
        }

        public override async Task<PagedList<MedicalBills>> GetPagedListData(SearchMedicalBill baseSearch)
        {
            PagedList<MedicalBills> pagedList = new PagedList<MedicalBills>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await this.unitOfWork.Repository<MedicalBills>().ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }

        /// <summary>
        /// Cập nhật trạng thái toa thuốc
        /// </summary>
        /// <param name="updateMedicalBillStatus"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMedicalBillStatus(UpdateMedicalBillStatus updateMedicalBillStatus)
        {
            bool result = false;
            var existMedicalBill = await this.unitOfWork.Repository<MedicalBills>().GetQueryable().Where(e => e.Id == updateMedicalBillStatus.MedicalBillId).FirstOrDefaultAsync();
            if (existMedicalBill != null)
            {
                if (updateMedicalBillStatus.Status > 0)
                {
                    Expression<Func<MedicalBills, object>>[] expressions = new Expression<Func<MedicalBills, object>>[]
                    {
                        e => e.Status
                    };
                    existMedicalBill.Status = updateMedicalBillStatus.Status;
                    switch (updateMedicalBillStatus.Status)
                    {
                        case (int)CatalogueUtilities.MedicalBillStatus.Wait:
                            {
                                if (updateMedicalBillStatus.PaymentMethodId.HasValue && updateMedicalBillStatus.PaymentMethodId > 0)
                                {
                                    // Lưu lịch sử thanh toán
                                    PaymentHistories paymentHistories = new PaymentHistories()
                                    {
                                        Active = true,
                                        Deleted = false,
                                        CreatedBy = updateMedicalBillStatus.CreatedBy,
                                        Created = DateTime.Now,
                                        ExaminationFee = updateMedicalBillStatus.TotalPrice,
                                        ServiceFee = updateMedicalBillStatus.Fee,
                                        AdditionServiceId = null,
                                        BankInfoId = updateMedicalBillStatus.BankInfoId,
                                        ExaminationFormId = existMedicalBill.ExaminationFormId,
                                        MedicalBillId = existMedicalBill.Id,
                                        ExaminationFormDetailId = null,
                                        HospitalId = existMedicalBill.HospitalId,
                                        PaymentMethodId = updateMedicalBillStatus.PaymentMethodId ?? 0,
                                    };
                                    await this.unitOfWork.Repository<PaymentHistories>().CreateAsync(paymentHistories);

                                    expressions = new Expression<Func<MedicalBills, object>>[]
                                    {
                                        e => e.Status,
                                        e => e.MedicalBillIndex
                                    };
                                    // Tạo STT chờ lấy thuốc
                                    existMedicalBill.MedicalBillIndex = await this.GetMedicalBillIndex(existMedicalBill);

                                }
                                else throw new Exception("Vui lòng chọn phương thức thanh toán");
                            }
                            break;
                        default:
                            break;
                    }
                    await this.unitOfWork.Repository<MedicalBills>().UpdateFieldsSaveAsync(existMedicalBill, expressions);
                    await this.unitOfWork.SaveAsync();
                    result = true;
                }
            }

            if (result)
            {
                //--------------------------- GỬI SMS STT CHO USER
                //--------------------------- idnex string
                //......................................................
                if (updateMedicalBillStatus.Status == (int)CatalogueUtilities.MedicalBillStatus.Wait)
                {
                    if (existMedicalBill != null)
                    {
                        var medicalRecordInfo = await unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == existMedicalBill.MedicalRecordId).FirstOrDefaultAsync();
                        if(medicalRecordInfo != null)
                        {
                            var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == medicalRecordInfo.UserId).FirstOrDefaultAsync();
                            if (userInfo != null)
                            {
                                if (!string.IsNullOrEmpty(existMedicalBill.MedicalBillIndex))
                                    await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format("{0} la ma dat lai mat khau Baotrixemay cua ban", existMedicalBill.MedicalBillIndex));
                            }
                        }
                       
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Lấy stt chờ lấy thuốc
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<string> GetMedicalBillIndex(MedicalBills item)
        {
            string indexString = string.Empty;
            int index = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", item.HospitalId),
                new SqlParameter("@CreatedDate", item.Created),
                new SqlParameter("@MedicalBillIndex", SqlDbType.Int, 0),

            };
            var obj = await this.unitOfWork.Repository<ExaminationForms>().ExcuteStoreGetValue("Index_GetMedicalBillIndex", parameters, "@MedicalBillIndex");
            indexString = obj.ToString();
            if (!string.IsNullOrEmpty(indexString))
            {
                int.TryParse(indexString, out index);
                index += 1;
            }
            indexString = index.ToString("D3");
            return indexString;
        }

    }
}
