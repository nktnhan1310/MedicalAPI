using AutoMapper;
using Medical.Entities;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Medical.Utilities;
using Microsoft.AspNetCore.SignalR;
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
    public class NotificationService : CoreHospitalService<Notifications, SearchNotification>, INotificationService
    {
        private readonly IMedicalDbContext Context;

        public NotificationService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IMedicalDbContext Context) : base(unitOfWork, mapper)
        {
            this.Context = Context;
        }

        protected override string GetStoreProcName()
        {
            return "Notification_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchNotification baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@FromUserId", baseSearch.FromUserId),
                new SqlParameter("@ToUserId", baseSearch.ToUserId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@NotificationTypeId", baseSearch.NotificationTypeId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Thêm mới thông báo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Notifications item)
        {
            bool result = false;
            if (item != null)
            {
                item.Id = 0;
                await this.unitOfWork.Repository<Notifications>().CreateAsync(item);
                await this.unitOfWork.SaveAsync();

                // ---------------- Tạo thông tin báo cáo cho user nếu active notification
                if (item.Active && !item.IsSendNotify)
                    await CreateNotifyUserData(item);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin thông báo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Notifications item)
        {
            bool result = false;
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();

            if (exists != null)
            {
                exists = mapper.Map<Notifications>(item);
                unitOfWork.Repository<Notifications>().Update(exists);
            }
            await unitOfWork.SaveAsync();
            result = true;
            if (exists.Active && !exists.IsSendNotify)
                await CreateNotifyUserData(item);
            return result;
        }

        /// <summary>
        /// Tạo thông tin thông báo cho user theo điều kiện tương ứng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task CreateNotifyUserData(Notifications item)
        {
            await Task.Run(() =>
            {
                object obj = new object();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    if (item.UserIds == null)
                        item.UserIds = new List<int>();
                    if (item.HospitalIds == null)
                        item.HospitalIds = new List<int>();
                    if (item.UserGroupIds == null)
                        item.UserGroupIds = new List<int>();
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NotificationId", item.Id),
                        new SqlParameter("@FromUserId", item.FromUserId),
                        new SqlParameter("@UserIds", string.Join(",", item.UserIds)),
                        new SqlParameter("@HospitalId", item.HospitalId),
                        new SqlParameter("@HospitalIds", string.Join(",", item.HospitalIds)),
                        new SqlParameter("@UserGroupIds", string.Join(",", item.UserGroupIds)),
                        new SqlParameter("@CreatedBy", item.CreatedBy)
                    };

                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "Nofification_CreateData";
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }

    }
}
