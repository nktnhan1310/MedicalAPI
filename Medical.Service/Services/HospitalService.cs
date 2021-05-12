using AutoMapper;
using Medical.Entities;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class HospitalService : DomainService<Hospitals, SearchHospital>, IHospitalService
    {
        public HospitalService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Hospital_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchHospital baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@Email", string.IsNullOrEmpty(baseSearch.Email) ? DBNull.Value : (object)baseSearch.Email),
                new SqlParameter("@Phone", baseSearch.Phone),
                new SqlParameter("@TotalVisitNo", baseSearch.TotalVisitNo),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Cập nhật thông tin bệnh viện
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Hospitals item)
        {
            bool result = false;
            if (item != null)
            {
                await unitOfWork.Repository<Hospitals>().CreateAsync(item);
                await unitOfWork.SaveAsync();
                // Cập nhật thông tin dịch vụ bệnh viện
                if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
                {
                    foreach (var serviceTypeMappingHospital in item.ServiceTypeMappingHospitals)
                    {
                        serviceTypeMappingHospital.HospitalId = item.Id;
                        await unitOfWork.Repository<ServiceTypeMappingHospital>().CreateAsync(serviceTypeMappingHospital);
                    }
                }
                // Cập nhật thông tin Kênh bệnh viện
                if (item.ChannelMappingHospitals != null && item.ChannelMappingHospitals.Any())
                {
                    foreach (var channelMappingHospital in item.ChannelMappingHospitals)
                    {
                        channelMappingHospital.HospitalId = item.Id;
                        await unitOfWork.Repository<ChannelMappingHospital>().CreateAsync(channelMappingHospital);
                    }
                }
                // Cập nhật thông tin file
                if (item.HospitalFiles != null && item.HospitalFiles.Any())
                {
                    foreach (var hospitalFile in item.HospitalFiles)
                    {
                        hospitalFile.HospitalId = item.Id;
                        await unitOfWork.Repository<HospitalFiles>().CreateAsync(hospitalFile);
                    }
                }
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin bệnh viện
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Hospitals item)
        {
            bool result = false;
            var exists = await Queryable
                 .AsNoTracking()
                 .Where(e => e.Id == item.Id && !e.Deleted)
                 .FirstOrDefaultAsync();

            if (exists != null)
            {
                exists = mapper.Map<Hospitals>(item);
                unitOfWork.Repository<Hospitals>().Update(exists);

                // Cập nhật thông tin dịch vụ
                if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
                {
                    foreach (var serviceTypeMappingHospital in item.ServiceTypeMappingHospitals)
                    {
                        var existServiceTypeMapping = await unitOfWork.Repository<ServiceTypeMappingHospital>().GetQueryable()
                                                             .AsNoTracking()
                                                             .Where(e => e.Id == serviceTypeMappingHospital.Id && !e.Deleted)
                                                             .FirstOrDefaultAsync();
                        if(existServiceTypeMapping != null)
                        {
                            existServiceTypeMapping = mapper.Map<ServiceTypeMappingHospital>(serviceTypeMappingHospital);
                            unitOfWork.Repository<ServiceTypeMappingHospital>().Update(existServiceTypeMapping);
                        }
                        else
                        {
                            await unitOfWork.Repository<ServiceTypeMappingHospital>().CreateAsync(serviceTypeMappingHospital);
                        }
                    }
                }

                // Cập nhật thông tin kênh thanh toán
                if (item.ChannelMappingHospitals != null && item.ChannelMappingHospitals.Any())
                {
                    foreach (var channelMappingHospital in item.ChannelMappingHospitals)
                    {
                        var existChannelMappingHospital = await unitOfWork.Repository<ChannelMappingHospital>().GetQueryable()
                                                             .AsNoTracking()
                                                             .Where(e => e.Id == channelMappingHospital.Id && !e.Deleted)
                                                             .FirstOrDefaultAsync();
                        if (existChannelMappingHospital != null)
                        {
                            existChannelMappingHospital = mapper.Map<ChannelMappingHospital>(channelMappingHospital);
                            unitOfWork.Repository<ChannelMappingHospital>().Update(existChannelMappingHospital);
                        }
                        else
                        {
                            await unitOfWork.Repository<ChannelMappingHospital>().CreateAsync(channelMappingHospital);
                        }
                    }
                }
                // Cập nhật thông tin file của bệnh viện
                if (item.HospitalFiles != null && item.HospitalFiles.Any())
                {
                    foreach (var hospitalFile in item.HospitalFiles)
                    {
                        var existHospitalFile = await unitOfWork.Repository<HospitalFiles>().GetQueryable()
                                                             .AsNoTracking()
                                                             .Where(e => e.Id == hospitalFile.Id && !e.Deleted)
                                                             .FirstOrDefaultAsync();
                        if (existHospitalFile != null)
                        {
                            existHospitalFile = mapper.Map<HospitalFiles>(hospitalFile);
                            unitOfWork.Repository<HospitalFiles>().Update(existHospitalFile);
                        }
                        else
                        {
                            await unitOfWork.Repository<HospitalFiles>().CreateAsync(hospitalFile);
                        }
                    }
                }

                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }


    }
}
