using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class UserService : DomainService<Users, SearchUser>, IUserService
    {
        public UserService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "User_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchUser baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@Email", string.IsNullOrEmpty(baseSearch.Email) ? DBNull.Value : (object)baseSearch.Email),
                new SqlParameter("@Phone", baseSearch.Phone),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Kiểm tra user đã tồn tại chưa?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(Users item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistEmail = !string.IsNullOrEmpty(item.Email) && await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.Email == item.Email);
            bool isExistPhone = !string.IsNullOrEmpty(item.Phone) && await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.Phone == item.Phone);
            bool isExistUserName = !string.IsNullOrEmpty(item.UserName)
                && await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id
                && (x.UserName.Contains(item.UserName)
                || x.Email.Contains(item.UserName)
                || x.Phone.Contains(item.UserName)
                ));
            bool isPhone = ValidateUserName.IsPhoneNumber(item.UserName);
            bool isEmail = ValidateUserName.IsEmail(item.UserName);

            if (isExistEmail)
                messages.Add("Email đã tồn tại!");
            if (isExistPhone)
                messages.Add("Số điện thoại đã tồn tại!");
            if (isExistUserName)
            {
                if (isPhone)
                    messages.Add("Số điện thoại đã tồn tại!");
                else if (isEmail)
                    messages.Add("Email đã tồn tại!");
                else
                    messages.Add("User name đã tồn tại!");
            }
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

        /// <summary>
        /// Lưu thông tin người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Users item)
        {
            bool result = false;
            if (item != null)
            {
                if (item != null)
                {
                    // Tạo mới nhóm người dùng
                    await this.unitOfWork.Repository<Users>().CreateAsync(item);
                    await this.unitOfWork.SaveAsync();

                    // Lưu thông tin user thuộc nhóm người dùng
                    if (item.UserInGroups != null && item.UserInGroups.Any())
                    {
                        foreach (var userInGroup in item.UserInGroups)
                        {
                            userInGroup.Created = DateTime.Now;
                            userInGroup.UserId = item.Id;
                            await this.unitOfWork.Repository<UserInGroups>().CreateAsync(userInGroup);
                        }
                    }
                    // Lưu thông tin chức năng + quyền tương ứng
                    if (item.PermitObjectPermissions != null && item.PermitObjectPermissions.Any())
                    {
                        foreach (var permitObjectPermission in item.PermitObjectPermissions)
                        {
                            permitObjectPermission.Created = DateTime.Now;
                            permitObjectPermission.UserId = item.Id;
                            await this.unitOfWork.Repository<PermitObjectPermissions>().CreateAsync(permitObjectPermission);
                        }
                    }
                    await this.unitOfWork.SaveAsync();


                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Users item)
        {
            bool result = false;
            var existItem = await this.Queryable.Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem != null)
            {
                existItem = mapper.Map<Users>(item);
                this.unitOfWork.Repository<Users>().Update(existItem);
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
                            existUserInGroup.UserId = item.Id;
                            existUserInGroup.Updated = DateTime.Now;
                            this.unitOfWork.Repository<UserInGroups>().Update(existUserInGroup);
                        }
                        else
                        {
                            userInGroup.Created = DateTime.Now;
                            userInGroup.UserId = item.Id;
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
                            existPermitObjectPermission.UserId = item.Id;
                            existPermitObjectPermission.Updated = DateTime.Now;
                            this.unitOfWork.Repository<PermitObjectPermissions>().Update(existPermitObjectPermission);
                        }
                        else
                        {
                            permitObjectPermission.Created = DateTime.Now;
                            permitObjectPermission.UserId = item.Id;
                            await this.unitOfWork.Repository<PermitObjectPermissions>().CreateAsync(permitObjectPermission);

                        }
                    }
                }
                await this.unitOfWork.SaveAsync();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra user đăng nhập
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> Verify(string userName, string password)
        {
            var user = await Queryable
                .Where(e => !e.Deleted
                && (e.UserName.Contains(userName)
                || e.Phone.Contains(userName)
                || e.Email.Contains(userName)
                )
                )
                .FirstOrDefaultAsync();
            if (user != null)
            {
                if (!user.Active)
                {
                    throw new Exception("Account is locked");
                }
                if (user.Password == SecurityUtils.HashSHA1(password))
                {
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }

        /// <summary>
        /// Kiểm tra pass word cũ đã giống chưa
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> CheckCurrentUserPassword(int userId, string password, string newPasssword)
        {
            string message = string.Empty;
            List<string> messages = new List<string>();
            bool isCurrentPassword = await this.Queryable.AnyAsync(x => x.Id == userId && x.Password == SecurityUtils.HashSHA1(password));
            bool isDuplicateNewPassword = await this.Queryable.AnyAsync(x => x.Id == userId && x.Password == SecurityUtils.HashSHA1(newPasssword));
            if (!isCurrentPassword)
                messages.Add("Mật khẩu cũ không chính xác");
            else if (isDuplicateNewPassword)
                messages.Add("Mật khẩu mới không được trùng mật khẩu cũ");
            if (messages.Any())
                message = string.Join("; ", messages);
            return message;
        }

        /// <summary>
        /// Kiểm tra quyền của user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="controller"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<bool> HasPermission(int userId, string controller, IList<string> permissions)
        {
            bool hasPermit = false;

            // Lấy ra những nhóm user thuộc
            var userGroupIds = await unitOfWork.Repository<UserInGroups>().GetQueryable()
                .Where(e => e.UserId == userId)
                .Select(e => e.UserGroupId).ToListAsync();

            var permissionIds = new List<int>();
            var permitObjectIds = new List<int>();

            if (userGroupIds != null && userGroupIds.Any())
            {
                // Lấy ra những quyền user có trong chức năng cần kiểm tra
                var permitObjectPermissions = await unitOfWork.Repository<PermitObjectPermissions>().GetQueryable()
                .Where(e => e.UserGroupId.HasValue && userGroupIds.Contains(e.UserGroupId.Value)).ToListAsync();
                if (permitObjectPermissions != null && permitObjectPermissions.Any())
                {
                    // Lấy danh mục mã quyền user cần kiểm tra
                    permissionIds = permitObjectPermissions.Select(e => e.PermissionId).ToList();
                    var permissionCodes = await unitOfWork.Repository<Permissions>().GetQueryable().Where(e => permissionIds.Contains(e.Id))
                        .Select(e => e.Code)
                        .ToListAsync();

                    permitObjectIds = permitObjectPermissions.Select(e => e.PermitObjectId).ToList();
                    // Lấy danh chức năng cần kiểm tra
                    var permitObjectControllers = await unitOfWork.Repository<PermitObjects>().GetQueryable().Where(e => permitObjectIds.Contains(e.Id))
                        .Select(e => e.ControllerNames.Split(";", StringSplitOptions.None))
                        .ToListAsync();

                    // Kiểm tra user có quyền trong chức năng không
                    if (permissionCodes != null && permissionCodes.Any() && permitObjectControllers != null && permitObjectControllers.Any())
                    {
                        hasPermit = permitObjectControllers.Any(x => x.Contains(controller)) && permissions.Any(x => permissionCodes.Contains(x));
                    }

                }
            }
            return hasPermit;
        }
    }
}
