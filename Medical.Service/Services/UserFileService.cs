using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
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
    public class UserFileService : DomainService<UserFiles, SearchUserFile>, IUserFileService
    {
        public UserFileService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Lấy danh sách phân trang file trong folder
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<UserFiles>> GetPagedListData(SearchUserFile baseSearch)
        {
            PagedList<UserFiles> pagedList = new PagedList<UserFiles>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;

            var items = Queryable.Where(e => !e.Deleted && e.Active
            && (!baseSearch.UserId.HasValue || e.UserId == baseSearch.UserId)
            && (!baseSearch.FolderId.HasValue || e.FolderId == baseSearch.FolderId)
            && (!baseSearch.TypeId.HasValue || e.FileType == baseSearch.TypeId)
            );
            // Filter theo tháng + năm
            if (baseSearch.FilterType.HasValue)
            {
                switch (baseSearch.FilterType.Value)
                {
                    case 0:
                        items = items.Where(e =>
                        (!baseSearch.Month.HasValue || e.Created.Month == baseSearch.Month.Value)
                        && (!baseSearch.Year.HasValue || e.Created.Year == baseSearch.Year.Value)
                        );
                        break;
                    case 1:
                        items = items.Where(e =>
                        (!baseSearch.Year.HasValue || e.Created.Year == baseSearch.Year.Value)
                        );
                        break;
                    default:
                        break;
                }
            }
            decimal itemCount = items.Count();
            pagedList = new PagedList<UserFiles>()
            {
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
                TotalItem = (int)itemCount,
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
            };
            return pagedList;
        }

        /// <summary>
        /// Lấy danh sách file filter theo tháng hoặc năm
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public async Task<PagedList<UserFileExtensions>> GetPagedListExtension(SearchUserFile baseSearch)
        {
            PagedList<UserFileExtensions> pagedList = new PagedList<UserFileExtensions>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;

            var items = Queryable.Where(e => !e.Deleted && e.Active
            && (!baseSearch.UserId.HasValue || e.UserId == baseSearch.UserId)
            && (!baseSearch.FolderId.HasValue || e.FolderId == baseSearch.FolderId)
            && (!baseSearch.TypeId.HasValue || e.FileType == baseSearch.TypeId)
            );

            IList<UserFiles> resultList = new List<UserFiles>();

            IList<UserFileExtensions> userFileExtensions = new List<UserFileExtensions>();
            
           

            // Filter theo tháng + năm
            if (baseSearch.FilterType.HasValue)
            {
                switch (baseSearch.FilterType.Value)
                {
                    case 0:
                        items = items.Where(e =>
                        (!baseSearch.Month.HasValue || e.Created.Month == baseSearch.Month.Value)
                        && (!baseSearch.Year.HasValue || e.Created.Year == baseSearch.Year.Value)
                        );
                        resultList = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(take).ToListAsync();
                        userFileExtensions = resultList.GroupBy(e => new
                        {
                            e.Created.Month,
                            e.Created.Year
                        })
                        .Select(e => new UserFileExtensions()
                        {
                            Month = e.Key.Month,
                            Year = e.Key.Year,
                            UserFiles = e.ToList()
                        }).ToList();
                        userFileExtensions = userFileExtensions.OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ToList();
                        break;
                    case 1:
                        items = items.Where(e =>
                        (!baseSearch.Year.HasValue || e.Created.Year == baseSearch.Year.Value)
                        );
                        resultList = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(take).ToListAsync();

                        userFileExtensions = resultList.GroupBy(e => new
                        {
                            e.Created.Year
                        })
                        .Select(e => new UserFileExtensions()
                        {
                            Year = e.Key.Year,
                            UserFiles = e.ToList()
                        }).ToList();
                        userFileExtensions = userFileExtensions.OrderByDescending(e => e.Year).ToList();
                        break;
                    default:
                        {
                            resultList = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(take).ToListAsync();
                            UserFileExtensions userFileExtension = new UserFileExtensions()
                            {
                                UserFiles = resultList
                            };
                            userFileExtensions.Add(userFileExtension);
                        }
                        break;
                }
            }
            else
            {
                resultList = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(take).ToListAsync();
                UserFileExtensions userFileExtension = new UserFileExtensions()
                {
                    UserFiles = resultList
                };
                userFileExtensions.Add(userFileExtension);
            }
            decimal itemCount = items.Count();
            pagedList = new PagedList<UserFileExtensions>()
            {
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
                TotalItem = (int)itemCount,
                Items = userFileExtensions.ToList()
            };


            return pagedList;
        }
    }
}
