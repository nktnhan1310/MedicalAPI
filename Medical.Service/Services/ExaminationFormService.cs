using AutoMapper;
using Medical.Entities;
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
using Medical.Utilities;
using Medical.Entities.Extensions;
using System.Linq.Expressions;

namespace Medical.Service
{
    public class ExaminationFormService : DomainService<ExaminationForms, SearchExaminationForm>, IExaminationFormService
    {
        public ExaminationFormService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "ExaminationForm_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchExaminationForm baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@RecordId", baseSearch.RecordId),
                new SqlParameter("@TypeId", baseSearch.TypeId),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@ReExaminationDate", baseSearch.ReExaminationDate),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Cập nhật thông trạng thái lịch hẹn (phiếu khám)
        /// </summary>
        /// <param name="updateExaminationStatus"></param>
        /// <returns></returns>
        public async Task<bool> UpdateExaminationStatus(UpdateExaminationStatus updateExaminationStatus)
        {
            bool result = false;
            int? action = null;
            var existExaminationFormInfo = await Queryable.Where(e => e.Id == updateExaminationStatus.ExaminationFormId).FirstOrDefaultAsync();
            Expression<Func<ExaminationForms, object>>[] includeProperties = new Expression<Func<ExaminationForms, object>>[] { };
            if (existExaminationFormInfo != null)
            {
                if (updateExaminationStatus.Status.HasValue)
                {
                    existExaminationFormInfo.Status = updateExaminationStatus.Status.Value;
                    existExaminationFormInfo.Updated = DateTime.Now;
                    existExaminationFormInfo.UpdatedBy = updateExaminationStatus.CreatedBy;
                    existExaminationFormInfo.PaymentMethodId = updateExaminationStatus.PaymentMethodId;
                    existExaminationFormInfo.BankInfoId = updateExaminationStatus.BankInfoId;
                    switch (updateExaminationStatus.Status)
                    {
                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                                existExaminationFormInfo = await this.GetExaminationIndex(existExaminationFormInfo);
                                existExaminationFormInfo.Code = RandomUtilities.RandomString(6);
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Code,
                                    x => x.ExaminationIndex,
                                    x => x.ExaminationPaymentIndex,
                                    x => x.PaymentMethodId,
                                };

                            }
                            break;
                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Cancel;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                };
                            }
                            break;
                        // Nếu trạng thái: đã xác nhận tái khám => lưu lại ngày tái khám trong phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination;
                                existExaminationFormInfo.ReExaminationDate = DateTime.Now;
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.ReExaminationDate
                                };
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            // Thêm lịch sử phiếu khám/lịch hẹn
            if (action.HasValue && updateExaminationStatus.Status.HasValue)
            {
                unitOfWork.Repository<ExaminationForms>().UpdateFieldsSave(existExaminationFormInfo, includeProperties);
                await unitOfWork.SaveAsync();
                existExaminationFormInfo.Comment = updateExaminationStatus.Comment;
                await CreateExaminationHistory(existExaminationFormInfo, updateExaminationStatus.CreatedBy);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Tạo mới phiếu khám bệnh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ExaminationForms item)
        {
            bool result = false;
            if (item != null)
            {
                if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
                    item = await this.GetExaminationIndex(item);

                await unitOfWork.Repository<ExaminationForms>().CreateAsync(item);
                await unitOfWork.SaveAsync();

                // Tạo lịch sử tạo phiếu khám bệnh
                await CreateExaminationHistory(item, item.CreatedBy);

                //--------------------------- GỬI SMS STT CHO USER
                //--------------------------- idnex string
                //......................................................

                result = true;
            }
            return result;
        }

        

        /// <summary>
        /// Cập nhật thông tin phiếu khám bệnh (lịch khám)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(ExaminationForms item)
        {
            bool result = false;
            if (item != null)
            {
                var existItem = await Queryable.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                if (existItem != null)
                {
                    item.Updated = DateTime.Now;
                    if (item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
                        item = await this.GetExaminationIndex(item);

                    existItem = mapper.Map<ExaminationForms>(item);
                    await unitOfWork.SaveAsync();

                    // Tạo lịch sử tạo phiếu khám bệnh
                    await CreateExaminationHistory(item, item.UpdatedBy);

                    //--------------------------- GỬI SMS STT CHO USER
                    //--------------------------- idnex string
                    //......................................................
                }
            }

            return result;
        }

        public async Task<ExaminationForms> GetExaminationIndex(ExaminationForms item)
        {
            if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value > 0 && item.Status == (int)CatalogueUtilities.ExaminationStatus.Confirmed)
            {
                var paymentMethodInfo = await unitOfWork.Repository<PaymentMethods>().GetQueryable().Where(e => !e.Deleted && e.Active
                && e.Id == item.PaymentMethodId).FirstOrDefaultAsync();
                if (paymentMethodInfo != null)
                {
                    string indexString = string.Empty;
                    // Lưu lịch sử thanh toán
                    PaymentHistories paymentHistories = new PaymentHistories();
                    //TH1: Thanh toán COD => Lấy STT tại phòng khám
                    //=> Cập nhật lại STT tại phòng khám cho mẫu phiếu khám bệnh
                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString())
                    {
                        // Lấy STT khám tại BV
                        SearchExaminationIndex searchExaminationIndex = new SearchExaminationIndex()
                        {
                            HospitalId = item.HospitalId ?? 0,
                            ServiceTypeId = item.ServiceTypeId,
                            ExaminationDate = item.ExaminationDate
                        };
                        if (string.IsNullOrEmpty(item.ExaminationIndex))
                        {
                            item.ExaminationIndex = await this.GetExaminationFormIndex(searchExaminationIndex);
                            item.ExaminationPaymentIndex = string.Empty;
                            indexString = item.ExaminationIndex;
                        }

                    }
                    //TH2: Thanh toán qua App => Lấy STT đóng tiền khám
                    //=> Cập nhật lại STT đóng tiền cho mẫu phiếu khám bệnh
                    else
                    {
                        // Lấy STT đóng tiền tại BV
                        SearchExaminationIndex searchExaminationIndex = new SearchExaminationIndex()
                        {
                            HospitalId = item.HospitalId ?? 0,
                            ServiceTypeId = item.ServiceTypeId,
                            ExaminationDate = item.ExaminationDate
                        };
                        if (string.IsNullOrEmpty(item.ExaminationPaymentIndex))
                        {
                            item.ExaminationPaymentIndex = await this.GetExaminationFormPaymentIndex(searchExaminationIndex);
                            item.ExaminationIndex = string.Empty;
                            indexString = item.ExaminationPaymentIndex;
                        }
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Tạo lịch sử phiếu khám bệnh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task CreateExaminationHistory(ExaminationForms item, string createdBy)
        {
            int action = 0;
            switch (item.Status)
            {
                case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                    action = (int)CatalogueUtilities.ExaminationAction.Cancel;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                    {
                        action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                        // Tạo lịch sử thanh toán
                        // Lưu lại thông tin số thứ tự
                        await CreatePaymentHistories(item, createdBy);
                    }
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                    {
                        action = (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination;
                        // Tạo lịch sử thanh toán
                        // Lưu lại thông tin số thứ tự
                        await CreatePaymentHistories(item, createdBy);
                    }
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.New:
                    action = (int)CatalogueUtilities.ExaminationAction.Create;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.WaitConfirm:
                    action = (int)CatalogueUtilities.ExaminationAction.Create;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.WaitReExamination:
                    action = (int)CatalogueUtilities.ExaminationAction.Update;
                    break;
                default:
                    break;
            }

            // Tạo lịch sử tạo phiếu khám bệnh
            ExaminationHistories examinationHistories = new ExaminationHistories()
            {
                Created = DateTime.Now,
                CreatedBy = createdBy,
                Active = true,
                Deleted = false,
                ExaminationFormId = item.Id,
                Status = item.Status,
                Action = action,
                Comment = item.Comment,
                Note = item.Note,
            };
            await unitOfWork.Repository<ExaminationHistories>().CreateAsync(examinationHistories);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Lưu thông tin lịch sử thanh toán + cập nhật số thứ tự cho người dùng
        /// </summary>
        /// <param name="examinationForms"></param>
        /// <returns></returns>
        private async Task CreatePaymentHistories(ExaminationForms examinationForms, string createdBy)
        {
            if (examinationForms.PaymentMethodId.HasValue && examinationForms.PaymentMethodId.Value > 0)
            {
                var paymentMethodInfo = await unitOfWork.Repository<PaymentMethods>().GetQueryable().Where(e => !e.Deleted && e.Active
                && e.Id == examinationForms.PaymentMethodId).FirstOrDefaultAsync();
                if (paymentMethodInfo != null)
                {
                    // Lưu lịch sử thanh toán
                    PaymentHistories paymentHistories = new PaymentHistories();
                    //TH1: Thanh toán COD => Lấy STT tại phòng khám
                    //=> Cập nhật lại STT tại phòng khám cho mẫu phiếu khám bệnh
                    if (paymentMethodInfo.Code == CatalogueUtilities.PaymentMethod.COD.ToString())
                    {
                        // Lưu lịch sử thanh toán
                        paymentHistories = new PaymentHistories()
                        {
                            Created = DateTime.Now,
                            Active = true,
                            Deleted = false,
                            CreatedBy = createdBy,
                            ExaminationFee = examinationForms.Price,
                            ServiceFee = examinationForms.FeeExamination,
                            ExaminationFormId = examinationForms.Id,
                            PaymentMethodId = examinationForms.PaymentMethodId.Value,
                            PaymentMethodName = paymentMethodInfo.Name
                        };
                        await unitOfWork.Repository<PaymentHistories>().CreateAsync(paymentHistories);
                        await unitOfWork.SaveAsync();
                    }
                    //TH2: Thanh toán qua App => Lấy STT đóng tiền khám
                    //=> Cập nhật lại STT đóng tiền cho mẫu phiếu khám bệnh
                    else
                    {

                        BankInfos bankInfos = null;
                        if (examinationForms.BankInfoId.HasValue && examinationForms.BankInfoId.Value > 0)
                        {
                            bankInfos = await unitOfWork.Repository<BankInfos>().GetQueryable().Where(e => !e.Deleted && e.Active && e.Id == examinationForms.BankInfoId.Value).FirstOrDefaultAsync();
                        }

                        // Lưu lịch sử thanh toán
                        paymentHistories = new PaymentHistories()
                        {
                            Created = DateTime.Now,
                            Active = true,
                            Deleted = false,
                            CreatedBy = createdBy,
                            ExaminationFee = examinationForms.Price,
                            ServiceFee = examinationForms.FeeExamination,
                            ExaminationFormId = examinationForms.Id,
                            PaymentMethodId = examinationForms.PaymentMethodId.Value,
                            PaymentMethodName = paymentMethodInfo.Name,
                            BankInfoId = examinationForms.BankInfoId,
                            BankInfo = bankInfos != null ? string.Format("STK: {0} - {1}", bankInfos.BankNo, bankInfos.BankBranch) : string.Empty
                        };
                        await unitOfWork.Repository<PaymentHistories>().CreateAsync(paymentHistories);
                        await unitOfWork.SaveAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Kiểm tra trùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(ExaminationForms item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;

            bool isExistDetailSchedule = await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.ExaminationScheduleDetailId == item.ExaminationScheduleDetailId);
            if (isExistDetailSchedule)
                messages.Add("Đã tồn tại phiếu khám với thời gian này!");

            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

        /// <summary>
        /// Lấy STT mới nhất của dịch vụ bệnh viện tương ứng
        /// </summary>
        /// <param name="searchExaminationIndex"></param>
        /// <returns></returns>
        public async Task<string> GetExaminationFormIndex(SearchExaminationIndex searchExaminationIndex)
        {
            string indexString = string.Empty;
            int index = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchExaminationIndex.HospitalId),
                new SqlParameter("@ServiceTypeId", searchExaminationIndex.ServiceTypeId),
                new SqlParameter("@ExaminationDate", searchExaminationIndex.ExaminationDate),
                new SqlParameter("@ExaminationIndex", SqlDbType.Int, 0),

            };
            var obj = await this.unitOfWork.Repository<ExaminationForms>().ExcuteStoreGetValue("Index_GetIExaminationIndex", parameters, "@ExaminationIndex");
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
        /// Lấy STT chờ khám
        /// </summary>
        /// <param name="searchExaminationIndex"></param>
        /// <returns></returns>
        public async Task<string> GetExaminationFormPaymentIndex(SearchExaminationIndex searchExaminationIndex)
        {
            string indexString = string.Empty;
            int index = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchExaminationIndex.HospitalId),
                new SqlParameter("@ServiceTypeId", searchExaminationIndex.ServiceTypeId),
                new SqlParameter("@ExaminationDate", searchExaminationIndex.ExaminationDate),
                new SqlParameter("@ExaminationIndex", SqlDbType.Int, 0),

            };
            var obj = await this.unitOfWork.Repository<ExaminationForms>().ExcuteStoreGetValue("Index_GetIExaminationPaymentIndex", parameters, "@ExaminationIndex");
            indexString = obj.ToString();
            if (!string.IsNullOrEmpty(indexString))
            {
                if (int.TryParse(indexString, out index))
                    index += 1;
            }
            indexString = index.ToString("D3");
            return indexString;
        }


    }
}
