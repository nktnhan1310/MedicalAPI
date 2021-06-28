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
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
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
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@Email", string.IsNullOrEmpty(baseSearch.Email) ? DBNull.Value : (object)baseSearch.Email),
                new SqlParameter("@Phone", baseSearch.Phone),
                new SqlParameter("@UserGroupId", baseSearch.UserGroupId),
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
                && (x.UserName == item.UserName
                || x.Email == item.UserName
                || x.Phone == item.UserName
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

                    // Lưu thông file của user
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var userFile in item.UserFiles)
                        {
                            userFile.Created = DateTime.Now;
                            userFile.UserId = item.Id;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(userFile);
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
                // Cập nhật thông tin file người dùng
                if (item.UserFiles != null && item.UserFiles.Any())
                {
                    foreach (var userFile in item.UserFiles)
                    {
                        var existUserFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable().Where(e => e.Id == userFile.Id).FirstOrDefaultAsync();
                        if (existUserFile != null)
                        {
                            existUserFile = mapper.Map<UserFiles>(userFile);
                            existUserFile.UserId = item.Id;
                            existUserFile.Updated = DateTime.Now;
                            this.unitOfWork.Repository<UserFiles>().Update(existUserFile);
                        }
                        else
                        {
                            userFile.Created = DateTime.Now;
                            userFile.UserId = item.Id;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(userFile);

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
        public async Task<bool> Verify(string userName, string password, bool isMrApp = false)
        {
            var user = await Queryable
                .Where(e => !e.Deleted
                && (e.UserName == userName
                || e.Phone == userName
                || e.Email == userName
                )
                && ((!isMrApp && (e.IsAdmin || e.HospitalId.HasValue)) || (isMrApp && !e.IsAdmin && !e.HospitalId.HasValue))
                )
                .FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.IsLocked && isMrApp)
                {
                    if (user.LockedDate.HasValue)
                        throw new Exception(string.Format("Account is Locked! Unlock date: {0}", user.LockedDate.Value.ToString("dd/MM/yyyy")));
                    else throw new Exception("Account is Locked");
                }
                if (!user.Active)
                {
                    throw new Exception("Account is UnActive");
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
                var permitObjectChecks = await unitOfWork.Repository<PermitObjects>().GetQueryable().Where(e => !e.Deleted
                && !string.IsNullOrEmpty(e.ControllerNames)
                && e.ControllerNames.Contains(controller)
                ).ToListAsync();
                permitObjectChecks = permitObjectChecks.Where(e => e.ControllerNames.Split(";", StringSplitOptions.None).Contains(controller)).ToList();
                if (permitObjectChecks != null && permitObjectChecks.Any())
                {
                    var permitObjectCheckIds = permitObjectChecks.Select(e => e.Id).ToList();
                    // Lấy ra những quyền user có trong chức năng cần kiểm tra
                    var permitObjectPermissions = await unitOfWork.Repository<PermitObjectPermissions>().GetQueryable()
                    .Where(e => e.UserGroupId.HasValue
                    && userGroupIds.Contains(e.UserGroupId.Value)
                    && permitObjectCheckIds.Contains(e.PermitObjectId)
                    )
                    .ToListAsync();
                    if (permitObjectPermissions != null && permitObjectPermissions.Any())
                    {
                        permitObjectIds = permitObjectPermissions.Select(e => e.PermitObjectId).Distinct().ToList();

                        foreach (var permitObjectId in permitObjectIds)
                        {
                            // Lấy danh mục mã quyền user cần kiểm tra
                            permissionIds = permitObjectPermissions.Where(e => e.PermitObjectId == permitObjectId).Select(e => e.PermissionId).ToList();
                            var permissionCodes = await unitOfWork.Repository<Permissions>().GetQueryable().Where(e => permissionIds.Contains(e.Id))
                                .Select(e => e.Code)
                                .ToListAsync();

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
                }

            }
            return hasPermit;
        }

        /// <summary>
        /// Cập nhật thông tin user token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="isLogin"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserToken(int userId, string token, bool isLogin = false)
        {
            bool result = false;
            var userInfo = await this.unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == userId).FirstOrDefaultAsync();
            if (userInfo != null)
            {
                if (isLogin)
                {
                    userInfo.Token = token;
                    userInfo.ExpiredDate = DateTime.UtcNow.AddDays(1);
                }
                else
                {
                    userInfo.Token = string.Empty;
                    userInfo.ExpiredDate = null;
                }
                Expression<Func<Users, object>>[] includeProperties = new Expression<Func<Users, object>>[]
                {
                    e => e.Token,
                    e => e.ExpiredDate
                };
                this.unitOfWork.Repository<Users>().UpdateFieldsSave(userInfo, includeProperties);
                await this.unitOfWork.SaveAsync();
                result = true;
            }

            return result;
        }
    }
}
