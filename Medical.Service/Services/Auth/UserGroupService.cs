using AutoMapper;
using Medical.Entities;
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
    public class UserGroupService : CatalogueService<UserGroups, BaseSearch>, IUserGroupService
    {
        public UserGroupService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }


        /// <summary>
        /// Tạo nhóm người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(UserGroups item)
        {
            bool result = false;
            if (item != null)
            {
                // Tạo mới nhóm người dùng
                await this.unitOfWork.Repository<UserGroups>().CreateAsync(item);
                await this.unitOfWork.SaveAsync();

                // Lưu thông tin user thuộc nhóm người dùng
                if (item.UserInGroups != null && item.UserInGroups.Any())
                {
                    foreach (var userInGroup in item.UserInGroups)
                    {
                        userInGroup.Created = DateTime.Now;
                        userInGroup.UserGroupId = item.Id;
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
                        await this.unitOfWork.Repository<PermitObjectPermissions>().CreateAsync(permitObjectPermission);
                    }
                }
                await this.unitOfWork.SaveAsync();


                result = true;
            }

            return result;
        }

        /// <summary>
        /// Cập nhật thông tin nhóm người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(UserGroups item)
        {
            bool result = false;
            var existItem = await this.Queryable.Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem != null)
            {
                existItem = mapper.Map<UserGroups>(item);
                this.unitOfWork.Repository<UserGroups>().Update(existItem);
                await this.unitOfWork.SaveAsync();

                // Cập nhật thông tin user ở nhóm
                if (item.UserInGroups != null && item.UserInGroups.Any())
                {
                    foreach (var userInGroup in item.UserInGroups)
                    {
                        var existUserInGroup = await this.unitOfWork.Repository<UserInGroups>().GetQueryable().Where(e => e.Id == userInGroup.Id).FirstOrDefaultAsync();
                        if (existUserInGroup != null)
                        {
                            existUserInGroup = mapper.Map<UserInGroups>(userInGroup);
                            existUserInGroup.UserGroupId = item.Id;
                            existUserInGroup.Updated = DateTime.Now;
                            this.unitOfWork.Repository<UserInGroups>().Update(existUserInGroup);
                        }
                        else
                        {
                            userInGroup.Created = DateTime.Now;
                            userInGroup.UserGroupId = item.Id;
                            await this.unitOfWork.Repository<UserInGroups>().CreateAsync(userInGroup);

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
                result = true;
            }

            return result;
        }

    }
}
