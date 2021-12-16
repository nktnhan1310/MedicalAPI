﻿using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class UserGroupService : CatalogueHospitalService<UserGroups, BaseHospitalSearch>, IUserGroupService
    {
        public UserGroupService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }



        /// <summary>
        /// Tạo nhóm người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(UserGroups item)
        {
            if (item == null) throw new AppException("Item không tồn tại");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tạo mới nhóm người dùng
                    await this.unitOfWork.Repository<UserGroups>().CreateAsync(item);
                    await this.unitOfWork.SaveAsync();

                    // Lưu thông tin user thuộc nhóm người dùng
                    if (item.UserIds != null && item.UserIds.Any())
                    {
                        foreach (var userId in item.UserIds)
                        {
                            UserInGroups userInGroup = new UserInGroups()
                            {
                                CreatedBy = item.CreatedBy,
                                UserId = userId,
                                Created = DateTime.Now,
                                UserGroupId = item.Id,
                                Active = true,
                                Deleted = false,
                                Id = 0
                            };
                            await this.unitOfWork.Repository<UserInGroups>().CreateAsync(userInGroup);
                        }
                    }
                    // Lưu thông tin chức năng + quyền tương ứng
                    if (item.PermitObjectPermissions != null && item.PermitObjectPermissions.Any())
                    {
                        foreach (var permitObjectPermission in item.PermitObjectPermissions)
                        {
                            permitObjectPermission.Created = DateTime.Now;
                            permitObjectPermission.UserGroupId = item.Id;
                            permitObjectPermission.Active = true;
                            permitObjectPermission.Id = 0;
                            await this.unitOfWork.Repository<PermitObjectPermissions>().CreateAsync(permitObjectPermission);
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
        /// Cập nhật thông tin nhóm người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(UserGroups item)
        {
            var existItem = await this.Queryable.Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem == null) throw new AppException("Không tìm thấy thông tin item");
            using(var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    existItem = mapper.Map<UserGroups>(item);
                    this.unitOfWork.Repository<UserGroups>().Update(existItem);
                    await this.unitOfWork.SaveAsync();

                    // Cập nhật thông tin user ở nhóm
                    if (item.UserIds != null && item.UserIds.Any())
                    {
                        foreach (var userId in item.UserIds)
                        {
                            var existUserInGroup = await this.unitOfWork.Repository<UserInGroups>().GetQueryable()
                                .Where(e => e.UserId == userId && e.UserGroupId == existItem.Id).FirstOrDefaultAsync();
                            if (existUserInGroup != null)
                            {
                                existUserInGroup.UserId = userId;
                                existUserInGroup.UserGroupId = item.Id;
                                existUserInGroup.Updated = DateTime.Now;
                                this.unitOfWork.Repository<UserInGroups>().Update(existUserInGroup);
                            }
                            else
                            {
                                UserInGroups userInGroup = new UserInGroups()
                                {
                                    CreatedBy = item.CreatedBy,
                                    UserId = userId,
                                    Created = DateTime.Now,
                                    UserGroupId = existItem.Id,
                                    Id = 0
                                };
                                await this.unitOfWork.Repository<UserInGroups>().CreateAsync(userInGroup);
                            }
                        }

                        // Kiểm tra những item không có trong role chọn => Xóa đi
                        var existGroupOlds = await this.unitOfWork.Repository<UserInGroups>().GetQueryable().Where(e => !item.UserIds.Contains(e.UserId) && e.UserGroupId == existItem.Id).ToListAsync();
                        if (existGroupOlds != null)
                        {
                            foreach (var existGroupOld in existGroupOlds)
                            {
                                this.unitOfWork.Repository<UserInGroups>().Delete(existGroupOld);
                            }
                        }

                    }
                    else
                    {
                        var existUserInGroups = await this.unitOfWork.Repository<UserInGroups>().GetQueryable()
                            .Where(e => !e.Deleted && e.UserGroupId == existItem.Id).ToListAsync();
                        if (existUserInGroups != null && existUserInGroups.Any())
                        {
                            foreach (var existUserInGroup in existUserInGroups)
                            {
                                this.unitOfWork.Repository<UserInGroups>().Delete(existUserInGroup);
                            }
                        }
                    }

                    // Cập nhật thông tin quyền với chứng năng tương ứng của nhóm
                    if (item.PermitObjectPermissions != null && item.PermitObjectPermissions.Any())
                    {
                        foreach (var permitObjectPermission in item.PermitObjectPermissions)
                        {
                            var existPermitObjectPermission = await this.unitOfWork.Repository<PermitObjectPermissions>().GetQueryable().Where(e => e.Id == permitObjectPermission.Id).FirstOrDefaultAsync();
                            if (existPermitObjectPermission != null)
                            {
                                existPermitObjectPermission = mapper.Map<PermitObjectPermissions>(permitObjectPermission);
                                existPermitObjectPermission.UserGroupId = item.Id;
                                existPermitObjectPermission.Updated = DateTime.Now;
                                this.unitOfWork.Repository<PermitObjectPermissions>().Update(existPermitObjectPermission);
                            }
                            else
                            {
                                permitObjectPermission.Created = DateTime.Now;
                                permitObjectPermission.UserGroupId = item.Id;
                                await this.unitOfWork.Repository<PermitObjectPermissions>().CreateAsync(permitObjectPermission);

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

    }
}
