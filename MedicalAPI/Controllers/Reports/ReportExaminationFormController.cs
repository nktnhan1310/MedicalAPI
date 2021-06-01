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
    [Route("api/report-examination-form")]
    [ApiController]
    [Description("Báo cáo lịch hẹn (tổng/ từng BV)")]
    public class ReportExaminationFormController : ReportCoreController<ReportExaminationForm, ReportExaminationFormModel, SearchReportRevenue>
    {
        private readonly IHospitalService hospitalService;
        public ReportExaminationFormController(IServiceProvider serviceProvider, ILogger<ReportCoreController<ReportExaminationForm, ReportExaminationFormModel, SearchReportRevenue>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IReportExaminationFormService>();
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
        }

        protected override string GetTemplateFilePath(string fileTemplateName)
        {
            return base.GetTemplateFilePath("ReportExaminationFormTemplate.xlsx");
        }

        protected override string GetReportName()
        {
            return "Report_Examination_Form";
        }

        protected override async Task<IDictionary<string, object>> GetParameterReport(PagedListReport<ReportExaminationFormModel> pagedListReport, SearchReportRevenue baseSearch)
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
            parameter.Add("TotalNewForm", pagedListReport.TotalNewForm);
            parameter.Add("TotalWaitConfirmForm", pagedListReport.TotalWaitConfirmForm);
            parameter.Add("TotalConfirmedForm", pagedListReport.TotalConfirmedForm);
            parameter.Add("TotalCanceledForm", pagedListReport.TotalCanceledForm);
            parameter.Add("TotalWaitReExaminationForm", pagedListReport.TotalWaitReExaminationForm);
            parameter.Add("TotalConfirmedReExaminationForm", pagedListReport.TotalConfirmedReExaminationForm);
            parameter.Add("TotalExamination", pagedListReport.TotalNewForm + pagedListReport.TotalWaitConfirmForm + pagedListReport.TotalConfirmedForm + pagedListReport.TotalWaitReExaminationForm + pagedListReport.TotalWaitReExaminationForm + pagedListReport.TotalConfirmedReExaminationForm);

            return parameter;
        }
    }
}
