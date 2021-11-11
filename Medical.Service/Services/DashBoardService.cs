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
        /// Lấy thông tin user hệ thống theo filter
        /// </summary>
        /// <param name="searchDashBoardUserSystem"></param>
        /// <returns></returns>
        public async Task<double> GetTotalUserSystem(SearchDashBoardUserSystem searchDashBoardUserSystem)
        {
            return await Task.Run(() =>
            {
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@HospitalId", searchDashBoardUserSystem.HospitalId),
                        new SqlParameter("@UserGroupId", searchDashBoardUserSystem.UserGroupId),
                        new SqlParameter("@IsLocked", searchDashBoardUserSystem.IsLocked),
                        new SqlParameter("@IsDeleted", searchDashBoardUserSystem.IsDeleted),
                        new SqlParameter("@IsActive", searchDashBoardUserSystem.IsActive),
                        new SqlParameter("@TotalUser", SqlDbType.Float, 0),

                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalUserSystemV2";
                    command.Parameters.AddRange(sqlParameters);
                    command.Parameters["@TotalUser"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    double totalUser = 0;
                    double.TryParse(command.Parameters["@TotalUser"].Value.ToString(), out totalUser);
                    return totalUser;
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

        /// <summary>
        /// Lấy thông tin chi phí của hệ thống theo filter
        /// </summary>
        /// <param name="searchDashBoardTotalPayment"></param>
        /// <returns></returns>
        public async Task<double> GetTotalPaymentV2(SearchDashBoardTotalPayment searchDashBoardTotalPayment)
        {
            return await Task.Run(() =>
            {
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@HospitalId", searchDashBoardTotalPayment.HospitalId),
                        new SqlParameter("@PaymentMethodId", searchDashBoardTotalPayment.PaymentMethodId),
                        new SqlParameter("@Status", searchDashBoardTotalPayment.Status),
                        new SqlParameter("@TotalAmount", SqlDbType.Float, 0),
                        
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalPaymentV2";
                    command.Parameters.AddRange(sqlParameters);
                    command.Parameters["@TotalAmount"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    double totalAmount = 0;
                    double.TryParse(command.Parameters["@TotalAmount"].Value.ToString(), out totalAmount);
                    return totalAmount;
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
        /// Lấy thông tin báo cáo tổng hợp
        /// </summary>
        /// <param name="dashBoardSynthesisRequest"></param>
        /// <returns></returns>
        public async Task<List<DashBoardSynthesisResponse>> GetSynthesisReport(DashBoardSynthesisRequest dashBoardSynthesisRequest)
        {
            return await Task.Run(() =>
            {
                List<DashBoardSynthesisResponse> results = new List<DashBoardSynthesisResponse>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                switch (dashBoardSynthesisRequest.SelectedType)
                {
                    // Tháng
                    case 1:
                        {
                            int monthValue = dashBoardSynthesisRequest.MonthValue.HasValue ? dashBoardSynthesisRequest.MonthValue.Value : DateTime.Now.Month;
                            int yearValue = dashBoardSynthesisRequest.YearValue.HasValue ? dashBoardSynthesisRequest.YearValue.Value : DateTime.Now.Year;
                            dashBoardSynthesisRequest.YearValue = yearValue;
                            dashBoardSynthesisRequest.MonthValue = monthValue;
                            dashBoardSynthesisRequest.FromDate = new DateTime(yearValue, monthValue, 1);
                            dashBoardSynthesisRequest.ToDate = dashBoardSynthesisRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                        }
                        break;
                    // Năm
                    case 0:
                    default:
                        {
                            dashBoardSynthesisRequest.YearValue = dashBoardSynthesisRequest.YearValue.HasValue ? dashBoardSynthesisRequest.YearValue.Value : DateTime.Now.Year;
                            //dashBoardSynthesisRequest.SelectedType = 0;
                        }
                        break;
                }


                try
                {
                    DateTime? examinationDate = null;
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@PageIndex", 1),
                        new SqlParameter("@PageSize", int.MaxValue),
                        new SqlParameter("@HospitalId", dashBoardSynthesisRequest.HospitalId),
                        new SqlParameter("@StatusList", dashBoardSynthesisRequest.StatusIds != null && dashBoardSynthesisRequest.StatusIds.Any() ? string.Join(',', dashBoardSynthesisRequest.StatusIds) : string.Empty),
                        new SqlParameter("@SelectedType", dashBoardSynthesisRequest.SelectedType),
                        new SqlParameter("@ExaminationDate", examinationDate),
                        new SqlParameter("@YearValue", dashBoardSynthesisRequest.YearValue),
                        new SqlParameter("@ServiceTypeId", dashBoardSynthesisRequest.ServiceTypeId),

                        new SqlParameter("@FromDate", dashBoardSynthesisRequest.FromDate),
                        new SqlParameter("@ToDate", dashBoardSynthesisRequest.ToDate),
                        new SqlParameter("@OrderBy", "ExaminationDate asc"),
                        //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalExaminationByStatusV1";
                    command.Parameters.AddRange(sqlParameters);
                    //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    results = MappingDataTable.ConvertToList<DashBoardSynthesisResponse>(dataTable);

                    if(results != null && results.Any())
                    {
                        switch (dashBoardSynthesisRequest.SelectedType)
                        {
                            // Theo năm
                            case 0:
                                {
                                    for (int i = 1; i <= 12; i++)
                                    {
                                        if (results.Any(x => x.MonthValue == i))
                                            continue;
                                        else
                                        {
                                            results.Add(new DashBoardSynthesisResponse()
                                            {
                                                YearValue = dashBoardSynthesisRequest.YearValue,
                                                MonthValue = i,
                                                TotalCancelExamination = 0,
                                                TotalExaminationForm = 0,
                                                TotalUserExamination = 0,
                                            });
                                        }
                                    }
                                    results = results.OrderBy(e => e.MonthValue).ToList();
                                }
                                break;
                            // Theo ngày
                            default:
                                {
                                    DateTime? firstDateExamination = null;
                                    DateTime? lastDateExamination = null;

                                    if(dashBoardSynthesisRequest.SelectedType == 1)
                                    {
                                        firstDateExamination = new DateTime(dashBoardSynthesisRequest.YearValue.Value, dashBoardSynthesisRequest.MonthValue.Value, 1);
                                        lastDateExamination = dashBoardSynthesisRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {

                                        firstDateExamination = dashBoardSynthesisRequest.FromDate;
                                        lastDateExamination = dashBoardSynthesisRequest.ToDate;

                                        //firstDateExamination = results.FirstOrDefault().ExaminationDate;
                                        //lastDateExamination = results.OrderByDescending(e => e.ExaminationDate).FirstOrDefault().ExaminationDate;
                                    }
                                    if (firstDateExamination.HasValue && lastDateExamination.HasValue)
                                    {
                                        while (firstDateExamination.Value <= lastDateExamination.Value)
                                        {
                                            if (results.Any(e => e.ExaminationDate == firstDateExamination))
                                            {
                                                firstDateExamination = firstDateExamination.Value.AddDays(1);
                                                continue;
                                            }
                                            else
                                            {
                                                results.Add(new DashBoardSynthesisResponse()
                                                {
                                                    ExaminationDate = firstDateExamination,
                                                    TotalCancelExamination = 0,
                                                    TotalExaminationForm = 0,
                                                    TotalUserExamination = 0,
                                                });
                                                firstDateExamination = firstDateExamination.Value.AddDays(1);
                                            }
                                        }
                                        results = results.OrderBy(e => e.ExaminationDate).ToList();
                                    }

                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (dashBoardSynthesisRequest.SelectedType)
                        {
                            // Theo năm
                            case 0:
                                {
                                    for (int i = 1; i <= 12; i++)
                                    {
                                        results.Add(new DashBoardSynthesisResponse()
                                        {
                                            YearValue = dashBoardSynthesisRequest.YearValue,
                                            MonthValue = i,
                                            TotalCancelExamination = 0,
                                            TotalExaminationForm = 0,
                                            TotalUserExamination = 0,
                                        });
                                    }
                                    results = results.OrderBy(e => e.MonthValue).ToList();
                                }
                                break;
                            // Theo ngày
                            default:
                                {
                                    DateTime? firstDateExamination = null;
                                    DateTime? lastDateExamination = null;

                                    if (dashBoardSynthesisRequest.SelectedType == 1)
                                    {
                                        firstDateExamination = new DateTime(dashBoardSynthesisRequest.YearValue.Value, dashBoardSynthesisRequest.MonthValue.Value, 1);
                                        lastDateExamination = dashBoardSynthesisRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        firstDateExamination = dashBoardSynthesisRequest.FromDate;
                                        lastDateExamination = dashBoardSynthesisRequest.ToDate;
                                    }
                                    if (firstDateExamination.HasValue && lastDateExamination.HasValue)
                                    {
                                        while (firstDateExamination.Value <= lastDateExamination.Value)
                                        {
                                            results.Add(new DashBoardSynthesisResponse()
                                            {
                                                ExaminationDate = firstDateExamination,
                                                TotalCancelExamination = 0,
                                                TotalExaminationForm = 0,
                                                TotalUserExamination = 0,
                                            });
                                            firstDateExamination = firstDateExamination.Value.AddDays(1);
                                        }
                                        results = results.OrderBy(e => e.ExaminationDate).ToList();
                                    }

                                }
                                break;
                        }
                    }

                    return results;
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
        /// Lấy thông tin báo cáo tổng hợp
        /// </summary>
        /// <param name="dashBoardSynthesisRequest"></param>
        /// <returns></returns>
        public async Task<List<DashBoardSynthesisByHospitalResponse>> GetSynthesisReportByHospital(DashBoardSynthesisRequest dashBoardSynthesisRequest)
        {
            return await Task.Run(() =>
            {
                List<DashBoardSynthesisByHospitalResponse> results = new List<DashBoardSynthesisByHospitalResponse>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                switch (dashBoardSynthesisRequest.SelectedType)
                {
                    // Tháng
                    case 1:
                        {
                            int monthValue = dashBoardSynthesisRequest.MonthValue.HasValue ? dashBoardSynthesisRequest.MonthValue.Value : DateTime.Now.Month;
                            int yearValue = dashBoardSynthesisRequest.YearValue.HasValue ? dashBoardSynthesisRequest.YearValue.Value : DateTime.Now.Year;
                            dashBoardSynthesisRequest.YearValue = yearValue;
                            dashBoardSynthesisRequest.MonthValue = monthValue;
                            dashBoardSynthesisRequest.FromDate = new DateTime(yearValue, monthValue, 1);
                            dashBoardSynthesisRequest.ToDate = dashBoardSynthesisRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                        }
                        break;
                    // Năm
                    case 0:
                    default:
                        {
                            dashBoardSynthesisRequest.YearValue = dashBoardSynthesisRequest.YearValue.HasValue ? dashBoardSynthesisRequest.YearValue.Value : DateTime.Now.Year;
                            //dashBoardSynthesisRequest.SelectedType = 0;
                        }
                        break;
                }
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@PageIndex", 1),
                        new SqlParameter("@PageSize", int.MaxValue),
                        new SqlParameter("@HospitalId", dashBoardSynthesisRequest.HospitalId),
                        new SqlParameter("@FromDate", dashBoardSynthesisRequest.FromDate),
                        new SqlParameter("@ToDate", dashBoardSynthesisRequest.ToDate),
                        new SqlParameter("@Year", dashBoardSynthesisRequest.YearValue),
                        //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetSynthesysReportHospital";
                    command.Parameters.AddRange(sqlParameters);
                    //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    results = MappingDataTable.ConvertToList<DashBoardSynthesisByHospitalResponse>(dataTable);
                    return results;
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
        /// Lấy thông tin báo cáo doanh thu
        /// </summary>
        /// <param name="dashBoardSaleRequest"></param>
        /// <returns></returns>
        public async Task<List<DashBoardSaleResponse>> GetSaleReport(DashBoardSaleRequest dashBoardSaleRequest)
        {
            return await Task.Run(() =>
            {
                List<DashBoardSaleResponse> results = new List<DashBoardSaleResponse>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                switch (dashBoardSaleRequest.SelectedType)
                {
                    // Tháng
                    case 1:
                        {
                            int monthValue = dashBoardSaleRequest.Month.HasValue ? dashBoardSaleRequest.Month.Value : DateTime.Now.Month;
                            int yearValue = dashBoardSaleRequest.Year.HasValue ? dashBoardSaleRequest.Year.Value : DateTime.Now.Year;
                            dashBoardSaleRequest.Year = yearValue;
                            dashBoardSaleRequest.Month = monthValue;
                            dashBoardSaleRequest.FromDate = new DateTime(yearValue, monthValue, 1);
                            dashBoardSaleRequest.ToDate = dashBoardSaleRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                        }
                        break;
                    // Năm
                    case 0:
                    default:
                        {
                            dashBoardSaleRequest.Year = dashBoardSaleRequest.Year.HasValue ? dashBoardSaleRequest.Year.Value : DateTime.Now.Year;
                            //dashBoardSynthesisRequest.SelectedType = 0;
                        }
                        break;
                }


                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@PageIndex", 1),
                        new SqlParameter("@PageSize", int.MaxValue),
                        new SqlParameter("@HospitalId", dashBoardSaleRequest.HospitalId),
                        
                        new SqlParameter("@SelectedType", dashBoardSaleRequest.SelectedType),
                        new SqlParameter("@FromDate", dashBoardSaleRequest.FromDate),
                        new SqlParameter("@ToDate", dashBoardSaleRequest.ToDate),
                        new SqlParameter("@Year", dashBoardSaleRequest.Year),
                        //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalPaymentReport";
                    command.Parameters.AddRange(sqlParameters);
                    //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    results = MappingDataTable.ConvertToList<DashBoardSaleResponse>(dataTable);

                    if (results != null && results.Any())
                    {
                        switch (dashBoardSaleRequest.SelectedType)
                        {
                            // Theo năm
                            case 0:
                                {
                                    for (int i = 1; i <= 12; i++)
                                    {
                                        if (results.Any(x => x.MonthValue == i))
                                            continue;
                                        else
                                        {
                                            results.Add(new DashBoardSaleResponse()
                                            {
                                                YearValue = dashBoardSaleRequest.Year,
                                                MonthValue = i,
                                            });
                                        }
                                    }
                                    results = results.OrderBy(e => e.MonthValue).ToList();
                                }
                                break;
                            // Theo ngày
                            default:
                                {
                                    DateTime? firstDateExamination = null;
                                    DateTime? lastDateExamination = null;

                                    if (dashBoardSaleRequest.SelectedType == 1)
                                    {
                                        firstDateExamination = new DateTime(dashBoardSaleRequest.Year.Value, dashBoardSaleRequest.Month.Value, 1);
                                        lastDateExamination = dashBoardSaleRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        firstDateExamination = dashBoardSaleRequest.FromDate;
                                        lastDateExamination = dashBoardSaleRequest.ToDate;

                                        //firstDateExamination = results.FirstOrDefault().PaymentDate;
                                        //lastDateExamination = results.OrderByDescending(e => e.PaymentDate).FirstOrDefault().PaymentDate;
                                    }
                                    if (firstDateExamination.HasValue && lastDateExamination.HasValue)
                                    {
                                        while (firstDateExamination.Value <= lastDateExamination.Value)
                                        {
                                            if (results.Any(e => e.PaymentDate == firstDateExamination))
                                            {
                                                firstDateExamination = firstDateExamination.Value.AddDays(1);
                                                continue;
                                            }
                                            else
                                            {
                                                results.Add(new DashBoardSaleResponse()
                                                {
                                                    PaymentDate = firstDateExamination,
                                                });
                                                firstDateExamination = firstDateExamination.Value.AddDays(1);
                                            }
                                        }
                                        results = results.OrderBy(e => e.PaymentDate).ToList();
                                    }

                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (dashBoardSaleRequest.SelectedType)
                        {
                            // Theo năm
                            case 0:
                                {
                                    for (int i = 1; i <= 12; i++)
                                    {
                                        results.Add(new DashBoardSaleResponse()
                                        {
                                            YearValue = dashBoardSaleRequest.Year,
                                            MonthValue = i,
                                        });
                                    }
                                    results = results.OrderBy(e => e.MonthValue).ToList();
                                }
                                break;
                            // Theo ngày
                            default:
                                {
                                    DateTime? firstDateExamination = null;
                                    DateTime? lastDateExamination = null;

                                    if (dashBoardSaleRequest.SelectedType == 1)
                                    {
                                        firstDateExamination = new DateTime(dashBoardSaleRequest.Year.Value, dashBoardSaleRequest.Month.Value, 1);
                                        lastDateExamination = dashBoardSaleRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        firstDateExamination = dashBoardSaleRequest.FromDate;
                                        lastDateExamination = dashBoardSaleRequest.ToDate;
                                    }
                                    if (firstDateExamination.HasValue && lastDateExamination.HasValue)
                                    {
                                        while (firstDateExamination.Value <= lastDateExamination.Value)
                                        {
                                            results.Add(new DashBoardSaleResponse()
                                            {
                                                PaymentDate = firstDateExamination,
                                            });
                                            firstDateExamination = firstDateExamination.Value.AddDays(1);
                                        }
                                        results = results.OrderBy(e => e.PaymentDate).ToList();
                                    }

                                }
                                break;
                        }
                    }

                    return results;
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
        /// Lấy thông tin báo cáo tổng hợp
        /// </summary>
        /// <param name="dashBoardSaleRequest"></param>
        /// <returns></returns>
        public async Task<List<DashBoardSaleByHospitalResponse>> GetSaleReportByHospital(DashBoardSaleRequest dashBoardSaleRequest)
        {
            return await Task.Run(() =>
            {
                List<DashBoardSaleByHospitalResponse> results = new List<DashBoardSaleByHospitalResponse>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                switch (dashBoardSaleRequest.SelectedType)
                {
                    // Tháng
                    case 1:
                        {
                            int monthValue = dashBoardSaleRequest.Month.HasValue ? dashBoardSaleRequest.Month.Value : DateTime.Now.Month;
                            int yearValue = dashBoardSaleRequest.Year.HasValue ? dashBoardSaleRequest.Year.Value : DateTime.Now.Year;
                            dashBoardSaleRequest.Year = yearValue;
                            dashBoardSaleRequest.Month = monthValue;
                            dashBoardSaleRequest.FromDate = new DateTime(yearValue, monthValue, 1);
                            dashBoardSaleRequest.ToDate = dashBoardSaleRequest.FromDate.Value.AddMonths(1).AddDays(-1);
                        }
                        break;
                    // Năm
                    case 0:
                    default:
                        {
                            dashBoardSaleRequest.Year = dashBoardSaleRequest.Year.HasValue ? dashBoardSaleRequest.Year.Value : DateTime.Now.Year;
                        }
                        break;
                }
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@PageIndex", 1),
                        new SqlParameter("@PageSize", int.MaxValue),
                        new SqlParameter("@HospitalId", dashBoardSaleRequest.HospitalId),
                        new SqlParameter("@FromDate", dashBoardSaleRequest.FromDate),
                        new SqlParameter("@ToDate", dashBoardSaleRequest.ToDate),
                        new SqlParameter("@Year", dashBoardSaleRequest.Year),
                        //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                    };
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "DashBoard_GetTotalPaymentReportByHospital";
                    command.Parameters.AddRange(sqlParameters);
                    //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;

                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    results = MappingDataTable.ConvertToList<DashBoardSaleByHospitalResponse>(dataTable);
                    return results;
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
