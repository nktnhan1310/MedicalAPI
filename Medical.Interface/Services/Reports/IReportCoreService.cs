using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Entities.DomainEntity.Search;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IReportCoreService<R, T> where R : ReportAppDomain where T : ReportBaseSearch
    {
        /// <summary>
        /// Lấy danh sách phân trang báo cáo
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        Task<PagedListReport<R>> GetPagedListReport(T baseSearch);

    }
}
