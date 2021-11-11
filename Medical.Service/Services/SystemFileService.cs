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
    public class SystemFileService : DomainService<SystemFiles, SearchSystemFile>, ISystemFileService
    {
        public SystemFileService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Lấy thông tin của systemfile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<SystemFiles> GetByIdAsync(int id)
        {
            var item = await Queryable.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if(item.HospitalId.HasValue && item.HospitalId.Value > 0)
            {
                var hospitalInfo = await this.unitOfWork.Repository<Hospitals>().GetQueryable()
                    .Where(e => e.Id == item.HospitalId.Value).FirstOrDefaultAsync();
                if (hospitalInfo != null) item.HospitalName = hospitalInfo.Name;
            }

            return item;
        }

        /// <summary>
        /// Lấy danh sách phân trang thông tin file
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<SystemFiles>> GetPagedListData(SearchSystemFile baseSearch)
        {
            PagedList<SystemFiles> pagedList = new PagedList<SystemFiles>();

            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;

            var items = this.Queryable.Where(e => !e.Deleted && e.Active
            && (!baseSearch.TypeId.HasValue || e.TypeId == baseSearch.TypeId.Value)
            && (!baseSearch.HospitalId.HasValue || e.HospitalId == baseSearch.HospitalId.Value)
            && (!baseSearch.SystemAdvertisementId.HasValue || e.SystemAdvertisementId == baseSearch.SystemAdvertisementId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent) ||
            ((e.Title.ToLower().Contains(baseSearch.SearchContent.ToLower()))
            || e.Description.ToLower().Contains(baseSearch.SearchContent.ToLower())
            || e.FileName.ToLower().Contains(baseSearch.SearchContent.ToLower())
            ))
            );
            decimal itemCount = items.Count();
            pagedList = new PagedList<SystemFiles>()
            {
                TotalItem = (int)itemCount,
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
            };
            if (pagedList.Items != null && pagedList.Items.Any())
            {
                foreach (var item in pagedList.Items)
                {
                    if (!item.HospitalId.HasValue || item.HospitalId.Value <= 0) continue;
                    var hospitalInfo = await this.unitOfWork.Repository<Hospitals>().GetQueryable()
                        .Where(e => e.Id == item.HospitalId.Value).FirstOrDefaultAsync();
                    if (hospitalInfo != null) item.HospitalName = hospitalInfo.Name;
                }
            }


            return pagedList;
        }
    }
}
