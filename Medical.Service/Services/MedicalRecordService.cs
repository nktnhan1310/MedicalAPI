using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
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
    public class MedicalRecordService : DomainService<MedicalRecords, SearchMedicalRecord>, IMedicalRecordService
    {
        public MedicalRecordService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "MedicalRecord_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchMedicalRecord baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@MedicalRecordId", baseSearch.MedicalRecordId),


                new SqlParameter("@UserId", baseSearch.UserId),
                //new SqlParameter("@HospitalId", baseSearch.HospitalId),
                new SqlParameter("@HospitalId", null),

                new SqlParameter("@Gender", baseSearch.Gender),
                new SqlParameter("@JobId", baseSearch.JobId),
                new SqlParameter("@CountryId", baseSearch.CountryId),
                new SqlParameter("@NationId", baseSearch.NationId),
                new SqlParameter("@CityId", baseSearch.CityId),
                new SqlParameter("@DistrictId", baseSearch.DistrictId),
                new SqlParameter("@WardId", baseSearch.WardId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Tạo hồ sơ
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(MedicalRecords item)
        {
            bool result = false;
            if (item != null)
            {
                // Lưu thông tin bác sĩ
                item.Id = 0;
                await unitOfWork.Repository<MedicalRecords>().CreateAsync(item);
                await unitOfWork.SaveAsync();
                // Cập nhật thông tin thêm của hồ sơ
                if (item.MedicalRecordAdditions != null && item.MedicalRecordAdditions.Any())
                {
                    foreach (var medicalRecordAddition in item.MedicalRecordAdditions)
                    {
                        medicalRecordAddition.MedicalRecordId = item.Id;
                        medicalRecordAddition.Created = DateTime.Now;
                        medicalRecordAddition.Active = true;
                        medicalRecordAddition.Id = 0;
                        await unitOfWork.Repository<MedicalRecordAdditions>().CreateAsync(medicalRecordAddition);
                    }
                }

                // Cập nhật thông tin file của hồ os7
                if (item.MedicalRecordFiles != null && item.MedicalRecordFiles.Any())
                {
                    foreach (var medicalRecordFile in item.MedicalRecordFiles)
                    {
                        medicalRecordFile.MedicalRecordId = item.Id;
                        medicalRecordFile.Created = DateTime.Now;
                        medicalRecordFile.Active = true;
                        medicalRecordFile.Id = 0;
                        await unitOfWork.Repository<MedicalRecordFiles>().CreateAsync(medicalRecordFile);
                    }
                }

                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        public override async Task<bool> UpdateAsync(MedicalRecords item)
        {
            bool result = false;
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();

            if (exists != null)
            {
                exists = mapper.Map<MedicalRecords>(item);
                unitOfWork.Repository<MedicalRecords>().Update(exists);

                // Cập nhật thông tin người thân của hồ sơ
                if (item.MedicalRecordAdditions != null && item.MedicalRecordAdditions.Any())
                {
                    foreach (var medicalRecordAddition in item.MedicalRecordAdditions)
                    {
                        var existMedicalRecordAddition = await unitOfWork.Repository<MedicalRecordAdditions>().GetQueryable()
                                                             .AsNoTracking()
                                                             .Where(e => e.Id == medicalRecordAddition.Id && !e.Deleted)
                                                             .FirstOrDefaultAsync();
                        if (existMedicalRecordAddition != null)
                        {
                            existMedicalRecordAddition = mapper.Map<MedicalRecordAdditions>(medicalRecordAddition);
                            existMedicalRecordAddition.MedicalRecordId = exists.Id;
                            existMedicalRecordAddition.Updated = DateTime.Now;
                            unitOfWork.Repository<MedicalRecordAdditions>().Update(existMedicalRecordAddition);
                        }
                        else
                        {
                            medicalRecordAddition.MedicalRecordId = exists.Id;
                            medicalRecordAddition.Created = DateTime.Now;
                            medicalRecordAddition.Id = 0;
                            await unitOfWork.Repository<MedicalRecordAdditions>().CreateAsync(medicalRecordAddition);
                        }
                    }
                }

                // Cập nhật thông tin file hồ sơ người dùng
                if (item.MedicalRecordFiles != null && item.MedicalRecordFiles.Any())
                {
                    foreach (var medicalRecordFile in item.MedicalRecordFiles)
                    {
                        var existMedicalRecordFile = await this.unitOfWork.Repository<MedicalRecordFiles>().GetQueryable()
                            .Where(e => e.Id == medicalRecordFile.Id
                            )
                            .FirstOrDefaultAsync();
                        if (existMedicalRecordFile != null)
                        {
                            existMedicalRecordFile = mapper.Map<MedicalRecordFiles>(medicalRecordFile);
                            existMedicalRecordFile.MedicalRecordId = item.Id;
                            existMedicalRecordFile.Updated = DateTime.Now;
                            this.unitOfWork.Repository<MedicalRecordFiles>().Update(existMedicalRecordFile);
                        }
                        else
                        {
                            medicalRecordFile.Created = DateTime.Now;
                            medicalRecordFile.MedicalRecordId = item.Id;
                            medicalRecordFile.Id = 0;
                            await this.unitOfWork.Repository<MedicalRecordFiles>().CreateAsync(medicalRecordFile);
                        }
                    }
                }
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        public override async Task<string> GetExistItemMessage(MedicalRecords item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistMedicalRecord = await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.UserId == item.UserId);
            if (isExistMedicalRecord)
                messages.Add("Hồ sơ của người dùng đã tồn tại!");
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

        public async Task<int> GetMedicalRecordIdByUser(int userId)
        {
            int medicalRecordId = 0;
            var medicalRecordInfo = await this.Queryable.Where(e => !e.Deleted && e.UserId == userId).FirstOrDefaultAsync();
            if (medicalRecordInfo != null)
                medicalRecordId = medicalRecordInfo.Id;
            return medicalRecordId;
        }
    }
}
