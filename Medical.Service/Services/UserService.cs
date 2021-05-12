using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
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
        public override Task<string> GetExistItemMessage(Users item)
        {
            return Task.Run(() =>
            {
                List<string> messages = new List<string>();
                string result = string.Empty;
                if (!string.IsNullOrEmpty(item.Email) && Queryable.Any(x => !x.Deleted && x.Id != item.Id && x.Email == item.Email))
                    messages.Add("Email đã tồn tại!");
                if (!string.IsNullOrEmpty(item.Phone) && Queryable.Any(x => !x.Deleted && x.Id != item.Id && x.Phone == item.Phone))
                    messages.Add("Số điện thoại đã tồn tại!");
                if (!string.IsNullOrEmpty(item.UserName) && Queryable.Any(x => !x.Deleted && x.Id != item.Id && x.UserName == item.UserName))
                    messages.Add("User name đã tồn tại!");
                if (messages.Any())
                    result = string.Join(" ", messages);
                return result;
            });
        }

    }
}
