using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities.DomainEntity.Search
{

    public interface IReportBaseSearch
    {
        int PageIndex { set; get; }
        int PageSize { set; get; }
        string SearchContent { set; get; }
        string OrderBy { set; get; }
    }

    public class ReportBaseSearch : IReportBaseSearch
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int PageIndex { set; get; }
        /// <summary>
        /// Số lượng item trên 1 trang
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// Nội dung tìm kiếm chung
        /// </summary>
        [StringLength(1000, ErrorMessage = "Nội dung không vượt quá 1000 kí tự")]
        public string SearchContent { set; get; }
        /// <summary>
        /// Cột sắp xếp
        /// </summary>
        public string OrderBy { set; get; }
        /// <summary>
        /// Xuất excel ko?
        /// </summary>
        [DefaultValue(false)]
        public bool IsExport { get; set; }

        /// <summary>
        /// Theo bệnh viện
        /// </summary>
        public int? HospitalId { get; set; }
    }
}
