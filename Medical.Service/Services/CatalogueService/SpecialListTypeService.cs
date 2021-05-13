using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Medical.Service.Services
{
    public class SpecialListTypeService : CatalogueService<SpecialistTypes, SearchSpecialListType>, ISpecialListTypeService
    {
        public SpecialListTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "SpecialListType_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchSpecialListType baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
                //new SqlParameter("SearchContent", baseSearch.SearchContent),
            };
            return parameters;
        }

        public override async Task<PagedList<SpecialistTypes>> GetPagedListData(SearchSpecialListType baseSearch)
        {
            PagedList<SpecialistTypes> pagedList = new PagedList<SpecialistTypes>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await this.unitOfWork.Repository<SpecialistTypes>().ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }
    }
}
