using AutoMapper;
using Medical.Entities;
using Medical.Entities.Reports;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class ReportRevenueService : ReportCoreService<ReportRevenue, SearchReportRevenue>, IReportRevenueService
    {
        public ReportRevenueService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IMedicalDbContext context) : base(unitOfWork, mapper, context)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Report_GetRevenue";
        }

        protected override SqlParameter[] GetSqlParameters(SearchReportRevenue baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                new SqlParameter("@TotalAppPrice", SqlDbType.Float, 0),

            };
            return parameters;
        }

        public override async Task<PagedListReport<ReportRevenue>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedListReport<ReportRevenue> pagedList = new PagedListReport<ReportRevenue>();
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
                    command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalAppPrice"].Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.TotalItem = int.Parse(command.Parameters["@TotalPage"].Value.ToString());
                    double totalRevenueValue = 0;
                    if (command.Parameters["@TotalAppPrice"] != null && double.TryParse(command.Parameters["@TotalAppPrice"].Value.ToString(), out totalRevenueValue))
                        pagedList.TotalRevenueValue = totalRevenueValue;
                    pagedList.Items = MappingDataTable.ConvertToList<ReportRevenue>(dataTable);
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
