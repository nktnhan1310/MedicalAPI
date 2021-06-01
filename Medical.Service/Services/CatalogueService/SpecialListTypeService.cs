using AutoMapper;
using Ganss.Excel;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Medical.Service.Services
{
    public class SpecialListTypeService : CatalogueHospitalService<SpecialistTypes, SearchSpecialListType>, ISpecialListTypeService
    {
        public SpecialListTypeService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "SpecialListType_GetPagingData";
        }

        protected override string GetTableName()
        {
            return "SpecialistTypes";
        }

        protected override SqlParameter[] GetSqlParameters(SearchSpecialListType baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@ExaminationDate", baseSearch.ExaminationDate),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
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

        /// <summary>
        /// Import file chuyên khoa
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public override async Task<AppDomainImportResult> ImportTemplateFile(Stream stream, string createdBy)
        {
            AppDomainImportResult appDomainImportResult = new AppDomainImportResult();
            var dataTable = SetDataTable();
            using (var package = new ExcelPackage(stream))
            {
                int totalFailed = 0;
                int totalSuccess = 0;
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                {
                    throw new Exception("Sheet name không tồn tại");
                }
                var existItems = Queryable.Where(e => !e.Deleted);
                var existHospitals = this.unitOfWork.Repository<Hospitals>().GetQueryable().Where(e => !e.Deleted && e.Active);
                var catalogueMappers = new ExcelMapper(stream) { HeaderRow = false, MinRowNumber = 1 }.Fetch<SpecialListTypeMapper>().ToList();
                if (catalogueMappers != null && catalogueMappers.Any())
                {
                    List<SpecialistTypes> specialistTypeImports = new List<SpecialistTypes>();
                    foreach (var catalogueMapper in catalogueMappers)
                    {
                        int index = catalogueMappers.IndexOf(catalogueMapper);
                        double? price = null;
                        Hospitals hospitalInfo = new Hospitals();
                        int resultIndex = index + 2;
                        IList<string> errors = new List<string>();
                        if (string.IsNullOrEmpty(catalogueMapper.HospitalCode))
                            errors.Add("Vui lòng nhập mã bệnh viện");
                        if(!existHospitals.Any(x => x.Code == catalogueMapper.HospitalCode))
                            errors.Add("Mã bệnh viện không tồn tại");
                        else
                            hospitalInfo = await existHospitals.Where(e => e.Code == catalogueMapper.HospitalCode).FirstOrDefaultAsync();
                        if (hospitalInfo == null)
                            errors.Add("Thông tin bệnh viện không tồn tại");
                        if (string.IsNullOrEmpty(catalogueMapper.Code))
                            errors.Add("Vui lòng nhập mã chuyên khoa");
                        if (hospitalInfo != null && (existItems.Any(x => x.HospitalId == hospitalInfo.Id && x.Code == catalogueMapper.Code) 
                            || specialistTypeImports.Any(x => x.HospitalId == hospitalInfo.Id &&  x.Code == catalogueMapper.Code)))
                            errors.Add("Mã chuyên khoa đã tồn tại");
                        if (string.IsNullOrEmpty(catalogueMapper.Name))
                            errors.Add("Vui lòng nhập tên chuyên khoa");
                        price = TryParseUtilities.TryParseDouble(catalogueMapper.Price);
                        if(price == null)
                            errors.Add("Giá khám không hợp lệ");

                        if (errors.Any())
                        {
                            ws.Cells["F" + resultIndex].Value = string.Join(", ", errors);
                            totalFailed += 1;
                        }
                        else
                        {
                            ws.Cells["F" + resultIndex].Value = "Thành công";
                            totalSuccess += 1;
                            SpecialistTypes item = new SpecialistTypes()
                            {
                                Code = catalogueMapper.Code,
                                Name = catalogueMapper.Name,
                                Description = catalogueMapper.Description,
                                Deleted = false,
                                Active = true,
                                Created = DateTime.Now,
                                HospitalId = hospitalInfo.Id,
                                CreatedBy = createdBy,
                                Price = price
                            };
                            specialistTypeImports.Add(item);
                            dataTable = AddDataTableRow(dataTable, item);
                        }
                    }
                }
                ws.Column(6).Hidden = false;
                ws.Cells.AutoFitColumns();
                appDomainImportResult.Data = new
                {
                    TotalSuccess = totalSuccess,
                    TotalFailed = totalFailed,
                };
                appDomainImportResult.ResultFile = package.GetAsByteArray();
            }
            if (dataTable.Rows.Count > 0)
            {
                var connectionString = string.Format(configuration.GetConnectionString("MedicalDbContext"));
                using (SqlBulkCopy bc = new SqlBulkCopy(connectionString))
                {
                    bc.DestinationTableName = this.GetTableName();
                    bc.BulkCopyTimeout = 4000;
                    await bc.WriteToServerAsync(dataTable);
                }
            }
            appDomainImportResult.Success = true;
            return appDomainImportResult;
        }

        protected override DataTable AddDataTableRow(DataTable dt, SpecialistTypes item)
        {
            dt.Rows.Add(item.Id, item.HospitalId, item.Price, item.Created, item.CreatedBy, item.Updated, item.UpdatedBy, item.Deleted, item.Active, item.Code, item.Name, item.Description);
            return dt;
        }

        /// <summary>
        /// Check trùng item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(SpecialistTypes item)
        {
            string result = string.Empty;
            bool isExistCode = await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.HospitalId == item.HospitalId && x.Code == item.Code);
            if (isExistCode)
                return "Mã đã tồn tại!";
            return result;
        }
    }
}
