using AutoMapper;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Entities.DomainEntity.Search;
using Medical.Interface.DbContext;
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
    public abstract class ReportCoreService<R, T> : IReportCoreService<R, T> where R : ReportAppDomain where T : ReportBaseSearch, new()
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        protected readonly IMedicalDbContext Context;
        public ReportCoreService(IUnitOfWork unitOfWork, IMapper mapper, IMedicalDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.Context = context;
        }

        /// <summary>
        /// Lấy danh sách báo cáo
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public virtual async Task<PagedListReport<R>> GetPagedListReport(T baseSearch)
        {
            PagedListReport<R> pagedList = new PagedListReport<R>();
            if (baseSearch.IsExport)
            {
                baseSearch.PageIndex = 1;
                baseSearch.PageSize = int.MaxValue;
            }
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }

        /// <summary>
        /// Lấy tên store procedure
        /// </summary>
        /// <returns></returns>
        protected virtual string GetStoreProcName()
        {
            return string.Empty;
        }

        /// <summary>
        /// Lấy thông tin sql parameter
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        protected virtual SqlParameter[] GetSqlParameters(T baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public virtual async Task<PagedListReport<R>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedListReport<R> pagedList = new PagedListReport<R>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.Items = MappingDataTable.ConvertToList<R>(dataTable);
                    if (pagedList.Items != null && pagedList.Items.Any())
                        pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalItem;
                    return pagedList;
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
