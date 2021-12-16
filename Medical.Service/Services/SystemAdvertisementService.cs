﻿using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
<<<<<<< HEAD
using Medical.Interface.DbContext;
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class SystemAdvertisementService : CoreHospitalService<SystemAdvertisements, BaseHospitalSearch>, ISystemAdvertisementService
    {
<<<<<<< HEAD
        public SystemAdvertisementService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
=======
        public SystemAdvertisementService(IMedicalUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        {
        }

        protected override string GetStoreProcName()
        {
            return "SystemAdvertisement_GetPagingData";
        }

        /// <summary>
        /// Thêm mới dữ liệu quảng cáo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(SystemAdvertisements item)
        {
<<<<<<< HEAD
            if (item == null) throw new AppException("item không tồn tại");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this.unitOfWork.Repository<SystemAdvertisements>().Create(item);
                    await this.unitOfWork.SaveAsync();
                    // Thêm mới thông tin file quảng cáo nếu có
                    if (item.SystemFiles != null && item.SystemFiles.Any())
                    {
                        foreach (var file in item.SystemFiles)
                        {
                            file.Created = DateTime.Now;
                            file.CreatedBy = item.CreatedBy;
                            file.SystemAdvertisementId = item.Id;
                            file.Active = true;
                            file.Deleted = false;
                            this.unitOfWork.Repository<SystemFiles>().Create(file);
                        }
                        await this.unitOfWork.SaveAsync();
                    }
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }
=======
            bool result = false;
            if (item == null) throw new AppException("item không tồn tại");
            this.unitOfWork.Repository<SystemAdvertisements>().Create(item);
            await this.unitOfWork.SaveAsync();
            // Thêm mới thông tin file quảng cáo nếu có
            if (item.SystemFiles != null && item.SystemFiles.Any())
            {
                foreach (var file in item.SystemFiles)
                {
                    file.Created = DateTime.Now;
                    file.CreatedBy = item.CreatedBy;
                    file.SystemAdvertisementId = item.Id;
                    file.Active = true;
                    file.Deleted = false;
                    this.unitOfWork.Repository<SystemFiles>().Create(file);
                }
                await this.unitOfWork.SaveAsync();
            }
            result = true;
            return result;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }

        /// <summary>
        /// Cập nhật dữ liệu báo cáo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(SystemAdvertisements item)
        {
<<<<<<< HEAD
=======
            bool result = false;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            if (item == null) throw new AppException("item không tồn tại");
            var existSystemAdvertisement = await this.unitOfWork.Repository<SystemAdvertisements>().GetQueryable()
                .Where(e => !e.Deleted && e.Id == item.Id).FirstOrDefaultAsync();
            if (existSystemAdvertisement == null) throw new AppException("item không tồn tại");
<<<<<<< HEAD
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    existSystemAdvertisement = mapper.Map<SystemAdvertisements>(item);
                    this.unitOfWork.Repository<SystemAdvertisements>().Update(existSystemAdvertisement);
                    // Thêm mới thông tin file quảng cáo nếu có
                    if (item.SystemFiles != null && item.SystemFiles.Any())
                    {
                        foreach (var file in item.SystemFiles)
                        {
                            var existFileInfo = await this.unitOfWork.Repository<SystemFiles>().GetQueryable()
                                .Where(e => !e.Deleted && e.Id == file.Id).FirstOrDefaultAsync();
                            if (existFileInfo != null)
                            {
                                existFileInfo = mapper.Map<SystemFiles>(file);
                                existFileInfo.Updated = DateTime.Now;
                                existFileInfo.UpdatedBy = item.CreatedBy;
                                existFileInfo.SystemAdvertisementId = item.Id;
                                existFileInfo.Active = true;
                                existFileInfo.Deleted = false;
                                this.unitOfWork.Repository<SystemFiles>().Update(existFileInfo);
                            }
                            else
                            {
                                file.Created = DateTime.Now;
                                file.CreatedBy = item.CreatedBy;
                                file.SystemAdvertisementId = item.Id;
                                file.Active = true;
                                file.Deleted = false;
                                this.unitOfWork.Repository<SystemFiles>().Create(existFileInfo);
                            }

                        }
                        await this.unitOfWork.SaveAsync();
                    }
                    await this.unitOfWork.SaveAsync();
                    var contextTransaction = await contextTransactionTask;
                    await contextTransaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    var contextTransaction = await contextTransactionTask;
                    contextTransaction.Rollback();
                    return false;
                }
            }
=======
            existSystemAdvertisement = mapper.Map<SystemAdvertisements>(item);
            this.unitOfWork.Repository<SystemAdvertisements>().Update(existSystemAdvertisement);
            // Thêm mới thông tin file quảng cáo nếu có
            if (item.SystemFiles != null && item.SystemFiles.Any())
            {
                foreach (var file in item.SystemFiles)
                {
                    var existFileInfo = await this.unitOfWork.Repository<SystemFiles>().GetQueryable()
                        .Where(e => !e.Deleted && e.Id == file.Id).FirstOrDefaultAsync();
                    if (existFileInfo != null)
                    {
                        existFileInfo = mapper.Map<SystemFiles>(file);
                        existFileInfo.Updated = DateTime.Now;
                        existFileInfo.UpdatedBy = item.CreatedBy;
                        existFileInfo.SystemAdvertisementId = item.Id;
                        existFileInfo.Active = true;
                        existFileInfo.Deleted = false;
                        this.unitOfWork.Repository<SystemFiles>().Update(existFileInfo);
                    }
                    else
                    {
                        file.Created = DateTime.Now;
                        file.CreatedBy = item.CreatedBy;
                        file.SystemAdvertisementId = item.Id;
                        file.Active = true;
                        file.Deleted = false;
                        this.unitOfWork.Repository<SystemFiles>().Create(existFileInfo);
                    }

                }
                await this.unitOfWork.SaveAsync();
            }
            await this.unitOfWork.SaveAsync();
            result = true;

            return result;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }
    }
}