using AutoMapper;
using Medical.Entities;
using Medical.Interface;
using Medical.Interface.UnitOfWork;
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
    public class UserAllergyService : DomainService<UserAllergies, SearchUserAllergy>, IUserAllergyService
    {
        public UserAllergyService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
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
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
            };

            if(pagedList.Items != null && pagedList.Items.Any())
            {
                foreach (var item in pagedList.Items)
                {
                    var allergyTypeInfo = await this.unitOfWork.Repository<AllergyTypes>().GetQueryable().Where(e => e.Id == item.AllergyTypeId).FirstOrDefaultAsync();
                    if (allergyTypeInfo != null) item.AllergyTypeName = allergyTypeInfo.Name;

                    var userInfo = await this.unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == item.UserId).FirstOrDefaultAsync();
                    if (userInfo != null) item.UserFullName = userInfo.LastName + " " + userInfo.FirstName;
                }
            }

            return pagedList;
        }


    }
}
