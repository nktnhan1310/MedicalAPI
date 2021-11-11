using Medical.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IUserService : IDomainService<Users, SearchUser>
    {
        Task<bool> Verify(string userName, string password, bool isMrApp = false);

        Task<bool> HasPermission(int userId, string controller, IList<string> permissions);
        Task<string> CheckCurrentUserPassword(int userId, string password, string newPasssword);
        Task<bool> UpdateUserToken(int userId, string token, bool isLogin = false);
        Task<bool> UpdateUserPassword(int userId, string newPassword);

        /// <summary>
        /// Thêm mới thông tin account + hồ sơ người bệnh
        /// </summary>
        /// <param name="userGeneralInfo"></param>
        /// <returns></returns>
        Task<bool> CreateUserGeneralInfo(UserGeneralInfo userGeneralInfo);

        /// <summary>
        /// Cập nhật thông tin account + người bệnh
        /// </summary>
        /// <param name="userGeneralInfo"></param>
        /// <returns></returns>
        Task<bool> UpdateUserGeneralInfo(UserGeneralInfo userGeneralInfo);

        #region CRON JOBS
        
        /// <summary>
        /// JOB TẠO NOTIFI CHÚC MỪNG SINH NHẬT
        /// </summary>
        /// <returns></returns>
        Task HappyBirthDateJob();

        #endregion
    }
}
