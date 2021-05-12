using AutoMapper;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Interface.Services.Base;
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

namespace Medical.Service.Services.DomainService
{
    public abstract class CatalogueService<E, T> : DomainService<E, T>, ICatalogueService<E, T> where E : MedicalCatalogueAppDomain where T : BaseSearch, new()
    {
        public CatalogueService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<bool> SaveAsync(E item)
        {
            var existCode = unitOfWork.CatalogueRepository<E>().GetQueryable()
                .AsNoTracking()
                .Where(e =>
                e.Code == item.Code
                && !e.Deleted)
                .FirstOrDefault();

            if (existCode != null && item.Id != existCode.Id)
            {
                throw new Exception("Mã đã tồn tại");
            }

            var exists = unitOfWork.CatalogueRepository<E>().GetQueryable()
                .AsNoTracking()
                .Where(e =>
                e.Id == item.Id
                && !e.Deleted)
                .FirstOrDefault();
            if (exists != null)
            {
                exists = mapper.Map<E>(item);
                unitOfWork.CatalogueRepository<E>().Update(exists);
            }
            else
            {
                await unitOfWork.CatalogueRepository<E>().CreateAsync(item);

            }
            await unitOfWork.SaveAsync();
            return true;

        }


        public virtual E GetByCode(string code)
        {
            return unitOfWork.CatalogueRepository<E>()
                .GetQueryable()
                .Where(e => e.Code == code && !e.Deleted)
                .FirstOrDefault();
        }

        /// <summary>
        /// Check trùng mã
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(E item)
        {
            return await Task.Run(() =>
            {
                string result = string.Empty;
                if (Queryable.Any(x => !x.Deleted && x.Id != item.Id && x.Code == item.Code))
                    return "Mã đã tồn tại!";
                return result;
            });
        }

        /// <summary>
        /// Lấy danh sách phân trang danh mục
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override Task<PagedList<E>> GetPagedListData(T baseSearch)
        {
            return Task.Run(() =>
            {
                PagedList<E> pagedList = new PagedList<E>();
                int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
                int take = baseSearch.PageSize;

                var items = Queryable.Where(GetExpression(baseSearch));
                decimal itemCount = items.Count();
                pagedList = new PagedList<E>()
                {
                    TotalItem = (int)itemCount,
                    Items = items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToList(),
                    PageIndex = baseSearch.PageIndex,
                    PageSize = baseSearch.PageSize,
                };
                return pagedList;
            });
        }

        protected virtual Expression<Func<E, bool>> GetExpression(T baseSearch)
        {
            return e => !e.Deleted 
            && (string.IsNullOrEmpty(baseSearch.SearchContent)
                || e.Code.Contains(baseSearch.SearchContent)
                || e.Name.Contains(baseSearch.SearchContent)
                || e.Description.Contains(baseSearch.SearchContent)
                );
        }

    }
}
