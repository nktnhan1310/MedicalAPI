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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class ReportUserExaminationService : ReportCoreService<ReportUserExaminationForm, SearchUserExaminationForm>, IReportUserExaminationService
    {
        public ReportUserExaminationService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IMedicalDbContext context) : base(unitOfWork, mapper, context)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Report_UserExaminationForm";
        }

        protected override SqlParameter[] GetSqlParameters(SearchUserExaminationForm baseSearch)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@SelectedType", baseSearch.SelectedType),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@Month", baseSearch.Month),
                new SqlParameter("@Year", baseSearch.Year),

                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                new SqlParameter("@TotalUserExamination", SqlDbType.Int, 0),

            };
            return parameters;
        }

        public override async Task<PagedListReport<ReportUserExaminationForm>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedListReport<ReportUserExaminationForm> pagedList = new PagedListReport<ReportUserExaminationForm>();
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
                    command.Parameters["@TotalUserExamination"].Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    //pagedList.TotalItem = int.Parse(command.Parameters["@TotalPage"].Value.ToString());
                    pagedList.TotalUserExamination = int.Parse(command.Parameters["@TotalUserExamination"].Value.ToString());
                    pagedList.Items = MappingDataTable.ConvertToList<ReportUserExaminationForm>(dataTable);
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
