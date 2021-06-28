using Medical.Core.App.Controllers;
using Medical.Entities;
using Medical.Extensions;
using Medical.Interface.Services;
using Medical.Models;
using Medical.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers
{
    [Route("api/medical-bills")]
    [ApiController]
    [Description("Quản lý đơn thuốc")]
    [Authorize]
    public class MedicalBillController : CatalogueCoreHospitalController<MedicalBills, MedicalBillModel, SearchMedicalBill>
    {
        private readonly IMedicalBillService medicalBillService;
        private readonly IMedicineService medicineService;

        public MedicalBillController(IServiceProvider serviceProvider, ILogger<CoreHospitalController<MedicalBills, MedicalBillModel, SearchMedicalBill>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IMedicalBillService>();
            medicalBillService = serviceProvider.GetRequiredService<IMedicalBillService>();
            medicineService = serviceProvider.GetRequiredService<IMedicineService>();
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MedicalAppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            SearchMedicalBill baseSearch = new SearchMedicalBill();
            IList<MedicalBillModel> itemModels = new List<MedicalBillModel>();
            baseSearch.PageIndex = 1;
            baseSearch.PageSize = int.MaxValue;
            baseSearch.OrderBy = "Id desc";
            baseSearch.HospitalId = LoginContext.Instance.CurrentUser.HospitalId;
            baseSearch.MedicalBillId = id;
            var pagedItems = await this.catalogueService.GetPagedListData(baseSearch);
            //var item = await this.catalogueService.GetByIdAsync(id);
            if (pagedItems != null && pagedItems.Items.Any())
            {
                var item = pagedItems.Items.FirstOrDefault();
                if (LoginContext.Instance.CurrentUser != null
                    && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue
                    || (LoginContext.Instance.CurrentUser.HospitalId.HasValue && LoginContext.Instance.CurrentUser.HospitalId == item.HospitalId)))
                {
                    var itemModel = mapper.Map<MedicalBillModel>(item);
                    var medicines = await this.medicineService.GetAsync(e => !e.Deleted && e.MedicalBillId == id);
                    if (medicines != null && medicines.Any())
                        itemModel.Medicines = mapper.Map<IList<MedicineModel>>(medicines);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new KeyNotFoundException("Item không tồn tại");

            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật trạng thái toa thuốc
        /// </summary>
        /// <param name="updateMedicalBillStatus"></param>
        /// <returns></returns>
        [HttpPut("update-medical-bill-status")]
        [MedicalAppAuthorize(new string[] { CoreContants.View, CoreContants.Update })]
        public async Task<AppDomainResult> UpdateMedicalBillStatus([FromBody] UpdateMedicalBillStatus updateMedicalBillStatus)
        {
            bool success = await this.medicalBillService.UpdateMedicalBillStatus(updateMedicalBillStatus);
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }
    }
}
