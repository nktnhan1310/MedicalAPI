using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
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
    public class UserFileService : DomainService<UserFiles, SearchUserFile>, IUserFileService
    {
        public UserFileService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
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
        /// Cập nhật folder cho file
        /// </summary>
        /// <param name="userFileIds"></param>
        /// <param name="userFolderId"></param>
        /// <param name="userId"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public async Task<bool> UpdateFolderForFile(List<int> userFileIds, int userFolderId)
        {
            using (var contextTransacion = await medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existUserFiles = await this.unitOfWork.Repository<UserFiles>().GetQueryable()
                        .Where(e => e.UserId == LoginContext.Instance.CurrentUser.UserId && userFileIds.Contains(e.Id)).ToListAsync();
                    if (existUserFiles == null || !existUserFiles.Any()) throw new AppException("Thông tin file không hợp lệ");
                    foreach (var existUserFile in existUserFiles)
                    {
                        existUserFile.Updated = DateTime.Now;
                        existUserFile.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                        existUserFile.UserId = LoginContext.Instance.CurrentUser.UserId;
                        existUserFile.FolderId = userFolderId;
                        Expression<Func<UserFiles, object>>[] includeProperties = new Expression<Func<UserFiles, object>>[]
                           {
                        e => e.Updated,
                        e => e.UpdatedBy,
                        e => e.UserId,
                        e => e.FolderId
                           };
                        this.unitOfWork.Repository<UserFiles>().UpdateFieldsSave(existUserFile, includeProperties);
                    }

                    await this.unitOfWork.SaveAsync();
                    await contextTransacion.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    contextTransacion.Rollback();
                    return false;
                }
            }
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
