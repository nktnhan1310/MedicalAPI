using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Entities
{
    public interface IBaseSearch
    {
        int PageIndex { set; get; }
        int PageSize { set; get; }
        string SearchContent { set; get; }
        string OrderBy { set; get; }
        string FileName { set; get; }
    }

    public class BaseSearch: IBaseSearch
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
        public string FileName { set; get; }
    }
}
