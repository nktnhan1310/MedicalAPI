using AutoMapper;
using Medical.Entities;
using Medical.Entities.Search;
<<<<<<< HEAD
using Medical.Extensions;
using Medical.Interface.DbContext;
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
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

namespace Medical.Service.Services
{
    public class UserFolderService : DomainService<UserFolders, SearchUserFolder>, IUserFolderService
    {
<<<<<<< HEAD
        public UserFolderService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
=======
        public UserFolderService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        {
        }

        /// <summary>
        /// Lấy danh sách folder của user
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<UserFolders>> GetPagedListData(SearchUserFolder baseSearch)
        {
            PagedList<UserFolders> pagedList = new PagedList<UserFolders>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;
            var items = Queryable.Where(e => !e.Deleted && e.Active
            && (!e.UserId.HasValue || !baseSearch.UserId.HasValue || e.UserId == baseSearch.UserId.Value)
            && (!baseSearch.TypeId.HasValue || e.TypeId == baseSearch.TypeId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent) || (e.FolderName.ToLower().Contains(baseSearch.SearchContent.ToLower())
            ))
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
            pagedList = new PagedList<UserFolders>()
            {
                TotalItem = (int)itemCount,
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
            };
            // Lấy tổng số hình ảnh trong folder
<<<<<<< HEAD
            if (pagedList != null && pagedList.Items.Any())
=======
            if(pagedList != null && pagedList.Items.Any())
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            {
                foreach (var item in pagedList.Items)
                {
                    item.TotalImageInFolder = await this.unitOfWork.Repository<UserFiles>().GetQueryable().Where(e => !e.Deleted && e.Active && e.FolderId == item.Id).CountAsync();
                }
            }

            return pagedList;
        }

        /// <summary>
        /// Lấy danh sách folder theo tháng/năm
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public async Task<PagedList<UserFolderExtensions>> GetPagedListExtension(SearchUserFolder baseSearch)
        {
            PagedList<UserFolderExtensions> pagedList = new PagedList<UserFolderExtensions>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;
            var items = Queryable.Where(e => !e.Deleted && e.Active
            && (!e.UserId.HasValue || !baseSearch.UserId.HasValue || e.UserId == baseSearch.UserId.Value)
            && (!baseSearch.TypeId.HasValue || e.TypeId == baseSearch.TypeId.Value)
            && (string.IsNullOrEmpty(baseSearch.SearchContent) || (e.FolderName.ToLower().Contains(baseSearch.SearchContent.ToLower())
            ))
            );
            IList<UserFolders> userFolders = new List<UserFolders>();
            IList<UserFolderExtensions> userFolderExtensions = new List<UserFolderExtensions>();
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

                        userFolders = await items.ToListAsync();

                        userFolderExtensions = userFolders.GroupBy(e => new
                        {
                            e.Created.Month,
                            e.Created.Year
                        })
                        .Select(e => new UserFolderExtensions()
                        {
                            Month = e.Key.Month,
                            Year = e.Key.Year,
                            UserFolders = e.ToList()
                        }).OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ToList();
<<<<<<< HEAD

=======
                            
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608

                        break;
                    case 1:
                        items = items.Where(e =>
                        (!baseSearch.Year.HasValue || e.Created.Year == baseSearch.Year.Value)
                        );
                        userFolders = await items.ToListAsync();

                        userFolderExtensions = userFolders.GroupBy(e => new
                        {
                            e.Created.Year
                        })
                        .Select(e => new UserFolderExtensions()
                        {
                            Year = e.Key.Year,
                            UserFolders = e.ToList()
                        }).OrderByDescending(e => e.Year).ToList();

                        break;
                    default:
                        break;
                }
            }

