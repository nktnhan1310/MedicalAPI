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
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class RoomExaminationService : CatalogueHospitalService<RoomExaminations, SearchRoomExamination>, IRoomExaminationService
    {
        public RoomExaminationService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
            this.configuration = configuration;
        }

        protected override string GetStoreProcName()
        {
            return "RoomExamination_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchRoomExamination baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@SpecialistTypeId", baseSearch.SpecialistTypeId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        public override async Task<string> GetExistItemMessage(RoomExaminations item)
        {
            string result = string.Empty;
            bool isExistCode = await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id
            && x.HospitalId == item.Id
            && x.Code == item.Code
            );
            if (isExistCode)
                return "Phòng đã tồn tại";

            return result;
        }

        /// <summary>
        /// Lấy thông tin trực theo phòng
        /// </summary>
        /// <param name="searchHopitalExtension"></param>
        /// <returns></returns>
        public async Task<IList<ExaminationScheduleDetails>> GetRoomDetail(SearchHopitalExtension searchHopitalExtension)
        {
            IList<ExaminationScheduleDetails> examinationScheduleDetails = new List<ExaminationScheduleDetails>();
            DateTime baseDate = DateTime.Now;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
            var thisWeekEnd = thisWeekStart.AddDays(6);
            if (!searchHopitalExtension.FromExaminationDate.HasValue)
                searchHopitalExtension.FromExaminationDate = thisWeekStart;
            if (!searchHopitalExtension.ToExaminationDate.HasValue)
                searchHopitalExtension.ToExaminationDate = thisWeekEnd;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@HospitalId", searchHopitalExtension.HospitalId),
                new SqlParameter("@RoomExaminationId", searchHopitalExtension.RoomExaminationId),
                new SqlParameter("@FromExaminationDate", searchHopitalExtension.FromExaminationDate),
                new SqlParameter("@ToExaminationDate", searchHopitalExtension.ToExaminationDate),
            };
            examinationScheduleDetails = await unitOfWork.Repository<ExaminationScheduleDetails>().ExcuteStoreAsync("RoomDetail_GetInfo", sqlParameters);
            return examinationScheduleDetails;
        }

        /// <summary>
        /// Lấy danh sách phân trang phòng của bệnh viện
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<RoomExaminations>> GetPagedListData(SearchRoomExamination baseSearch)
        {
            PagedList<RoomExaminations> pagedList = new PagedList<RoomExaminations>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await this.unitOfWork.Repository<RoomExaminations>().ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }

        /// <summary>
        /// Import danh sách phòng khám theo bệnh viện
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="createdBy"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public override async Task<AppDomainImportResult> ImportTemplateFile(Stream stream, string createdBy, int? hospitalId)
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
                var existItems = Queryable.Where(e => !e.Deleted
                && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
                );
                var existSpecialistTypes = this.unitOfWork.Repository<SpecialistTypes>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active
                    && (!hospitalId.HasValue || e.HospitalId == hospitalId.Value)
                    );


                var existHospitals = this.unitOfWork.Repository<Hospitals>().GetQueryable().Where(e => !e.Deleted && e.Active);
                var catalogueMappers = new ExcelMapper(stream) { HeaderRow = false, MinRowNumber = 1 }.Fetch<RoomExaminationMapper>().ToList();
                if (catalogueMappers != null && catalogueMappers.Any())
                {
                    List<RoomExaminations> roomExaminations = new List<RoomExaminations>();
                    foreach (var catalogueMapper in catalogueMappers)
                    {
                        string roomCode = string.Empty;
                        int index = catalogueMappers.IndexOf(catalogueMapper);
                        int? specialistTypeId = null;
                        Hospitals hospitalInfo = new Hospitals();
                        SpecialistTypes specialistTypeInfo = new SpecialistTypes();
                        int resultIndex = index + 2;
                        IList<string> errors = new List<string>();
                        if (string.IsNullOrEmpty(catalogueMapper.HospitalCode))
                            errors.Add("Vui lòng nhập mã bệnh viện");

                        // Kiểm tra user có theo bệnh viện hay không
                        // TH1: Theo bệnh viện => Lấy theo thông tin id bệnh viện
                        if (hospitalId.HasValue && hospitalId.Value > 0)
                        {
                            hospitalInfo = await existHospitals.Where(e => e.Id == hospitalId.Value).FirstOrDefaultAsync();
                        }
                        // TH2: System admin import => Lấy theo mã bệnh viện theo file excel
                        else
                        {
                            if (!existHospitals.Any(x => x.Code == catalogueMapper.HospitalCode))
                                errors.Add("Mã bệnh viện không tồn tại");
                            else
                                hospitalInfo = await existHospitals.Where(e => e.Code == catalogueMapper.HospitalCode).FirstOrDefaultAsync();
                        }
                        if (hospitalInfo == null)
                            errors.Add("Thông tin bệnh viện không tồn tại");
                        if (string.IsNullOrEmpty(catalogueMapper.SpecialistTypeCode))
                            errors.Add("Vui lòng nhập mã chuyên khoa");
                        else
                        {
                            specialistTypeInfo = await this.unitOfWork.Repository<SpecialistTypes>().GetQueryable()
                                .Where(e => !e.Deleted && e.Active && e.Code == catalogueMapper.SpecialistTypeCode).FirstOrDefaultAsync();
                            if (specialistTypeInfo == null) errors.Add(string.Format("Chuyên khoa với mã {0} không tồn tại!", catalogueMapper.SpecialistTypeCode));
                            else specialistTypeId = specialistTypeInfo.Id;
                        }
                        // Kiểm tra tên phòng khám
                        // 1. Không tồn tại => thông báo lỗi
                        if (string.IsNullOrEmpty(catalogueMapper.Name))
                            errors.Add("Vui lòng nhập tên phòng khám");
                        // 2. Tồn tại => generate code cho phòng khám
                        else
                        {
                            roomCode = this.GenerateRoomCode(catalogueMapper.RoomIndex, catalogueMapper.Name, specialistTypeInfo == null ? string.Empty : specialistTypeInfo.Code);
                        }

                        if (hospitalInfo != null && (existItems.Any(x => x.HospitalId == hospitalInfo.Id && x.Code == roomCode)
                            || roomExaminations.Any(x => x.HospitalId == hospitalInfo.Id && x.Code == roomCode)))
                            errors.Add("Mã phòng đã tồn tại");


                        if (errors.Any())
                        {
                            ws.Cells["G" + resultIndex].Value = string.Join(", ", errors);
                            totalFailed += 1;
                        }
                        else
                        {
                            ws.Cells["G" + resultIndex].Value = "Thành công";
                            totalSuccess += 1;

                            RoomExaminations roomExamination = new RoomExaminations()
                            {
                                Deleted = false,
                                Active = true,
                                Created = DateTime.Now,
                                CreatedBy = createdBy,
                                Code = roomCode,
                                HospitalId = hospitalInfo.Id,
                                SpecialistTypeId = specialistTypeId,
                                Description = catalogueMapper.Description,
                                RoomIndex = catalogueMapper.RoomIndex,
                                ExaminationAreaDescription = catalogueMapper.ExaminationAreaDescription,

                            };
                            roomExaminations.Add(roomExamination);
                            dataTable = AddDataTableRow(dataTable, roomExamination);
                        }
                    }
                }
                ws.Column(7).Hidden = false;
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

        protected override DataTable AddDataTableRow(DataTable dt, RoomExaminations item)
        {
            dt.Rows.Add(item.Id, item.HospitalId, item.Created, item.CreatedBy, item.Updated, item.UpdatedBy, item.Deleted, item.Active, item.Code, item.Name, item.Description, item.ExaminationAreaDescription, item.RoomIndex, item.SpecialistTypeId);
            return dt;
        }

        protected override string GetTableName()
        {
            return "RoomExaminations";
        }

        /// <summary>
        /// Tạo mã phòng
        /// </summary>
        /// <param name="roomIndex"></param>
        /// <param name="roomName"></param>
        /// <param name="specialistTypeCode"></param>
        /// <returns></returns>
        public string GenerateRoomCode(string roomIndex, string roomName, string specialistTypeCode)
        {
            string result = string.Empty;
            string roomCompactName = StringCipher.GetFirstCharOfWord(roomName);
            if (!string.IsNullOrEmpty(specialistTypeCode))
                result = string.Format("{0}-{1}{2}", specialistTypeCode, roomCompactName, !string.IsNullOrEmpty(roomIndex) ? roomIndex : string.Empty);
            else if (!string.IsNullOrEmpty(roomCompactName))
                result = string.Format("{0}{1}", roomCompactName, !string.IsNullOrEmpty(roomIndex) ? roomIndex : string.Empty);
            return result;
        }


    }
}
