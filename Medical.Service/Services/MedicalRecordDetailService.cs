using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class MedicalRecordDetailService : CoreHospitalService<MedicalRecordDetails, SearchMedicalRecordDetail>, IMedicalRecordDetailService
    {
        public MedicalRecordDetailService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "MedicalRecordDetail_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchMedicalRecordDetail baseSearch)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@MedicalRecordId", baseSearch.MedicalRecordId),
                new SqlParameter("@ExaminationFormId", baseSearch.ExaminationFormId),
                new SqlParameter("@SpecialistTypeId", baseSearch.SpecialistTypeId),
                new SqlParameter("@ServiceTypeId", baseSearch.ServiceTypeId),
                new SqlParameter("@MedicalRecordDetailId", baseSearch.MedicalRecordDetailId),

                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return sqlParameters;
        }

        /// <summary>
        /// Cập nhật thông tin file chi tiết hồ sơ bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailId"></param>
        /// <param name="medicalRecordDetailFiles"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMedicalRecordDetailFileAsync(int medicalRecordDetailId, IList<MedicalRecordDetailFiles> medicalRecordDetailFiles)
        {
            bool result = false;
            if (medicalRecordDetailFiles != null && medicalRecordDetailFiles.Any())
            {
                foreach (var medicalRecordDetailFile in medicalRecordDetailFiles)
                {
                    var existMedicalRecordDetailFile = await this.unitOfWork.Repository<MedicalRecordDetailFiles>().GetQueryable()
                        .Where(e => !e.Deleted
                        && e.MedicalRecordDetailId == medicalRecordDetailId
                        && e.Id == medicalRecordDetailFile.Id
                        ).FirstOrDefaultAsync();
                    if (existMedicalRecordDetailFile != null)
                    {
                        medicalRecordDetailFile.MedicalRecordDetailId = medicalRecordDetailId;
                        existMedicalRecordDetailFile = mapper.Map<MedicalRecordDetailFiles>(medicalRecordDetailFile);
                        this.unitOfWork.Repository<MedicalRecordDetailFiles>().Update(existMedicalRecordDetailFile);
                    }
                    else
                    {
                        medicalRecordDetailFile.MedicalRecordDetailId = medicalRecordDetailId;
                        await this.unitOfWork.Repository<MedicalRecordDetailFiles>().CreateAsync(medicalRecordDetailFile);
                    }
                }
                await this.unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }
    }
}
