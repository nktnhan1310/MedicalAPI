using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface.Services
{
    public interface IMedicineService : ICatalogueHospitalService<Medicines, BaseHospitalSearch>
    {
    }
}
