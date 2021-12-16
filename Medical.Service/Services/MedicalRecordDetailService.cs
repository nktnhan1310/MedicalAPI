﻿using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
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
        public MedicalRecordDetailService(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
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
                new SqlParameter("@IsReExamination", baseSearch.IsReExamination),

                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                //new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return sqlParameters;
        }

        /// <summary>
        /// Cập nhật thông tin file chi tiết hồ sơ bệnh án
        /// </summary>
        /// <param name="medicalRecordDetailId"></param>
        /// <param name="medicalRecordDetailFiles"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMedicalRecordDetailFileAsync(int medicalRecordDetailId, IList<UserFiles> medicalRecordDetailFiles)
        {
<<<<<<< HEAD
            int? userId = null;
            var medicalRecordId = await this.unitOfWork.Repository<MedicalRecordDetails>().GetQueryable().Where(e => e.Id == medicalRecordDetailId).Select(e => e.MedicalRecordId).FirstOrDefaultAsync();
            if (medicalRecordId.HasValue && medicalRecordId.Value > 0)
=======
            bool result = false;
            int? userId = null;
            var medicalRecordId = await this.unitOfWork.Repository<MedicalRecordDetails>().GetQueryable().Where(e => e.Id == medicalRecordDetailId).Select(e => e.MedicalRecordId).FirstOrDefaultAsync();
            if(medicalRecordId.HasValue && medicalRecordId.Value > 0)
            {
                userId = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == medicalRecordId.Value).Select(e => e.UserId).FirstOrDefaultAsync();
            }


            if (medicalRecordDetailFiles != null && medicalRecordDetailFiles.Any())
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
            {
                userId = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == medicalRecordId.Value).Select(e => e.UserId).FirstOrDefaultAsync();
            }
            if (medicalRecordDetailFiles == null || !medicalRecordDetailFiles.Any()) throw new AppException("Không tồn tại file để update");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var medicalRecordDetailFile in medicalRecordDetailFiles)
                    {
                        var existMedicalRecordDetailFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable()
                            .Where(e => !e.Deleted
                            && e.MedicalRecordDetailId == medicalRecordDetailId
                            && e.Id == medicalRecordDetailFile.Id
                            ).FirstOrDefaultAsync();
                        if (existMedicalRecordDetailFile != null)
                        {
                            medicalRecordDetailFile.MedicalRecordDetailId = medicalRecordDetailId;
                            existMedicalRecordDetailFile = mapper.Map<UserFiles>(medicalRecordDetailFile);
                            existMedicalRecordDetailFile.UserId = userId;
                            this.unitOfWork.Repository<UserFiles>().Update(existMedicalRecordDetailFile);
                        }
                        else
                        {
                            medicalRecordDetailFile.MedicalRecordDetailId = medicalRecordDetailId;
                            medicalRecordDetailFile.UserId = userId;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(medicalRecordDetailFile);
                        }
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
        }

        /// <summary>
        /// Thêm mới tiểu sử bệnh án
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(MedicalRecordDetails item)
        {
            if (item == null) throw new AppException("Vui lòng chọn thông tin item");
            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
<<<<<<< HEAD
                    await this.unitOfWork.Repository<MedicalRecordDetails>().CreateAsync(item);
                    await this.unitOfWork.SaveAsync();

                    // Lấy ID USER
                    int? userId = null;
                    var medicalRecordInfo = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == item.MedicalRecordId).FirstOrDefaultAsync();
                    if (medicalRecordInfo != null) userId = medicalRecordInfo.UserId;

                    // Thêm file cho tiểu sử bệnh án
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            file.MedicalRecordDetailId = item.Id;
                            file.UserId = userId;
                            await this.unitOfWork.Repository<UserFiles>().CreateAsync(file);
                        }
=======
                    var existMedicalRecordDetailFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable()
                        .Where(e => !e.Deleted
                        && e.MedicalRecordDetailId == medicalRecordDetailId
                        && e.Id == medicalRecordDetailFile.Id
                        ).FirstOrDefaultAsync();
                    if (existMedicalRecordDetailFile != null)
                    {
                        medicalRecordDetailFile.MedicalRecordDetailId = medicalRecordDetailId;
                        existMedicalRecordDetailFile = mapper.Map<UserFiles>(medicalRecordDetailFile);
                        existMedicalRecordDetailFile.UserId = userId;
                        this.unitOfWork.Repository<UserFiles>().Update(existMedicalRecordDetailFile);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                    }

                    // Cập nhật thông tin đơn thuốc
                    if (item.MedicalBills != null && item.MedicalBills.Any())
                    {
<<<<<<< HEAD
                        foreach (var medicalBill in item.MedicalBills)
                        {
                            medicalBill.Created = DateTime.Now;
                            medicalBill.CreatedBy = item.CreatedBy;
                            await this.unitOfWork.Repository<MedicalBills>().CreateAsync(medicalBill);
                        }
=======
                        medicalRecordDetailFile.MedicalRecordDetailId = medicalRecordDetailId;
                        medicalRecordDetailFile.UserId = userId;
                        await this.unitOfWork.Repository<UserFiles>().CreateAsync(medicalRecordDetailFile);
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
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
        }

        /// <summary>
