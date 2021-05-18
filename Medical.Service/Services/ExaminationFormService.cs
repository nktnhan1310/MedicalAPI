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

                    switch (updateExaminationStatus.Status)
                    {
                        // Nếu trạng thái: đã xác nhận => lưu lại thông tin mã phiếu khám + lịch sử phiếu
                        case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                            {
                                action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                                existExaminationFormInfo.Code = RandomUtilities.RandomString(6);
                                includeProperties = new Expression<Func<ExaminationForms, object>>[]
                                {
                                    x => x.Status,
                                    x => x.Updated,
                                    x => x.UpdatedBy,
                                    x => x.Code,
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
                await CreateExaminationHistory(updateExaminationStatus.ExaminationFormId, updateExaminationStatus.Status.Value, updateExaminationStatus.CreatedBy, updateExaminationStatus.Comment);
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
                await unitOfWork.Repository<ExaminationForms>().CreateAsync(item);
                await unitOfWork.SaveAsync();

                // Tạo lịch sử tạo phiếu khám bệnh
                await CreateExaminationHistory(item.Id, item.Status, item.CreatedBy, item.Comment);
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
                    existItem = mapper.Map<ExaminationForms>(item);
                    await unitOfWork.SaveAsync();

                    // Tạo lịch sử tạo phiếu khám bệnh
                    await CreateExaminationHistory(item.Id, item.Status, item.CreatedBy, item.Comment);
                }
            }

            return result;
        }

        /// <summary>
        /// Tạo lịch sử phiếu khám bệnh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task CreateExaminationHistory(int examinationFormId, int status, string createdBy, string comment)
        {
            int action = 0;
            switch (status)
            {
                case (int)CatalogueUtilities.ExaminationStatus.Canceled:
                    action = (int)CatalogueUtilities.ExaminationAction.Cancel;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.Confirmed:
                    action = (int)CatalogueUtilities.ExaminationAction.Confirm;
                    break;
                case (int)CatalogueUtilities.ExaminationStatus.ConfirmedReExamination:
                    action = (int)CatalogueUtilities.ExaminationAction.ConfirmReExamination;
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
                ExaminationFormId = examinationFormId,
                Status = status,
                Action = action,
                Comment = comment
            };
            await unitOfWork.Repository<ExaminationHistories>().CreateAsync(examinationHistories);
            await unitOfWork.SaveAsync();
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

    }
}
