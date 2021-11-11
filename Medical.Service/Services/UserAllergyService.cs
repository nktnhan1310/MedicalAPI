using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface;
using Medical.Interface.DbContext;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class UserAllergyService : DomainService<UserAllergies, SearchUserAllergy>, IUserAllergyService
    {
        public UserAllergyService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }

        /// <summary>
        /// Thêm mới dị ứng cho user
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(UserAllergies item)
        {
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    item.Created = DateTime.Now;
                    item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                    this.unitOfWork.Repository<UserAllergies>().Create(item);
                    await this.unitOfWork.SaveAsync();

                    // Thêm mới mô tả cho dị ứng của user
                    if (item.DescriptionTypeIds != null && item.DescriptionTypeIds.Any())
                    {
                        foreach (var descriptionTypeId in item.DescriptionTypeIds)
                        {
                            UserAllergyDetails userAllergyDetail = new UserAllergyDetails()
                            {
                                Deleted = false,
                                Active = true,
                                DescriptionTypeId = descriptionTypeId,
                                UserAllergyId = item.Id,
                                Created = DateTime.Now,
                                CreatedBy = LoginContext.Instance.CurrentUser.UserName
                            };
                            this.unitOfWork.Repository<UserAllergyDetails>().Create(userAllergyDetail);
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
        /// Cập nhật thông tin dị ứng của user
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(UserAllergies item)
        {
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    
                    var existItem = await this.unitOfWork.Repository<UserAllergies>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
                    if (existItem == null) throw new AppException("Không tìm thấy thông tin item");
                    existItem = mapper.Map<UserAllergies>(item);
                    existItem.Updated = DateTime.Now;
                    existItem.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    this.unitOfWork.Repository<UserAllergies>().Update(existItem);

                    // Cập nhật thông tin mô tả ghi chú của user
                    if (item.DescriptionTypeIds != null && item.DescriptionTypeIds.Any())
                    {
                        foreach (var descriptionTypeId in item.DescriptionTypeIds)
                        {
                            var existDescriptionTypeInfo = await this.unitOfWork.Repository<UserAllergyDetails>().GetQueryable()
                                .Where(e => e.DescriptionTypeId == descriptionTypeId && e.UserAllergyId == item.Id).FirstOrDefaultAsync();
                            if (existDescriptionTypeInfo != null)
                            {
                                existDescriptionTypeInfo.Updated = DateTime.Now;
                                existDescriptionTypeInfo.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                                Expression<Func<UserAllergyDetails, object>>[] includeProperties = new Expression<Func<UserAllergyDetails, object>>[]
                                {
                                    e => e.Updated,
                                    e => e.UpdatedBy
                                };
                                this.unitOfWork.Repository<UserAllergyDetails>().UpdateFieldsSave(existDescriptionTypeInfo, includeProperties);
                            }
                            else
                            {
                                UserAllergyDetails userAllergyDetail = new UserAllergyDetails()
                                {
                                    Deleted = false,
                                    Active = true,
                                    Created = DateTime.Now,
                                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                    DescriptionTypeId = descriptionTypeId,
                                    UserAllergyId = item.Id,
                                };
                                this.unitOfWork.Repository<UserAllergyDetails>().Create(userAllergyDetail);
                            }
                        }

                        // Lấy ra tất cả item còn tồn tại dưới db => xóa
                        var currentExistItems = await this.unitOfWork.Repository<UserAllergyDetails>().GetQueryable()
                            .Where(e => !e.Deleted && !item.DescriptionTypeIds.Contains(e.DescriptionTypeId.Value)).ToListAsync();
                        if(currentExistItems != null && currentExistItems.Any())
                        {
                            foreach (var currentExistItem in currentExistItems)
                            {
                                this.unitOfWork.Repository<UserAllergyDetails>().Delete(currentExistItem);
                            }
                        }

                    }
                    else
                    {
                        // Lấy hết tất cả mô tả dưới db và xóa
                        var currentUserAllergyDetails = await this.unitOfWork.Repository<UserAllergyDetails>().GetQueryable()
                            .Where(e => e.UserAllergyId == existItem.Id).ToListAsync();
                        if (currentUserAllergyDetails != null && currentUserAllergyDetails.Any())
                        {
                            foreach (var currentUserAllergyDetail in currentUserAllergyDetails)
                            {
                                this.unitOfWork.Repository<UserAllergyDetails>().Delete(currentUserAllergyDetail);
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
        /// Lấy danh sách phân trang nhóm dị ứng của người dùng
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<UserAllergies>> GetPagedListData(SearchUserAllergy baseSearch)
        {
            PagedList<UserAllergies> pagedList = new PagedList<UserAllergies>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;
            var items = Queryable.Where(e => !e.Deleted && e.Active
            && (!e.UserId.HasValue || !baseSearch.UserId.HasValue || e.UserId == baseSearch.UserId.Value)
            && (!baseSearch.AllergyTypeId.HasValue || e.AllergyTypeId == baseSearch.AllergyTypeId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent) || (e.Description.ToLower().Contains(baseSearch.SearchContent.ToLower())
            ))
            );
            decimal itemCount = items.Count();
            pagedList = new PagedList<UserAllergies>()
            {
                TotalItem = (int)itemCount,
                Items = await items.OrderBy(e => e.AllergyTypeId).ThenBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
            };

            if (pagedList.Items != null && pagedList.Items.Any())
            {
                foreach (var item in pagedList.Items)
                {
                    var allergyTypeInfo = await this.unitOfWork.Repository<AllergyTypes>().GetQueryable().Where(e => e.Id == item.AllergyTypeId).FirstOrDefaultAsync();
                    if (allergyTypeInfo != null) item.AllergyTypeName = allergyTypeInfo.Name;

                    var userInfo = await this.unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == item.UserId).FirstOrDefaultAsync();
                    if (userInfo != null) item.UserFullName = userInfo.LastName + " " + userInfo.FirstName;
                }
            }
            else
            {
                // LẤY RA TẤT CẢ NHÓM DỊ ỨNG TẠO CHO USER
                using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var allergyTypeInfos = await this.unitOfWork.Repository<AllergyTypes>().GetQueryable().Where(e => !e.Deleted).ToListAsync();
                        if (allergyTypeInfos != null && allergyTypeInfos.Any())
                        {
                            foreach (var allergyTypeInfo in allergyTypeInfos)
                            {
                                UserAllergies userAllergies = new UserAllergies()
                                {
                                    UserId = LoginContext.Instance.CurrentUser.UserId,
                                    Created = DateTime.Now,
                                    CreatedBy = LoginContext.Instance.CurrentUser.UserName,
                                    Active = true,
                                    Deleted = false,
                                    AllergyTypeId = allergyTypeInfo.Id,
                                };
                                this.unitOfWork.Repository<UserAllergies>().Create(userAllergies);
                            }
                        }
                        await this.unitOfWork.SaveAsync();
                        var contextTransaction = await contextTransactionTask;
                        await contextTransaction.CommitAsync();
                        // Load lại thông tin danh sách
                        pagedList = await this.GetPagedListData(baseSearch);
                    }
                    catch (Exception)
                    {
                        var contextTransaction = await contextTransactionTask;
                        contextTransaction.Rollback();
                        return pagedList;
                    }
                }

            }
            return pagedList;
        }

        public override async Task<string> GetExistItemMessage(UserAllergies item)
        {
            List<string> messages = new List<string>();

            // KIỂM TRA NHÓM DỊ ỨNG PHẢI NHÓM DỊ ỨNG KHÁC KHÔNG?
            // NHÓM DỊ ỨNG KHÁC => KIỂM TRA TÊN DỊ ỨNG ĐÃ TỒN TẠI CHƯA
            // Lấy ra thông tin nhóm dị ứng
            var allergyTypeInfo = await this.unitOfWork.Repository<AllergyTypes>().GetQueryable()
                .Where(e => e.Id == item.AllergyTypeId).FirstOrDefaultAsync();
            if (allergyTypeInfo != null && allergyTypeInfo.Code == CatalogueUtilities.AllergyType.OTHER.ToString()
                && !string.IsNullOrEmpty(item.OtherName))
            {
                bool isExistAllergyType = await this.unitOfWork.Repository<UserAllergies>().GetQueryable()
                .AnyAsync(e => !e.Deleted && e.AllergyTypeId == item.AllergyTypeId
                && e.OtherName.ToLower() == item.OtherName.ToLower() && e.Id != item.Id);
                if (isExistAllergyType) return "Nhóm dị ứng đã tồn tại";
            }
            bool isExistItem = await this.unitOfWork.Repository<UserAllergies>().GetQueryable()
                .AnyAsync(e => !e.Deleted && e.AllergyTypeId == item.AllergyTypeId && e.Id != item.Id);
            if (isExistItem) return "Nhóm dị ứng đã tồn tại";

            string result = string.Empty;
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }
    }
}
