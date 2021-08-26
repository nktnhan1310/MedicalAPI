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
    }
}
