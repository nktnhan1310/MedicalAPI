using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Medical.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class AppPolicyService : CoreHospitalService<AppPolicies, SearchAppPolicy>, IAppPolicyService
    {
        public AppPolicyService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }

        /// <summary>
        /// Thêm mới thông tin chính sách
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(AppPolicies item)
        {
            if (item == null) throw new AppException("Thông tin item không hợp lệ");
            using (var contextTransactionTask = medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this.unitOfWork.Repository<AppPolicies>().Create(item);
                    await this.unitOfWork.SaveAsync();
                    if (item.AppPolicyDetails != null && item.AppPolicyDetails.Any())
                    {
                        foreach (var detail in item.AppPolicyDetails)
                        {
                            detail.Created = DateTime.Now;
                            detail.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                            detail.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
                        }
                    }

                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin chính sách
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(AppPolicies item)
        {
            // Lấy thông tin chính sách
            var existItem = await this.unitOfWork.Repository<AppPolicies>().GetQueryable()
                .Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem == null) throw new AppException("Item không tồn tại");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    item.Updated = DateTime.Now;
                    item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    existItem = mapper.Map<AppPolicies>(item);
                    this.unitOfWork.Repository<AppPolicies>().Update(existItem);
                    // Cập nhật chi tiết chính sách
                    if (item.AppPolicyDetails != null && item.AppPolicyDetails.Any())
                    {
                        foreach (var appPolicyDetail in item.AppPolicyDetails)
                        {
                            var existAppPolicyDetail = await this.unitOfWork.Repository<AppPolicyDetails>().GetQueryable()
                                .Where(e => e.Id == appPolicyDetail.Id).FirstOrDefaultAsync();
                            if (existAppPolicyDetail == null)
                            {
                                appPolicyDetail.Created = DateTime.Now;
                                appPolicyDetail.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                appPolicyDetail.AppPolicyId = existItem.Id;
                                this.unitOfWork.Repository<AppPolicyDetails>().Create(appPolicyDetail);
                            }
                            else
                            {
                                existAppPolicyDetail = mapper.Map<AppPolicyDetails>(appPolicyDetail);
                                existAppPolicyDetail.Updated = DateTime.Now;
                                existAppPolicyDetail.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                                this.unitOfWork.Repository<AppPolicyDetails>().Update(existAppPolicyDetail);
                            }
                        }
                    }
                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }


        }

        /// <summary>
        /// Lấy thông tin danh sách phân trang của chính sách
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<AppPolicies>> GetPagedListData(SearchAppPolicy baseSearch)
        {
            PagedList<AppPolicies> pagedList = new PagedList<AppPolicies>();

            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;

            var items = Queryable.Where(e => !e.Deleted 
            && (!baseSearch.HospitalId.HasValue || e.HospitalId == baseSearch.HospitalId.Value)
            && (!baseSearch.TypeId.HasValue || e.TypeId == baseSearch.TypeId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent) || e.Title.ToLower().Contains(baseSearch.SearchContent.ToLower())
            || e.Content.ToLower().Contains(baseSearch.SearchContent.ToLower())
            )
            );
            decimal itemCount = items.Count();
            pagedList = new PagedList<AppPolicies>()
            {
                TotalItem = (int)itemCount,
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
            };
            return pagedList;
        }

        public override async Task<string> GetExistItemMessage(AppPolicies item)
        {
            string result = string.Empty;
            var isExistPolicyTask = this.unitOfWork.Repository<AppPolicies>().GetQueryable()
                .AnyAsync(e => e.Id != item.Id 
                && e.TypeId == item.TypeId
                && ((!item.HospitalId.HasValue && item.HospitalId.Value > 0) || e.HospitalId == item.HospitalId)
                );
            if (await isExistPolicyTask)
                result = "Thông tin chính sách đã tồn tại";
            return result;
        }

    }
}
