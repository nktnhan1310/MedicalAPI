using AutoMapper;
using Medical.Entities;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class DashBoardService : IDashBoardService
    {
        protected readonly IMedicalDbContext Context;
        protected readonly IMedicalUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        public DashBoardService(IMedicalUnitOfWork medicalUnitOfWork, IMedicalDbContext Context, IMapper mapper)
        {
            this.unitOfWork = medicalUnitOfWork;
            this.Context = Context;
            this.mapper = mapper;
        }

        /// <summary>
        /// Lấy tổng số lịch khám theo trạng thái của lịch
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        public async Task<DashBoardResponse> GetTotalExaminationByRequest(DashBoardRequest dashBoardRequest)
        {
            return await Task.Run(() =>
            {
                DashBoardResponse dashBoardResponse = new DashBoardResponse();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@HospitalId", dashBoardRequest.HospitalId),
                        new SqlParameter("@StatusList", dashBoardRequest.StatusList),
                        new SqlParameter("@ExaminationDate", dashBoardRequest.ExaminationDate),
                        new SqlParameter("@MonthValue", dashBoardRequest.MonthValue),
                        new SqlParameter("@YearValue", dashBoardRequest.YearValue),
                        new SqlParameter("@ServiceTypeId", dashBoardRequest.ServiceTypeId),
                        new SqlParameter("@TotalExaminationByDate", SqlDbType.Int, 0),
                        new SqlParameter("@TotalExaminationByMonth", SqlDbType.Int, 0),
                        new SqlParameter("@TotalExaminationByYear", SqlDbType.Int, 0),
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalExaminationByStatus";
                    command.Parameters.AddRange(sqlParameters);
                    command.Parameters["@TotalExaminationByDate"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalExaminationByMonth"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalExaminationByYear"].Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    dashBoardResponse.TotalExaminationFormByDate = int.Parse(command.Parameters["@TotalExaminationByDate"].Value.ToString());
                    dashBoardResponse.TotalExaminationFormByMonth = int.Parse(command.Parameters["@TotalExaminationByMonth"].Value.ToString());
                    dashBoardResponse.TotalExaminationFormByYear = int.Parse(command.Parameters["@TotalExaminationByYear"].Value.ToString());
                    return dashBoardResponse;
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

        /// <summary>
        /// Lấy tổng user của hệ thống
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        public async Task<DashBoardResponse> GetTotalUser(DashBoardRequest dashBoardRequest)
        {
            return await Task.Run(() =>
            {
                DashBoardResponse dashBoardResponse = new DashBoardResponse();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@IsCheckActiveUser", dashBoardRequest.IsCheckActiveUser),
                        new SqlParameter("@TotalUserByDate", SqlDbType.Int, 0),
                        new SqlParameter("@TotalUserByMonth", SqlDbType.Int, 0),
                        new SqlParameter("@TotalUserByYear", SqlDbType.Int, 0),
                        new SqlParameter("@TotalUserActive", SqlDbType.Int, 0),

                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalUser";
                    command.Parameters.AddRange(sqlParameters);
                    command.Parameters["@TotalUserByDate"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalUserByMonth"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalUserByYear"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalUserActive"].Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    int totalUserActive = 0;
                    int totalUserByDate = 0;
                    int totalUserByMonth = 0;
                    int totalUserByYear = 0;
                    if (int.TryParse(command.Parameters["@TotalUserByDate"].Value.ToString(), out totalUserByDate))
                        dashBoardResponse.TotalUserByDate = totalUserByDate;
                    if (int.TryParse(command.Parameters["@TotalUserByMonth"].Value.ToString(), out totalUserByMonth))
                        dashBoardResponse.TotalUserByMonth = totalUserByMonth;
                    if (int.TryParse(command.Parameters["@TotalUserByYear"].Value.ToString(), out totalUserByYear))
                        dashBoardResponse.TotalUserByYear = totalUserByYear;
                    if (int.TryParse(command.Parameters["@TotalUserActive"].Value.ToString(), out totalUserActive))
                        dashBoardResponse.TotalUserActive = totalUserActive;
                    return dashBoardResponse;
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

        /// <summary>
        /// Lấy tổng user đã khám bệnh
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        public async Task<DashBoardResponse> GetTotalUserExamination(DashBoardRequest dashBoardRequest)
        {
            return await Task.Run(() =>
            {
                DashBoardResponse dashBoardResponse = new DashBoardResponse();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@HospitalId", dashBoardRequest.HospitalId),
                        new SqlParameter("@ExaminationDate", dashBoardRequest.ExaminationDate),
                        new SqlParameter("@Month", dashBoardRequest.MonthValue),
                        new SqlParameter("@Year", dashBoardRequest.YearValue),
                        new SqlParameter("@TotalUserByDate", SqlDbType.Int, 0),
                        new SqlParameter("@TotalUserByMonth", SqlDbType.Int, 0),
                        new SqlParameter("@TotalUserByYear", SqlDbType.Int, 0),
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalUserExamination";
                    command.Parameters.AddRange(sqlParameters);
                    command.Parameters["@TotalUserByDate"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalUserByMonth"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalUserByYear"].Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    int totalUserByDate = 0;
                    int totalUserByMonth = 0;
                    int totalUserByYear = 0;
                    if (int.TryParse(command.Parameters["@TotalUserByDate"].Value.ToString(), out totalUserByDate))
                        dashBoardResponse.TotalUserByDate = totalUserByDate;
                    if (int.TryParse(command.Parameters["@TotalUserByMonth"].Value.ToString(), out totalUserByMonth))
                        dashBoardResponse.TotalUserByMonth = totalUserByMonth;
                    if (int.TryParse(command.Parameters["@TotalUserByYear"].Value.ToString(), out totalUserByYear))
                        dashBoardResponse.TotalUserByYear = totalUserByYear;
                    return dashBoardResponse;
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

        /// <summary>
        /// Lấy tổng số tiền thanh toán của hệ thống
        /// </summary>
        /// <param name="dashBoardRequest"></param>
        /// <returns></returns>
        public async Task<DashBoardResponse> GetTotalPaymentSystem(DashBoardRequest dashBoardRequest)
        {
            return await Task.Run(() =>
            {
                DashBoardResponse dashBoardResponse = new DashBoardResponse();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@HospitalId", dashBoardRequest.HospitalId),
                        new SqlParameter("@ExaminationDate", dashBoardRequest.ExaminationDate),
                        new SqlParameter("@Month", dashBoardRequest.MonthValue),
                        new SqlParameter("@Year", dashBoardRequest.YearValue),
                        new SqlParameter("@TotalAppPriceByDate", SqlDbType.Float, 0),
                        new SqlParameter("@TotalAppPriceByMonth", SqlDbType.Float, 0),
                        new SqlParameter("@TotalAppPriceByYear", SqlDbType.Float, 0),
                        new SqlParameter("@TotalCODPriceByDate", SqlDbType.Float, 0),
                        new SqlParameter("@TotalCODPriceByMonth", SqlDbType.Float, 0),
                        new SqlParameter("@TotalCODPriceByYear", SqlDbType.Float, 0),
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalPayment";
                    command.Parameters.AddRange(sqlParameters);
                    command.Parameters["@TotalAppPriceByDate"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalAppPriceByMonth"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalAppPriceByYear"].Direction = ParameterDirection.Output;

                    command.Parameters["@TotalCODPriceByDate"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalCODPriceByMonth"].Direction = ParameterDirection.Output;
                    command.Parameters["@TotalCODPriceByYear"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    double totalAppPriceByDate = 0;
                    double totalAppPriceByMonth = 0;
                    double totalAppPriceByYear = 0;

                    double totalCODPriceByDate = 0;
                    double totalCODPriceByMonth = 0;
                    double totalCODPriceByYear = 0;

                    if (double.TryParse(command.Parameters["@TotalAppPriceByDate"].Value.ToString(), out totalAppPriceByDate))
                        dashBoardResponse.TotalAppPriceByDate = totalAppPriceByDate;
                    if (double.TryParse(command.Parameters["@TotalAppPriceByMonth"].Value.ToString(), out totalAppPriceByMonth))
                        dashBoardResponse.TotalAppPriceByMonth = totalAppPriceByMonth;
                    if (double.TryParse(command.Parameters["@TotalAppPriceByYear"].Value.ToString(), out totalAppPriceByYear))
                        dashBoardResponse.TotalAppPriceByYear = totalAppPriceByYear;


                    if (double.TryParse(command.Parameters["@TotalCODPriceByDate"].Value.ToString(), out totalCODPriceByDate))
                        dashBoardResponse.TotalCODPriceByDate = totalCODPriceByDate;
                    if (double.TryParse(command.Parameters["@TotalCODPriceByMonth"].Value.ToString(), out totalCODPriceByMonth))
                        dashBoardResponse.TotalCODPriceByMonth = totalCODPriceByMonth;
                    if (double.TryParse(command.Parameters["@TotalCODPriceByYear"].Value.ToString(), out totalCODPriceByYear))
                        dashBoardResponse.TotalCODPriceByYear = totalCODPriceByYear;
                    return dashBoardResponse;
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
