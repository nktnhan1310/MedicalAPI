using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
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
<<<<<<< HEAD
        private IMedicalRecordService medicalRecordService;
        private INotificationService notificationService;
        public UserService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper, IServiceProvider serviceProvider) : base(unitOfWork, medicalDbContext, mapper)
        {
=======
        private IMedicalDbContext medicalDbContext;
        private IMedicalRecordService medicalRecordService;
        private INotificationService notificationService;
        public UserService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IMedicalDbContext medicalDbContext, IServiceProvider serviceProvider) : base(unitOfWork, mapper)
        {
            this.medicalDbContext = medicalDbContext;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            this.medicalRecordService = serviceProvider.GetRequiredService<IMedicalRecordService>();
            this.notificationService = serviceProvider.GetRequiredService<INotificationService>();
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
                new SqlParameter("@IsHospital", baseSearch.IsHospital),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
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

            // Tạo code cho user trường hợp thêm mới
            if (item.Id <= 0 || string.IsNullOrEmpty(item.UserCode))
                item.UserCode = await this.GetUserCode(item);

            bool isExistUserCode = !string.IsNullOrEmpty(item.UserCode) && await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.UserCode == item.UserCode);
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
            if (isExistUserCode)
            {
                item.UserCode = await this.GetUserCode(item);
                await this.GetExistItemMessage(item);
            }

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
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<string> GetUserCode(Users item)
        {
            string result = string.Empty;
            // NẾU LÀ NV BỆNH VIỆN => TẠO MÃ RANDOM (MÃ BV + CHUỖI RANDOM 8 KÍ TỰ)
            if (item.HospitalId.HasValue && item.HospitalId.Value > 0)
            {
                var hospitalInfo = await this.unitOfWork.Repository<Hospitals>().GetQueryable()
                    .Where(e => e.Id == item.HospitalId.Value).FirstOrDefaultAsync();
                // KHỞI TẠO CHUỖI RANDOM 8 KÍ TỰ SỐ
                var randomNumber = RandomUtilities.RandomString(8, null, 8, true);
                result = hospitalInfo.Code + randomNumber;
            }
            // NẾU LÀ USER KHÔNG THUỘC BỆNH VIỆN => TẠO MÃ RANDOM GỒM 2 KÍ TỰ CHỮ + 8 KÍ TỰ SỐ
            else
                result = RandomUtilities.RandomString(10, 2, 8, true);
            return result;
        }

        /// <summary>
        /// Lưu thông tin người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Users item)
        {
            if (item == null) throw new AppException("Thông tin item không tồn tại");

            // Tạo mới nhóm người dùng
            item.Id = 0;
            await this.unitOfWork.Repository<Users>().CreateAsync(item);
            await this.unitOfWork.SaveAsync();

            // Lưu thông tin user thuộc nhóm người dùng
            if (item.UserGroupIds != null && item.UserGroupIds.Any())
            {
                foreach (var userGroupId in item.UserGroupIds)
                {
                    UserInGroups userInGroup = new UserInGroups()
                    {
                        Created = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        UserId = item.Id,
                        UserGroupId = userGroupId,
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
                    permitObjectPermission.UserId = item.Id;
                    permitObjectPermission.Active = true;
                    permitObjectPermission.Id = 0;
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
                    userFile.Active = true;
                    userFile.Id = 0;
                    await this.unitOfWork.Repository<UserFiles>().CreateAsync(userFile);
                }
            }
            await this.unitOfWork.SaveAsync();
            await this.medicalDbContext.SaveChangesAsync();

            this.medicalDbContext.Entry<Users>(item).State = EntityState.Detached;
            return true;
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Users item)
        {
            var existItem = await this.Queryable.Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem == null) throw new AppException("Không tìm thấy thông tin item");

            if (!item.IsResetPassword)
                item.Password = existItem.Password;
            existItem = mapper.Map<Users>(item);
            this.unitOfWork.Repository<Users>().Update(existItem);
            await this.unitOfWork.SaveAsync();

            // Cập nhật thông tin user ở nhóm
            if (item.UserGroupIds != null && item.UserGroupIds.Any())
            {
                foreach (var userGroupId in item.UserGroupIds)
                {
                    var existUserInGroup = await this.unitOfWork.Repository<UserInGroups>().GetQueryable()
                        .Where(e => e.UserGroupId == userGroupId && e.UserId == existItem.Id).FirstOrDefaultAsync();
                    if (existUserInGroup != null)
                    {
                        existUserInGroup.UserGroupId = userGroupId;
                        existUserInGroup.UserId = item.Id;
                        existUserInGroup.Updated = DateTime.Now;
                        this.unitOfWork.Repository<UserInGroups>().Update(existUserInGroup);
                    }
                    else
                    {
                        UserInGroups userInGroup = new UserInGroups()
                        {
                            Created = DateTime.Now,
                            CreatedBy = item.CreatedBy,
                            UserId = item.Id,
                            UserGroupId = userGroupId,
                            Active = true,
                            Deleted = false,
                        };

                        userInGroup.Created = DateTime.Now;
                        userInGroup.UserId = item.Id;
                        userInGroup.Id = 0;
                        await this.unitOfWork.Repository<UserInGroups>().CreateAsync(userInGroup);
                    }
                }

<<<<<<< HEAD
                // Kiểm tra những item không có trong role chọn => Xóa đi
                var existGroupOlds = await this.unitOfWork.Repository<UserInGroups>().GetQueryable().Where(e => !item.UserGroupIds.Contains(e.UserGroupId) && e.UserId == existItem.Id).ToListAsync();
                if (existGroupOlds != null)
                {
                    foreach (var existGroupOld in existGroupOlds)
=======
                    // Kiểm tra những item không có trong role chọn => Xóa đi
                    var existGroupOlds = await this.unitOfWork.Repository<UserInGroups>().GetQueryable().Where(e => !item.UserGroupIds.Contains(e.UserGroupId) && e.UserId == existItem.Id).ToListAsync();
                    if (existGroupOlds != null)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                    {
                        this.unitOfWork.Repository<UserInGroups>().Delete(existGroupOld);
                    }
                }
            }
            else
            {
                var userInGroups = await this.unitOfWork.Repository<UserInGroups>().GetQueryable().Where(e => e.UserId == existItem.Id).ToListAsync();
                if (userInGroups != null && userInGroups.Any())
                {
                    foreach (var userInGroup in userInGroups)
                    {
                        //userInGroup.Updated = DateTime.Now;
                        //userInGroup.UpdatedBy = item.UpdatedBy;
                        //userInGroup.Deleted = true;
                        this.unitOfWork.Repository<UserInGroups>().Delete(userInGroup);
                    }
                }
            }

            // Cập nhật thông tin quyền với chứng năng tương ứng của nhóm
            if (item.PermitObjectPermissions != null && item.PermitObjectPermissions.Any())
            {
                foreach (var permitObjectPermission in item.PermitObjectPermissions)
                {
                    var existPermitObjectPermission = await this.unitOfWork.Repository<PermitObjectPermissions>().GetQueryable()
                        .Where(e => e.Id == permitObjectPermission.Id).FirstOrDefaultAsync();
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
                        permitObjectPermission.Id = 0;
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
                        userFile.Id = 0;
                        await this.unitOfWork.Repository<UserFiles>().CreateAsync(userFile);

                    }
                }
            }
            await this.unitOfWork.SaveAsync();
            return true;
        }

        /// <summary>
        /// Tạo mới thông tin user + hồ sơ người bệnh
        /// </summary>
        /// <param name="userGeneralInfo"></param>
        /// <returns></returns>
        public async Task<bool> CreateUserGeneralInfo(UserGeneralInfo userGeneralInfo)
        {
            if (userGeneralInfo == null) throw new AppException("Không tìm thấy thông tin thêm mới");

            // KIỂM TRA THÔNG TIN USER + HỒ SƠ NGƯỜI BỆNH CỦA USER
            if (userGeneralInfo.User == null) throw new AppException("Không tìm thấy thông tin user");
            if (userGeneralInfo.MedicalRecord == null) throw new AppException("Không tìm thấy thông tin hồ sơ người bệnh");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // THÊM MỚI THÔNG TIN USER
                    userGeneralInfo.User.Active = true;
                    await this.CreateAsync(userGeneralInfo.User);

                    // THÊM MỚI THÔNG TIN HỒ SƠ
                    userGeneralInfo.MedicalRecord.UserId = userGeneralInfo.User.Id;
                    userGeneralInfo.MedicalRecord.BirthDate = userGeneralInfo.User.BirthDate;
                    userGeneralInfo.MedicalRecord.Phone = userGeneralInfo.User.Phone;
                    userGeneralInfo.MedicalRecord.Email = userGeneralInfo.User.Email;
                    userGeneralInfo.MedicalRecord.Address = userGeneralInfo.User.Address;
                    userGeneralInfo.MedicalRecord.UserFullName = userGeneralInfo.User.FirstName + " " + userGeneralInfo.User.LastName;
                    userGeneralInfo.MedicalRecord.Active = true;
                    await this.medicalRecordService.CreateAsync(userGeneralInfo.MedicalRecord);

                    // THÊM MỚI THÔNG TIN FILE CHO HỒ SƠ (NẾU CÓ)
                    if (userGeneralInfo.UserFiles != null && userGeneralInfo.UserFiles.Any())
                    {
                        foreach (var userFile in userGeneralInfo.UserFiles)
                        {
                            userFile.Created = DateTime.Now;
                            userFile.CreatedBy = userGeneralInfo.User.CreatedBy;
                            userFile.UserId = userGeneralInfo.User.Id;
                            userFile.MedicalRecordId = userGeneralInfo.MedicalRecord.Id;
                            userFile.Active = true;
                            userFile.Id = 0;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(userFile);
                        }
                        await this.unitOfWork.SaveAsync();
                    }
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
        /// Cập nhật thông tin chung của hồ sơ
        /// </summary>
        /// <param name="userGeneralInfo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserGeneralInfo(UserGeneralInfo userGeneralInfo)
        {
            bool success = true;
            if (userGeneralInfo == null) throw new AppException("Không tìm thấy thông tin cập nhật");
            // KIỂM TRA THÔNG TIN USER + HỒ SƠ NGƯỜI BỆNH CỦA USER
            if (userGeneralInfo.User == null) throw new AppException("Không tìm thấy thông tin user");
            if (userGeneralInfo.MedicalRecord == null) throw new AppException("Không tìm thấy thông tin hồ sơ người bệnh");
            using (var contextTransaction = await medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // CẬP NHẬT THÔNG TIN USER
                    userGeneralInfo.User.Active = true;
                    success &= await this.UpdateAsync(userGeneralInfo.User);
                    // CẬP NHẬT THÔNG TIN HỒ SƠ
                    userGeneralInfo.MedicalRecord.UserId = userGeneralInfo.User.Id;
                    userGeneralInfo.MedicalRecord.BirthDate = userGeneralInfo.User.BirthDate;
                    userGeneralInfo.MedicalRecord.Phone = userGeneralInfo.User.Phone;
                    userGeneralInfo.MedicalRecord.Email = userGeneralInfo.User.Email;
                    userGeneralInfo.MedicalRecord.Address = userGeneralInfo.User.Address;
                    userGeneralInfo.MedicalRecord.UserFullName = userGeneralInfo.User.FirstName + " " + userGeneralInfo.User.LastName;
                    userGeneralInfo.MedicalRecord.Active = true;
                    success &= await this.medicalRecordService.UpdateAsync(userGeneralInfo.MedicalRecord);

                    // CẬP NHẬT THÔNG TIN FILE CỦA USER
                    if (userGeneralInfo.UserFiles != null && userGeneralInfo.UserFiles.Any())
                    {
                        foreach (var userFile in userGeneralInfo.UserFiles)
                        {
                            var existUserFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable()
                                .Where(e => !e.Deleted && e.UserId == userGeneralInfo.User.Id && e.MedicalRecordId == userGeneralInfo.MedicalRecord.Id).FirstOrDefaultAsync();

                            if (existUserFile == null)
                            {
                                userFile.Created = DateTime.Now;
                                userFile.CreatedBy = userGeneralInfo.User.CreatedBy;
                                userFile.UserId = userGeneralInfo.User.Id;
                                userFile.MedicalRecordId = userGeneralInfo.MedicalRecord.Id;
                                userFile.Active = true;
                                userFile.Id = 0;
                                this.unitOfWork.Repository<UserFiles>().Create(userFile);
                            }
                            else
                            {
                                existUserFile = mapper.Map<UserFiles>(userFile);
                                existUserFile.Updated = DateTime.Now;
                                existUserFile.UpdatedBy = userGeneralInfo.User.UpdatedBy;
                                existUserFile.UserId = userGeneralInfo.User.Id;
                                existUserFile.MedicalRecordId = userGeneralInfo.MedicalRecord.Id;
                                existUserFile.Active = true;
                                this.unitOfWork.Repository<UserFiles>().Update(existUserFile);
                            }
                        }
                        await this.unitOfWork.SaveAsync();
                    }
                    await contextTransaction.CommitAsync();
                }
                catch (Exception)
                {
                    success = false;
                    contextTransaction.Rollback();
                }
            }
            return success;
        }

        /// <summary>
        /// Tạo mới thông tin user + hồ sơ người bệnh
        /// </summary>
        /// <param name="userGeneralInfo"></param>
        /// <returns></returns>
        public async Task<bool> CreateUserGeneralInfo(UserGeneralInfo userGeneralInfo)
        {
            bool result = true;
            if (userGeneralInfo == null) throw new AppException("Không tìm thấy thông tin thêm mới");

            // KIỂM TRA THÔNG TIN USER + HỒ SƠ NGƯỜI BỆNH CỦA USER
            if (userGeneralInfo.User == null) throw new AppException("Không tìm thấy thông tin user");
            if (userGeneralInfo.MedicalRecord == null) throw new AppException("Không tìm thấy thông tin hồ sơ người bệnh");
            using (var contextTransaction = await this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // THÊM MỚI THÔNG TIN USER
                    userGeneralInfo.User.Active = true;
                    result &= await this.CreateAsync(userGeneralInfo.User);

                    // THÊM MỚI THÔNG TIN HỒ SƠ
                    userGeneralInfo.MedicalRecord.UserId = userGeneralInfo.User.Id;
                    userGeneralInfo.MedicalRecord.BirthDate = userGeneralInfo.User.BirthDate;
                    userGeneralInfo.MedicalRecord.Phone = userGeneralInfo.User.Phone;
                    userGeneralInfo.MedicalRecord.Email = userGeneralInfo.User.Email;
                    userGeneralInfo.MedicalRecord.Address = userGeneralInfo.User.Address;
                    userGeneralInfo.MedicalRecord.UserFullName = userGeneralInfo.User.FirstName + " " + userGeneralInfo.User.LastName;
                    userGeneralInfo.MedicalRecord.Active = true;
                    result &= await this.medicalRecordService.CreateAsync(userGeneralInfo.MedicalRecord);

                    // THÊM MỚI THÔNG TIN FILE CHO HỒ SƠ (NẾU CÓ)
                    if (userGeneralInfo.UserFiles != null && userGeneralInfo.UserFiles.Any())
                    {
                        foreach (var userFile in userGeneralInfo.UserFiles)
                        {
                            userFile.Created = DateTime.Now;
                            userFile.CreatedBy = userGeneralInfo.User.CreatedBy;
                            userFile.UserId = userGeneralInfo.User.Id;
                            userFile.MedicalRecordId = userGeneralInfo.MedicalRecord.Id;
                            userFile.Active = true;
                            userFile.Id = 0;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(userFile);
                        }
                        await this.unitOfWork.SaveAsync();
                    }

                    await contextTransaction.CommitAsync();
                }
                catch (Exception)
                {
                    result = false;
                    contextTransaction.Rollback();
                }
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin chung của hồ sơ
        /// </summary>
        /// <param name="userGeneralInfo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserGeneralInfo(UserGeneralInfo userGeneralInfo)
        {
            bool success = true;
            if (userGeneralInfo == null) throw new AppException("Không tìm thấy thông tin cập nhật");
            // KIỂM TRA THÔNG TIN USER + HỒ SƠ NGƯỜI BỆNH CỦA USER
            if (userGeneralInfo.User == null) throw new AppException("Không tìm thấy thông tin user");
            if (userGeneralInfo.MedicalRecord == null) throw new AppException("Không tìm thấy thông tin hồ sơ người bệnh");
            using (var contextTransaction = await medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // CẬP NHẬT THÔNG TIN USER
                    userGeneralInfo.User.Active = true;
                    success &= await this.UpdateAsync(userGeneralInfo.User);
                    // CẬP NHẬT THÔNG TIN HỒ SƠ
                    userGeneralInfo.MedicalRecord.UserId = userGeneralInfo.User.Id;
                    userGeneralInfo.MedicalRecord.BirthDate = userGeneralInfo.User.BirthDate;
                    userGeneralInfo.MedicalRecord.Phone = userGeneralInfo.User.Phone;
                    userGeneralInfo.MedicalRecord.Email = userGeneralInfo.User.Email;
                    userGeneralInfo.MedicalRecord.Address = userGeneralInfo.User.Address;
                    userGeneralInfo.MedicalRecord.UserFullName = userGeneralInfo.User.FirstName + " " + userGeneralInfo.User.LastName;
                    userGeneralInfo.MedicalRecord.Active = true;
                    success &= await this.medicalRecordService.UpdateAsync(userGeneralInfo.MedicalRecord);

                    // CẬP NHẬT THÔNG TIN FILE CỦA USER
                    if (userGeneralInfo.UserFiles != null && userGeneralInfo.UserFiles.Any())
                    {
                        foreach (var userFile in userGeneralInfo.UserFiles)
                        {
                            var existUserFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable()
                                .Where(e => !e.Deleted && e.UserId == userGeneralInfo.User.Id && e.MedicalRecordId == userGeneralInfo.MedicalRecord.Id).FirstOrDefaultAsync();

                            if (existUserFile == null)
                            {
                                userFile.Created = DateTime.Now;
                                userFile.CreatedBy = userGeneralInfo.User.CreatedBy;
                                userFile.UserId = userGeneralInfo.User.Id;
                                userFile.MedicalRecordId = userGeneralInfo.MedicalRecord.Id;
                                userFile.Active = true;
                                userFile.Id = 0;
                                this.unitOfWork.Repository<UserFiles>().Create(userFile);
                            }
                            else
                            {
                                existUserFile = mapper.Map<UserFiles>(userFile);
                                existUserFile.Updated = DateTime.Now;
                                existUserFile.UpdatedBy = userGeneralInfo.User.UpdatedBy;
                                existUserFile.UserId = userGeneralInfo.User.Id;
                                existUserFile.MedicalRecordId = userGeneralInfo.MedicalRecord.Id;
                                existUserFile.Active = true;
                                this.unitOfWork.Repository<UserFiles>().Update(existUserFile);
                            }
                        }
                        await this.unitOfWork.SaveAsync();
                    }
                    await contextTransaction.CommitAsync();
                }
                catch (Exception)
                {
                    success = false;
                    contextTransaction.Rollback();
                }
            }
            return success;
        }

        /// <summary>
        /// Cập nhật password mới cho user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserPassword(int userId, string newPassword)
        {
            bool result = false;

            var existUserInfo = await this.unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == userId).FirstOrDefaultAsync();
            if (existUserInfo != null)
            {
                existUserInfo.Password = newPassword;
                existUserInfo.Updated = DateTime.Now;
                Expression<Func<Users, object>>[] includeProperties = new Expression<Func<Users, object>>[]
                {
                    e => e.Password,
                    e => e.Updated
                };
                await this.unitOfWork.Repository<Users>().UpdateFieldsSaveAsync(existUserInfo, includeProperties);
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
                    {
                        // Nếu qua thời hạn => unlock user
                        if (user.LockedDate.Value < DateTime.Now && user.Password == SecurityUtils.HashSHA1(password))
                        {
                            user.IsLocked = false;
                            user.LockedDate = null;
                            user.TotalViolations = 0;
                            Expression<Func<Users, object>>[] includeProperties = new Expression<Func<Users, object>>[]
                            {
                                e => e.IsLocked,
                                e => e.LockedDate,
                                e => e.TotalViolations
                            };
                            await this.unitOfWork.Repository<Users>().UpdateFieldsSaveAsync(user, includeProperties);
                            return true;
                        }
                        else throw new Exception(string.Format("Account is Locked! Unlock date: {0}", user.LockedDate.Value.ToString("dd/MM/yyyy")));
                    }
                    else throw new Exception("Account is Locked");
                }
                if (!user.Active)
                {
                    throw new Exception("Account is UnActive");
                }
                if (!user.IsAdmin && !user.IsCheckOTP)
                {
                    throw new Exception("Người dùng chưa xác thực otp");
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

            var userInfo = await unitOfWork.Repository<Users>().GetQueryable().Where(e => e.Id == userId).FirstOrDefaultAsync();
            if (userInfo != null && userInfo.IsLocked)
                throw new AppException("User account is locked!");

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
            //this.medicalDbContext.Entry<Users>(userInfo).State = EntityState.Detached;
            if (userInfo != null)
            {
                if (isLogin)
                {
                    userInfo.Token = token;
                    userInfo.ExpiredDate = DateTime.Now.AddDays(1);
                    //userInfo.ExpiredDate = DateTime.Now.AddMinutes(1);
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

        #region CRON JOBS

        /// <summary>
        /// JOB TẠO THÔNG BÁO CHÚC MỪNG SINH NHẬT USER
        /// </summary>
        /// <returns></returns>
        public async Task HappyBirthDateJob()
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0);
            DateTime dateCheck = DateTime.Now.Date + ts;
            // LẤY RA THÔNG TIN USER CÓ NGÀY SINH NHẬT HÔM NAY
            var userInfos = await this.Queryable
                .Where(e => !e.Deleted && e.Active && e.BirthDate.HasValue
                && e.BirthDate.Value.Date == dateCheck.Date
                && !e.HospitalId.HasValue
                && !e.IsAdmin
                )
                .Select(e => new Users()
                {
                    Id = e.Id,
                    Phone = e.Phone,
                    Email = e.Email
                })
                .ToListAsync();
            if (userInfos != null && userInfos.Any())
            {
                // LẤY RA THÔNG TIN TEMPLATE CHÚC MỪNG SINH NHẬT
                var happyBirthDateTemplate = await this.unitOfWork.Repository<NotificationTemplates>().GetQueryable()
                    .Where(e => !e.Deleted && e.Code == CoreContants.TEMPLATE_HAPPY_BIRTHDATE).FirstOrDefaultAsync();
                var notificationUserType = await this.unitOfWork.Repository<NotificationTypes>().GetQueryable()
                    .Where(e => !e.Deleted && e.Code == CatalogueUtilities.NotificationType.USER.ToString()).FirstOrDefaultAsync();
                // TẠO THÔNG BÁO CHÚC MỪNG SINH NHẬT CHO NGÀY HIỆN TẠI
                if (notificationUserType != null && happyBirthDateTemplate != null)
                {
                    Notifications notifications = new Notifications()
                    {
                        Created = DateTime.Now,
                        CreatedBy = "Job",
                        Active = true,
                        Deleted = false,
                        IsRead = false,
                        IsSendNotify = false,
                        Content = happyBirthDateTemplate.Content,
                        Title = happyBirthDateTemplate.Title,
                        NotificationTypeId = notificationUserType.Id,
                        TypeId = (int)CatalogueUtilities.NotificationCatalogueType.HappyBirthDate,
                        NotificationTemplateId = happyBirthDateTemplate.Id,
                        UserIds = userInfos.Select(e => e.Id).ToList()
                    };
                    await this.notificationService.CreateAsync(notifications);
                }
            }
        }

        #endregion



    }
}
