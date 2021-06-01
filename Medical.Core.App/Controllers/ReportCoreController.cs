using AutoMapper;
using Medical.Entities;
using Medical.Entities.DomainEntity.Search;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models.DomainModel;
using Medical.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Medical.Core.App.Controllers
{
    [ApiController]
    public abstract class ReportCoreController<R, M, T> : ControllerBase where R : ReportAppDomain where M : ReportAppDomainModel, new() where T : ReportBaseSearch, new()
    {
        protected readonly ILogger<ReportCoreController<R, M, T>> logger;
        protected readonly IServiceProvider serviceProvider;
        protected readonly IMapper mapper;
        protected IReportCoreService<R, T> domainService;
        protected IWebHostEnvironment env;
        public ReportCoreController(IServiceProvider serviceProvider, ILogger<ReportCoreController<R, M, T>> logger, IWebHostEnvironment env)
        {
            this.env = env;
            this.logger = logger;
            this.mapper = serviceProvider.GetRequiredService<IMapper>();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Lấy danh sách phân trang báo cáo
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-paged-list")]
        public virtual async Task<AppDomainResult> GetPagedListReport([FromQuery] T baseSearch)
        {
            var pagedList = await this.domainService.GetPagedListReport(baseSearch);
            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK,
                Data = mapper.Map<PagedListReport<M>>(pagedList)
            };
        }

        /// <summary>
        /// Xuất báo cáo
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpPost("export-report")]
        public virtual async Task<AppDomainResult> ExportReport([FromQuery] T baseSearch)
        {
            string fileResultPath = string.Empty;
            PagedListReport<M> pagedListModel = new PagedListReport<M>();
            // ------------------------------------------LẤY THÔNG TIN XUẤT EXCEL
            // 1. LẤY THÔNG TIN DATA VÀ ĐỔ DATA VÀO TEMPLATE
            PagedListReport<R> pagedList = await this.domainService.GetPagedListReport(baseSearch);
            pagedListModel = mapper.Map<PagedListReport<M>>(pagedList);
            ExcelUtility excelUtility = new ExcelUtility();
            // 2. LẤY THÔNG TIN FILE TEMPLATE ĐỂ EXPORT
            excelUtility.TemplateFileData = System.IO.File.ReadAllBytes(GetTemplateFilePath(string.Empty));
            // 3. LẤY THÔNG TIN THAM SỐ TRUYỀN VÀO
            excelUtility.ParameterData = await GetParameterReport(pagedListModel, baseSearch);
            if (pagedListModel.Items == null || !pagedListModel.Items.Any())
                pagedListModel.Items.Add(new M());
            byte[] fileByteReport = excelUtility.Export(pagedListModel.Items);
            // Xuất biểu đồ nếu có
            fileByteReport = await this.ExportChart(fileByteReport, pagedListModel.Items);
            // 4. LƯU THÔNG TIN FILE BÁO CÁO XUỐNG FOLDER BÁO CÁO
            string fileName = string.Format("{0}-{1}.xlsx", Guid.NewGuid().ToString(), GetReportName());
            string fileUploadPath = Path.Combine(env.ContentRootPath, UPLOAD_FOLDER_NAME, REPORT_FOLDER_NAME);
            string path = Path.Combine(fileUploadPath, fileName);
            FileUtils.CreateDirectory(fileUploadPath);
            FileUtils.SaveToPath(path, fileByteReport);
            // 5. TRẢ ĐƯỜNG DẪN FILE CHO CLIENT DOWN VỀ
            //var currentLinkSite = $"{Medical.Extensions.HttpContext.Current.Request.Scheme}://{Medical.Extensions.HttpContext.Current.Request.Host}/{UPLOAD_FOLDER_NAME}/{REPORT_FOLDER_NAME}/";
            fileResultPath = Path.Combine(UPLOAD_FOLDER_NAME, REPORT_FOLDER_NAME, Path.GetFileName(path));
            return new AppDomainResult()
            {
                Data = fileResultPath,
                ResultCode = (int)HttpStatusCode.OK,
                Success = true,
            };
        }

        protected virtual async Task<byte[]> ExportChart(byte[] excelData, IList<M> listData)
        {
            return excelData;
        }

        protected virtual string GetReportName()
        {
            return string.Empty;
        }

        /// <summary>
        /// Lấy đường dẫn file template
        /// </summary>
        /// <param name="fileTemplateName"></param>
        /// <returns></returns>
        protected virtual string GetTemplateFilePath(string fileTemplateName)
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, TEMP_FOLDER_NAME, fileTemplateName);
            if (!System.IO.File.Exists(path))
                throw new AppException("File template không tồn tại!");
            return path;
        }

        /// <summary>
        /// Lấy thông số parameter truyền vào
        /// </summary>
        /// <param name="pagedList"></param>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        protected virtual async Task<IDictionary<string, object>> GetParameterReport(PagedListReport<M> pagedList, T baseSearch)
        {
            return await Task.Run(() =>
            {
                IDictionary<string, object> dictionaries = new Dictionary<string, object>();
                return dictionaries;
            });
        }

        #region Contants

        public const string TEMP_FOLDER_NAME = "Template";
        public const string UPLOAD_FOLDER_NAME = "upload";
        public const string REPORT_FOLDER_NAME = "report";

        #endregion

    }
}
