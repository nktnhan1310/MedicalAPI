using AutoMapper;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.DbContext;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Medical.Service.Services.DomainService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Service
{
    public class AdditionServiceType : CatalogueHospitalService<AdditionServices, BaseHospitalSearch>, IAdditionServiceType
    {
        public AdditionServiceType(IMedicalUnitOfWork unitOfWork, IMedicalDbContext medicalDbContext, IMapper mapper) : base(unitOfWork, medicalDbContext, mapper)
        {
        }

        protected override string GetTableName()
        {
            return "AdditionServices";
        }

        /// <summary>
        /// Thêm mới dịch vụ phát sinh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(AdditionServices item)
        {

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this.unitOfWork.Repository<AdditionServices>().Create(item);
                    await this.unitOfWork.SaveAsync();

                    // THÊM MỚI CHI TIẾT DỊCH VỤ PHÁT SINH  
                    if (item.AdditionServiceDetails != null && item.AdditionServiceDetails.Any())
                    {
                        foreach (var additionServiceDetail in item.AdditionServiceDetails)
                        {
                            additionServiceDetail.AdditionServiceId = item.Id;
                            additionServiceDetail.HospitalId = item.HospitalId;
                            this.unitOfWork.Repository<AdditionServiceDetails>().Create(additionServiceDetail);
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
        /// Cập nhật thông tin dịch vụ phát sinh
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(AdditionServices item)
        {

            using (var contextTransactionTask = this.medicalDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existItem = await this.unitOfWork.Repository<AdditionServices>().GetQueryable()
                        .Where(e => e.Id == item.Id).FirstOrDefaultAsync();
                    if (existItem == null) throw new AppException("Không tìm thấy thông tin item");
                    existItem = mapper.Map<AdditionServices>(item);
                    existItem.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                    this.unitOfWork.Repository<AdditionServices>().Update(existItem);

                    // CẬP NHẬT THÔNG TIN CHI TIẾT DỊCH VỤ PHÁT SINH
                    if (item.AdditionServiceDetails != null && item.AdditionServiceDetails.Any())
                    {
                        foreach (var additionServiceDetail in item.AdditionServiceDetails)
                        {
                            var existAdditionServiceDetail = await this.unitOfWork.Repository<AdditionServiceDetails>().GetQueryable()
                                .Where(e => e.Id == additionServiceDetail.Id).FirstOrDefaultAsync();
                            if (existAdditionServiceDetail != null)
                            {
                                existAdditionServiceDetail = mapper.Map<AdditionServiceDetails>(additionServiceDetail);
                                existAdditionServiceDetail.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                                existAdditionServiceDetail.AdditionServiceId = existItem.Id;
                                existAdditionServiceDetail.HospitalId = existItem.HospitalId;
                                this.unitOfWork.Repository<AdditionServiceDetails>().Update(existAdditionServiceDetail);

                            }
                            else
                            {
                                additionServiceDetail.AdditionServiceId = existItem.Id;
                                additionServiceDetail.HospitalId = existItem.HospitalId;
                                this.unitOfWork.Repository<AdditionServiceDetails>().Create(additionServiceDetail);
                            }
                        }
                    }
                    else
                    {
                        // XÓA HẾT TẤT CẢ CHI TIẾT DỊCH VỤ PHÁT SINH TRONG DB
                        var currentExistAdditionServiceDetails = await this.unitOfWork.Repository<AdditionServiceDetails>().GetQueryable()
                            .Where(e => !e.Deleted && e.AdditionServiceId == existItem.Id).ToListAsync();
                        if(currentExistAdditionServiceDetails != null && currentExistAdditionServiceDetails.Any())
                        {
                            foreach (var currentExistAdditionServiceDetail in currentExistAdditionServiceDetails)
                            {
                                this.unitOfWork.Repository<AdditionServiceDetails>().Delete(currentExistAdditionServiceDetail);
                            }
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

        protected override DataTable AddDataTableRow(DataTable dt, AdditionServices item)
        {
            dt.Rows.Add(item.Id, null, item.Created, item.CreatedBy, item.Updated, item.UpdatedBy, item.Deleted, item.Active, item.HospitalId, item.Code, item.Name, item.Description);
            return dt;
        }

    }
}