<<<<<<< HEAD
=======
        /// Thêm mới tiểu sử bệnh án
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(MedicalRecordDetails item)
        {
            bool success = false;
            if (item != null)
            {
                await this.unitOfWork.Repository<MedicalRecordDetails>().CreateAsync(item);
                await this.unitOfWork.SaveAsync();

                // Lấy ID USER
                int? userId = null;
                var medicalRecordInfo = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == item.MedicalRecordId).FirstOrDefaultAsync();
                if(medicalRecordInfo != null) userId = medicalRecordInfo.UserId;

                // Thêm file cho tiểu sử bệnh án
                if (item.UserFiles != null && item.UserFiles.Any())
                {
                    foreach (var file in item.UserFiles)
                    {
                        file.MedicalRecordDetailId = item.Id;
                        file.UserId = userId;
                        await this.unitOfWork.Repository<UserFiles>().CreateAsync(file);
                    }
                }

                // Cập nhật thông tin đơn thuốc
                if (item.MedicalBills != null && item.MedicalBills.Any())
                {
                    foreach (var medicalBill in item.MedicalBills)
                    {
                        medicalBill.Created = DateTime.Now;
                        medicalBill.CreatedBy = item.CreatedBy;
                        await this.unitOfWork.Repository<MedicalBills>().CreateAsync(medicalBill);
                    }
                }
                await this.unitOfWork.SaveAsync();
                success = true;
            }

            return success;
        }

        /// <summary>
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        /// Cập nhật tiểu sử bệnh án
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(MedicalRecordDetails item)
        {
<<<<<<< HEAD
            var existItem = await this.unitOfWork.Repository<MedicalRecordDetails>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem == null) throw new AppException("Thông tin item không tồn tại");

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
=======
            bool success = false;
            if (item != null)
            {
                var existItem = await this.unitOfWork.Repository<MedicalRecordDetails>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
                if (existItem != null)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                {
                    existItem = mapper.Map<MedicalRecordDetails>(item);
                    this.unitOfWork.Repository<MedicalRecordDetails>().Update(item);
                    await this.unitOfWork.SaveAsync();

                    // Lấy ID USER
                    int? userId = null;
                    var medicalRecordInfo = await this.unitOfWork.Repository<MedicalRecords>().GetQueryable().Where(e => e.Id == item.MedicalRecordId).FirstOrDefaultAsync();
                    if (medicalRecordInfo != null) userId = medicalRecordInfo.UserId;

                    // Cập nhật thông tin file của tiểu sử bệnh án
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            var existFile = await this.unitOfWork.Repository<UserFiles>().GetQueryable().Where(e => e.Id == file.Id).FirstOrDefaultAsync();
                            if (existFile != null)
                            {
                                existFile = mapper.Map<UserFiles>(file);
                                existFile.MedicalRecordDetailId = existItem.Id;
                                existFile.UserId = userId;
                                this.unitOfWork.Repository<UserFiles>().Update(existFile);
                            }
                            else
                            {
                                file.MedicalRecordDetailId = existItem.Id;
                                file.UserId = userId;
                                await this.unitOfWork.Repository<UserFiles>().CreateAsync(file);
                            }
                        }
                        await this.unitOfWork.SaveAsync();
                    }

                    // Cập nhật thông tin đơn thuốc
<<<<<<< HEAD
                    if (item.MedicalBills != null && item.MedicalBills.Any())
=======
                    if(item.MedicalBills != null && item.MedicalBills.Any())
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                    {
                        foreach (var medicalBill in item.MedicalBills)
                        {
                            var existMedicalBill = await this.unitOfWork.Repository<MedicalBills>().GetQueryable().Where(e => e.Id == medicalBill.Id).FirstOrDefaultAsync();
<<<<<<< HEAD
                            if (existMedicalBill != null)
=======
                            if(existMedicalBill != null)
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
                            {
                                existMedicalBill = mapper.Map<MedicalBills>(medicalBill);
                                existMedicalBill.MedicalRecordDetailId = existItem.Id;
                                this.unitOfWork.Repository<MedicalBills>().Update(existMedicalBill);
                            }
                            else
                            {
                                medicalBill.MedicalRecordDetailId = existItem.Id;
                                await this.unitOfWork.Repository<MedicalBills>().CreateAsync(medicalBill);
                            }
                        }
                    }
<<<<<<< HEAD

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
                    success = true;
                }
            }

            return success;
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
        }

    }
}
