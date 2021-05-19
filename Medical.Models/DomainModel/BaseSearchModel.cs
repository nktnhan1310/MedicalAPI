using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models.DomainModel
{
    public interface IDomainSearchModel
    {
        int PageIndex { set; get; }
        int PageSize { set; get; }
        string SearchContent { set; get; }
        string OrderBy { set; get; }
        string FileName { get; set; }
    }


    public class BaseSearchModel
    {
        public int PageIndex { set; get; }
        public int PageSize { set; get; }
        public string SearchContent { set; get; }
        public string OrderBy { set; get; }
        public string FileName { set; get; }
    }

}
