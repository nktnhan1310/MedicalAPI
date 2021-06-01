using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Entities.Reports;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers.Reports
{
    [Route("api/report-revenue")]
    [ApiController]
    [Description("Báo cáo doanh thu (tổng/ từng BV)")]
    public class ReportRevenueController : ReportCoreController<ReportRevenue, ReportRevenueModel, SearchReportRevenue>
    {
        private readonly IHospitalService hospitalService;
        public ReportRevenueController(IServiceProvider serviceProvider, ILogger<ReportCoreController<ReportRevenue, ReportRevenueModel, SearchReportRevenue>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IReportRevenueService>();
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
        }

        protected override string GetTemplateFilePath(string fileTemplateName)
        {
            return base.GetTemplateFilePath("ReportRevenueTemplate.xlsx");
        }

        protected override string GetReportName()
        {
            return "Report_Revenue";
        }

        protected override async Task<IDictionary<string, object>> GetParameterReport(PagedListReport<ReportRevenueModel> pagedListReport, SearchReportRevenue baseSearch)
        {
            IDictionary<string, object> parameter = new Dictionary<string, object>();
            string hospitalName = "Tất cả";
            if (baseSearch.HospitalId.HasValue)
            {
                var hospitalInfo = await this.hospitalService.GetByIdAsync(baseSearch.HospitalId.Value);
                if (hospitalInfo != null)
                    hospitalName = hospitalInfo.Name;
            }
            parameter.Add("HospitalParam", hospitalName);
            parameter.Add("TotalAppPrice", pagedListReport.TotalRevenueValue.HasValue ? pagedListReport.TotalRevenueValue.Value.ToString("#,###") : string.Empty);
            return parameter;
        }
    }
}
