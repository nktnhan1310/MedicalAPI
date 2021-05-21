using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class HospitalService : DomainService<Hospitals, SearchHospital>, IHospitalService
    {
        private readonly IConfiguration configuration;
        public HospitalService(IMedicalUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper)
        {
            this.configuration = configuration;
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
                        serviceTypeMappingHospital.Created = DateTime.Now;
                        serviceTypeMappingHospital.CreatedBy = item.CreatedBy;
                        await unitOfWork.Repository<ServiceTypeMappingHospital>().CreateAsync(serviceTypeMappingHospital);
                    }
                }
                // Cập nhật thông tin Kênh bệnh viện
                if (item.ChannelMappingHospitals != null && item.ChannelMappingHospitals.Any())
                {
                    foreach (var channelMappingHospital in item.ChannelMappingHospitals)
                    {
                        channelMappingHospital.HospitalId = item.Id;
                        channelMappingHospital.Created = DateTime.Now;
                        channelMappingHospital.CreatedBy = item.CreatedBy;
                        await unitOfWork.Repository<ChannelMappingHospital>().CreateAsync(channelMappingHospital);
                    }
                }
                // Cập nhật thông tin file
                if (item.HospitalFiles != null && item.HospitalFiles.Any())
                {
                    foreach (var hospitalFile in item.HospitalFiles)
                    {
                        hospitalFile.HospitalId = item.Id;
                        hospitalFile.Created = DateTime.Now;
                        hospitalFile.CreatedBy = item.CreatedBy;
                        await unitOfWork.Repository<HospitalFiles>().CreateAsync(hospitalFile);
                    }
                }
                // Cập nhật thông ngân hàng liên kết
                if (item.BankInfos != null && item.BankInfos.Any())
                {
                    foreach (var bankInfo in item.BankInfos)
                    {
                        bankInfo.HospitalId = item.Id;
                        bankInfo.Created = DateTime.Now;
                        bankInfo.CreatedBy = item.CreatedBy;
                        await unitOfWork.Repository<BankInfos>().CreateAsync(bankInfo);
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
                        if (existServiceTypeMapping != null)
                        {
                            serviceTypeMappingHospital.Updated = DateTime.Now;
                            serviceTypeMappingHospital.UpdatedBy = item.UpdatedBy;
                            existServiceTypeMapping = mapper.Map<ServiceTypeMappingHospital>(serviceTypeMappingHospital);
                            unitOfWork.Repository<ServiceTypeMappingHospital>().Update(existServiceTypeMapping);
                        }
                        else
                        {
                            serviceTypeMappingHospital.HospitalId = item.Id;
                            serviceTypeMappingHospital.Created = DateTime.Now;
                            serviceTypeMappingHospital.CreatedBy = item.UpdatedBy;
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
                            channelMappingHospital.Updated = DateTime.Now;
                            channelMappingHospital.UpdatedBy = item.UpdatedBy;
                            existChannelMappingHospital = mapper.Map<ChannelMappingHospital>(channelMappingHospital);
                            unitOfWork.Repository<ChannelMappingHospital>().Update(existChannelMappingHospital);
                        }
                        else
                        {
                            channelMappingHospital.HospitalId = item.Id;
                            channelMappingHospital.Created = DateTime.Now;
                            channelMappingHospital.CreatedBy = item.UpdatedBy;
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
                            hospitalFile.Updated = DateTime.Now;
                            hospitalFile.UpdatedBy = item.UpdatedBy;
                            existHospitalFile = mapper.Map<HospitalFiles>(hospitalFile);
                            unitOfWork.Repository<HospitalFiles>().Update(existHospitalFile);
                        }
                        else
                        {
                            hospitalFile.HospitalId = item.Id;
                            hospitalFile.Created = DateTime.Now;
                            hospitalFile.CreatedBy = item.UpdatedBy;
                            await unitOfWork.Repository<HospitalFiles>().CreateAsync(hospitalFile);
                        }
                    }
                }

                // Cập nhật thông tin ngân hàng liên kết
                if (item.BankInfos != null && item.BankInfos.Any())
                {
                    foreach (var bankInfo in item.BankInfos)
                    {
                        var existBankInfo = await unitOfWork.Repository<BankInfos>().GetQueryable()
                                                             .AsNoTracking()
                                                             .Where(e => e.Id == bankInfo.Id && !e.Deleted)
                                                             .FirstOrDefaultAsync();
                        if (existBankInfo != null)
                        {
                            bankInfo.Updated = DateTime.Now;
                            bankInfo.UpdatedBy = item.UpdatedBy;
                            existBankInfo = mapper.Map<BankInfos>(bankInfo);
                            unitOfWork.Repository<BankInfos>().Update(existBankInfo);
                        }
                        else
                        {
                            bankInfo.HospitalId = item.Id;
                            bankInfo.Created = DateTime.Now;
                            bankInfo.CreatedBy = item.UpdatedBy;
                            await unitOfWork.Repository<BankInfos>().CreateAsync(bankInfo);
                        }
                    }
                }
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Check trùng mã bệnh viện
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(Hospitals item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;

            bool isExistItem = await this.Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.Code == item.Code);
            if (isExistItem)
                messages.Add("Mã bệnh viện đã tồn tại");
            // Kiểm tra trùng kênh đăng ký
            if (item.ChannelMappingHospitals != null && item.ChannelMappingHospitals.Any())
            {
                List<int> channelIds = new List<int>();
                foreach (var channelMappingHospital in item.ChannelMappingHospitals)
                {
                    if (channelIds.Any(x => x == channelMappingHospital.ChannelId))
                    {
                        var channelInfo = await this.unitOfWork.Repository<Channels>().GetQueryable().Where(e => e.Id == channelMappingHospital.ChannelId).FirstOrDefaultAsync();
                        if (channelInfo != null)
                            messages.Add(string.Format("Kênh đăng ký {0} đã tồn tại!", channelInfo.Name));
                        else messages.Add("Kênh đăng ký không tồn tại!");
                    }
                    channelIds.Add(channelMappingHospital.ChannelId);
                }
            }
            // Kiểm tra trùng dịch vụ
            if (item.ServiceTypeMappingHospitals != null && item.ServiceTypeMappingHospitals.Any())
            {
                List<int> serviceTypeIds = new List<int>();
                foreach (var serviceTypeMappingHospital in item.ServiceTypeMappingHospitals)
                {
                    if (serviceTypeIds.Any(x => x == serviceTypeMappingHospital.ServiceTypeId))
                    {
                        var serviceTypeInfo = await this.unitOfWork.Repository<ServiceTypes>().GetQueryable().Where(e => e.Id == serviceTypeMappingHospital.ServiceTypeId).FirstOrDefaultAsync();
                        if (serviceTypeInfo != null)
                            messages.Add(string.Format("Dịch vụ {0} đã tồn tại!", serviceTypeInfo.Name));
                        else messages.Add("Dịch vụ không tồn tại!");
                    }
                    serviceTypeIds.Add(serviceTypeMappingHospital.ServiceTypeId);
                }
            }


            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

    }
}