            decimal itemCount = userFolderExtensions.Count();
            pagedList = new PagedList<UserFolderExtensions>()
            {
                TotalItem = (int)itemCount,
                Items = userFolderExtensions,
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
            };
            // Lấy tổng số hình ảnh trong folder
            if (pagedList != null && pagedList.Items.Any())
            {
                foreach (var item in pagedList.Items)
                {
<<<<<<< HEAD
                    if (item.UserFolders != null && item.UserFolders.Any())
=======
                    if(item.UserFolders != null && item.UserFolders.Any())
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                    {
                        foreach (var userFolder in item.UserFolders)
                        {
                            userFolder.TotalImageInFolder = await this.unitOfWork.Repository<UserFiles>().GetQueryable().Where(e => !e.Deleted && e.Active && e.FolderId == userFolder.Id).CountAsync();
                        }
                    }
                }
            }

            return pagedList;
        }

        public override async Task<bool> CreateAsync(UserFolders item)
        {
<<<<<<< HEAD
            if (item == null) throw new AppException("Vui lòng chọn thông tin item");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await this.unitOfWork.Repository<UserFolders>().CreateAsync(item);
                    await this.unitOfWork.SaveAsync();
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            file.FolderId = item.Id;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(file);
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
           
=======
            bool result = false;
            if (item != null)
            {
                await this.unitOfWork.Repository<UserFolders>().CreateAsync(item);
                await this.unitOfWork.SaveAsync();
                if (item.UserFiles != null && item.UserFiles.Any())
                {
                    foreach (var file in item.UserFiles)
                    {
                        file.FolderId = item.Id;
                        await this.unitOfWork.Repository<UserFiles>().CreateAsync(file);
                    }
                }
                await this.unitOfWork.SaveAsync();
                result = true;
            }
            return result;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }

        public override async Task<bool> UpdateAsync(UserFolders item)
        {
<<<<<<< HEAD
            var existFolders = await this.unitOfWork.Repository<UserFolders>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existFolders == null) throw new AppException("Thông tin item không tồn tại");

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    existFolders = mapper.Map<UserFolders>(item);
                    this.unitOfWork.Repository<UserFolders>().Update(existFolders);

                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            var existUserFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable().Where(e => e.Id == file.Id).FirstOrDefaultAsync();
                            if (existUserFile == null)
                            {
                                file.FolderId = item.Id;
                                await this.unitOfWork.Repository<UserFiles>().CreateAsync(file);
                            }
                            else
                            {
                                existUserFile = mapper.Map<UserFiles>(file);
                                existUserFile.FolderId = item.Id;
                                this.unitOfWork.Repository<UserFiles>().Update(existUserFile);
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
=======
            bool result = false;
            var existFolders = await this.unitOfWork.Repository<UserFolders>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existFolders != null)
            {
                existFolders = mapper.Map<UserFolders>(item);
                this.unitOfWork.Repository<UserFolders>().Update(existFolders);

                if (item.UserFiles != null && item.UserFiles.Any())
                {
                    foreach (var file in item.UserFiles)
                    {
                        var existUserFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable().Where(e => e.Id == file.Id).FirstOrDefaultAsync();
                        if(existUserFile == null)
                        {
                            file.FolderId = item.Id;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(file);
                        }
                        else
                        {
                            existUserFile = mapper.Map<UserFiles>(file);
                            existUserFile.FolderId = item.Id;
                            this.unitOfWork.Repository<UserFiles>().Update(existUserFile);
                        }
                        
                    }
                }
                await this.unitOfWork.SaveAsync();
                result = true;
            }
            return result;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }

        public override async Task<string> GetExistItemMessage(UserFolders item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
<<<<<<< HEAD
            bool isExistFolderName = await Queryable.AnyAsync(e => !e.Deleted && e.Active
=======
            bool isExistFolderName = await Queryable.AnyAsync(e => !e.Deleted && e.Active 
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            && e.FolderName.ToLower().Contains(item.FolderName.ToLower())
             && e.Id != item.Id);
            if (isExistFolderName)
                result = "Tên folder đã tồn tại";
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

    }
}
