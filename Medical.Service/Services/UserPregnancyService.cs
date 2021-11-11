using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
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
    public class UserPregnancyService : DomainService<UserPregnancies, SearchUserPregnancy>, IUserPregnancyService
    {
        public UserPregnancyService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "UserPregnancy_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchUserPregnancy baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@MedicalRecordId", baseSearch.MedicalRecordId),
                new SqlParameter("@TypeId", baseSearch.TypeId),

                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(UserPregnancies item)
        {
            bool result = false;
            if (item == null) throw new AppException("Không tìm thấy thông tin item");

            this.unitOfWork.Repository<UserPregnancies>().Create(item);
            await this.unitOfWork.SaveAsync();
            if (item.UserPregnancyDetails != null && item.UserPregnancyDetails.Any())
            {
                foreach (var detail in item.UserPregnancyDetails)
                {
                    detail.Created = DateTime.Now;
                    detail.CreatedBy = item.CreatedBy;
                    detail.UserPregnancyId = item.Id;
                    detail.Active = true;
                    this.unitOfWork.Repository<UserPregnancyDetails>().Create(detail);
                }
                await this.unitOfWork.SaveAsync();
            }
            result = true;
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(UserPregnancies item)
        {
            bool result = false;
            var existItem = await this.unitOfWork.Repository<UserPregnancies>().GetQueryable()
                .Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem != null)
            {
                existItem = mapper.Map<UserPregnancies>(item);
                if (item.UserPregnancyDetails != null && item.UserPregnancyDetails.Any())
                {
                    foreach (var detail in item.UserPregnancyDetails)
                    {
                        var existDetail = await this.unitOfWork.Repository<UserPregnancyDetails>().GetQueryable()
                            .Where(e => e.Id == detail.Id).FirstOrDefaultAsync();
                        if (existDetail != null)
                        {
                            existDetail = mapper.Map<UserPregnancyDetails>(detail);
                            detail.Updated = DateTime.Now;
                            detail.UpdatedBy = item.UpdatedBy;
                            detail.UserPregnancyId = item.Id;
                            detail.Active = true;

                            this.unitOfWork.Repository<UserPregnancyDetails>().Update(existDetail);
                        }
                        else
                        {
                            detail.Created = DateTime.Now;
                            detail.CreatedBy = item.UpdatedBy;
                            detail.UserPregnancyId = item.Id;
                            detail.Active = true;

                            this.unitOfWork.Repository<UserPregnancyDetails>().Create(detail);
                        }
                    }
                }
                await this.unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }
    }
}
