using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
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
    public class MedicalRecordHistoryService : DomainService<MedicalRecordHistories, SearchMedicalRecordHistory>, IMedicalRecordHistoryService
    {
        public MedicalRecordHistoryService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<MedicalRecordHistories>> GetPagedListData(SearchMedicalRecordHistory baseSearch)
        {
            PagedList<MedicalRecordHistories> pagedList = new PagedList<MedicalRecordHistories>();
            int skip = (baseSearch.PageIndex - 1) * baseSearch.PageSize;
            int take = baseSearch.PageSize;

            var items = Queryable.Where(e => !e.Deleted && e.Active
            && (!baseSearch.UserId.HasValue || e.UserId == baseSearch.UserId.Value)
            && (!baseSearch.Type.HasValue || e.Type == baseSearch.Type.Value)
            && (!baseSearch.MedicalRecordId.HasValue || e.MedicalRecordId == baseSearch.MedicalRecordId.Value)
            );
            decimal itemCount = items.Count();
            pagedList = new PagedList<MedicalRecordHistories>()
            {
                TotalItem = (int)itemCount,
                Items = await items.OrderBy(baseSearch.OrderBy).Skip(skip).Take(baseSearch.PageSize).ToListAsync(),
                PageIndex = baseSearch.PageIndex,
                PageSize = baseSearch.PageSize,
            };
            if (pagedList.Items != null && pagedList.Items.Any())
            {
                foreach (var item in pagedList.Items)
                {
                    var userInfo = await this.unitOfWork.Repository<Users>().GetQueryable().Where(e => !e.Deleted && e.Active && e.Id == item.UserId).FirstOrDefaultAsync();
                    if (userInfo != null)
                    {
                        item.UserFullName = userInfo.LastName + " " + userInfo.FirstName;
                        item.UserPhone = userInfo.Phone;
                        item.UserEmail = userInfo.Email;
                    }
                }
            }
            return pagedList;
        }

        /// <summary>
        /// Thêm mới tiền sử bệnh/ tiền sử phẫu thuật
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(MedicalRecordHistories item)
        {
            bool result = false;
            if (item == null) throw new AppException("Không tìm thấy thông tin item");
            this.unitOfWork.Repository<MedicalRecordHistories>().Create(item);
            await this.unitOfWork.SaveAsync();
            if (item.UserFiles != null && item.UserFiles.Any())
            {
                foreach (var userFile in item.UserFiles)
                {
                    userFile.Created = DateTime.Now;
                    userFile.CreatedBy = item.CreatedBy;
                    userFile.MedicalRecordId = item.MedicalRecordId;
                    userFile.UserId = item.UserId;
                    userFile.MedicalRecordHistoryId = item.Id;
                    this.unitOfWork.Repository<UserFiles>().Create(userFile);
                }
                await this.unitOfWork.SaveAsync();
            }
            result = true;

            return result;
        }

        /// <summary>
        /// Cập nhật tiền sử/tiền sử phẫu thuật
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(MedicalRecordHistories item)
        {
            bool result = false;
            if (item == null) throw new AppException("Không tìm thấy thông tin item");
            var existItem = await this.unitOfWork.Repository<MedicalRecordHistories>().GetQueryable()
                .Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem == null) throw new AppException("Không tìm thấy thông tin tiền sử");
            existItem = mapper.Map<MedicalRecordHistories>(item);
            this.unitOfWork.Repository<MedicalRecordHistories>().Update(existItem);
            if (item.UserFiles != null && item.UserFiles.Any())
            {
                foreach (var userFile in item.UserFiles)
                {
                    var existUserFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable()
                        .Where(e => e.Id == userFile.Id).FirstOrDefaultAsync();
                    if (existUserFile != null)
                    {
                        existUserFile = mapper.Map<UserFiles>(userFile);
                        existUserFile.MedicalRecordId = item.MedicalRecordId;
                        existUserFile.UserId = item.UserId;
                        existUserFile.MedicalRecordHistoryId = item.Id;
                        existUserFile.Updated = DateTime.Now;
                        existUserFile.UpdatedBy = item.UpdatedBy;
                        this.unitOfWork.Repository<UserFiles>().Update(existUserFile);
                    }
                    else
                    {
                        userFile.Created = DateTime.Now;
                        userFile.CreatedBy = item.CreatedBy;
                        userFile.MedicalRecordId = item.MedicalRecordId;
                        userFile.UserId = item.UserId;
                        userFile.MedicalRecordHistoryId = item.Id;
                        this.unitOfWork.Repository<UserFiles>().Create(userFile);
                    }
                }
            }
            await this.unitOfWork.SaveAsync();
            result = true;
            return result;
        }

    }
}
