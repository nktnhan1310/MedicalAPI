using Medical.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IUserService : IDomainService<Users, SearchUser>
    {
        Task<bool> Verify(string userName, string password);

        Task<bool> HasPermission(int userId, string controller, IList<string> permissions);
    }
}
