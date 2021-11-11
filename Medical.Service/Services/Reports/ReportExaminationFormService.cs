using AutoMapper;
using Medical.Entities;
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
    public class ReportExaminationFormService : ReportCoreService<ReportExaminationForm, SearchReportRevenue>, IReportExaminationFormService
    {
        public ReportExaminationFormService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IMedicalDbContext context) : base(unitOfWork, mapper, context)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Report_ExaminationForm";
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
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                new SqlParameter("@TotalNewForm", SqlDbType.Int, 0),
                new SqlParameter("@TotalWaitConfirmForm", SqlDbType.Int, 0),
                new SqlParameter("@TotalConfirmedForm", SqlDbType.Int, 0),
                new SqlParameter("@TotalCanceledForm", SqlDbType.Int, 0),
                new SqlParameter("@TotalWaitReExaminationForm", SqlDbType.Int, 0),
                new SqlParameter("@TotalConfirmedReExaminationForm", SqlDbType.Int, 0),
            };
            return parameters;
        }

        public override async Task<PagedListReport<ReportExaminationForm>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedListReport<ReportExaminationForm> pagedList = new PagedListReport<ReportExaminationForm>();
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
                    //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalNewForm"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalWaitConfirmForm"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalConfirmedForm"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalCanceledForm"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalWaitReExaminationForm"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalConfirmedReExaminationForm"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    //pagedList.TotalItem = int.Parse(command.Parameters["@TotalPage"].Value.ToString());
                    int totalNewForm = 0;
                    int totalWaitConfirmForm = 0;
                    int totalConfirmedForm = 0;
                    int totalCanceledForm = 0;
                    int totalWaitReExaminationForm = 0;
                    int totalConfirmedReExaminationForm = 0;

                    if (command.Parameters["@TotalNewForm"] != null && int.TryParse(command.Parameters["@TotalNewForm"].Value.ToString(), out totalNewForm))
                        pagedList.TotalNewForm = totalNewForm;
                    if (command.Parameters["@TotalWaitConfirmForm"] != null && int.TryParse(command.Parameters["@TotalWaitConfirmForm"].Value.ToString(), out totalWaitConfirmForm))
                        pagedList.TotalWaitConfirmForm = totalWaitConfirmForm;
                    if (command.Parameters["@TotalConfirmedForm"] != null && int.TryParse(command.Parameters["@TotalConfirmedForm"].Value.ToString(), out totalConfirmedForm))
                        pagedList.TotalConfirmedForm = totalConfirmedForm;
                    if (command.Parameters["@TotalCanceledForm"] != null && int.TryParse(command.Parameters["@TotalCanceledForm"].Value.ToString(), out totalCanceledForm))
                        pagedList.TotalCanceledForm = totalCanceledForm;
                    if (command.Parameters["@TotalWaitReExaminationForm"] != null && int.TryParse(command.Parameters["@TotalWaitReExaminationForm"].Value.ToString(), out totalWaitReExaminationForm))
                        pagedList.TotalWaitReExaminationForm = totalWaitReExaminationForm;
                    if (command.Parameters["@TotalConfirmedReExaminationForm"] != null && int.TryParse(command.Parameters["@TotalConfirmedReExaminationForm"].Value.ToString(), out totalConfirmedReExaminationForm))
                        pagedList.TotalConfirmedReExaminationForm = totalConfirmedReExaminationForm;
                    pagedList.Items = MappingDataTable.ConvertToList<ReportExaminationForm>(dataTable);
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
