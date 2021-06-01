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
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers.Reports
{
    [Route("api/report-user-examination-form")]
    [ApiController]
    [Description("Báo cáo số lượng người đã khám ngày/tháng/năm")]
    public class ReportUserExaminationFormController : ReportCoreController<ReportUserExaminationForm, ReportUserExaminationFormModel, SearchUserExaminationForm>
    {
        private readonly IHospitalService hospitalService;
        public ReportUserExaminationFormController(IServiceProvider serviceProvider, ILogger<ReportCoreController<ReportUserExaminationForm, ReportUserExaminationFormModel, SearchUserExaminationForm>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IReportUserExaminationService>();
            hospitalService = serviceProvider.GetRequiredService<IHospitalService>();
        }

        protected override string GetTemplateFilePath(string fileTemplateName)
        {
            return base.GetTemplateFilePath("ReportUserExaminationFormTemplate.xlsx");
        }

        protected override string GetReportName()
        {
            return "Report_UserExaminationForm";
        }

        protected override async Task<IDictionary<string, object>> GetParameterReport(PagedListReport<ReportUserExaminationFormModel> pagedList, SearchUserExaminationForm baseSearch)
        {
            IDictionary<string, object> parameter = new Dictionary<string, object>();
            string hospitalName = "Tất cả";
            if (baseSearch.HospitalId > 0)
            {
                var hospitalInfo = await this.hospitalService.GetByIdAsync(baseSearch.HospitalId);
                if (hospitalInfo != null)
                    hospitalName = hospitalInfo.Name;
            }
            string searchParam = string.Empty;
            switch (baseSearch.SelectedType)
            {
                case 0:
                    {
                        parameter.Add("SelectedType", "Ngày");
                        searchParam += string.Format("Theo ngày: {0}. Tháng: {1}. Năm {2}", baseSearch.ExaminationDate.HasValue ? baseSearch.ExaminationDate.Value.ToString("dd/MM/yyyy") : string.Empty, baseSearch.Month.HasValue ? baseSearch.Month.Value.ToString() : string.Empty, baseSearch.Year.HasValue ? baseSearch.Year.Value.ToString() : string.Empty);
                    }
                    break;
                case 1:
                    {
                        parameter.Add("SelectedType", "Tháng");
                        searchParam += string.Format("Tháng: {0}. Năm {1}", baseSearch.Month.HasValue ? baseSearch.Month.Value.ToString() : string.Empty, baseSearch.Year.HasValue ? baseSearch.Year.Value.ToString() : string.Empty);
                    }
                    break;
                case 2:
                    {
                        parameter.Add("SelectedType", "Năm");
                        searchParam += string.Format("Năm: {0}", baseSearch.Year.HasValue ? baseSearch.Year.Value.ToString() : string.Empty);
                    }
                    break;
                default:
                    {
                        parameter.Add("SelectedType", "Ngày");
                        searchParam += string.Format("Theo ngày: {0}. Tháng: {1}. Năm: {2}", baseSearch.ExaminationDate.HasValue ? baseSearch.ExaminationDate.Value.ToString("dd/MM/yyyy") : string.Empty, baseSearch.Month.HasValue ? baseSearch.Month.Value.ToString() : string.Empty, baseSearch.Year.HasValue ? baseSearch.Year.Value.ToString() : string.Empty);
                    }
                    break;
            }
            parameter.Add("SearchParam", searchParam);
            parameter.Add("HospitalParam", hospitalName);
            parameter.Add("TotalUserExamination", pagedList.TotalUserExamination);
            return parameter;
        }

        protected override async Task<byte[]> ExportChart(byte[] excelData, IList<ReportUserExaminationFormModel> listData)
        {
            return await Task.Run(() =>
            {
                int totalItem = listData.Count();
                int excelFromValue = 6;
                int excelToValue = 6;

                int excelFromTitle = 6;
                int excelToTitle = 6;

                if (listData.Any())
                {
                    excelToValue += (totalItem - 1);
                    excelToTitle += (totalItem - 1);

                    // Set again position chart for excel

                }

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    //Open Excel + Get WorkSheet
                    using (var memoryStream = new MemoryStream(excelData))
                    {
                        excelPackage.Load(memoryStream);
                    }
                    ExcelWorksheet ws = excelPackage.Workbook.Worksheets.First();
                    ExcelWorksheet wsChart = excelPackage.Workbook.Worksheets["Chart"];
                    var chart = wsChart.Drawings.AddChart("LineChartWithDroplines", eChartType.Line) as ExcelLineChart;
                    var serie = chart.Series.Add(ws.Cells[excelFromValue, 2, excelToValue, 2], ws.Cells[excelFromTitle, 1, excelToTitle, 1]);
                    serie.Header = "Số người dùng đã khám bệnh";
                    chart.SetPosition(0, 0, 0, 0);
                    chart.SetSize(1200, 400);
                    chart.Title.Text = "BÁO CÁO SỐ LƯỢNG NGƯỜI ĐÃ KHÁM";
                    excelData = excelPackage.GetAsByteArray();
                    return excelData;
                }
            });

        }

    }
}
