using Medical.Entities;
using Medical.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface
{
    public interface IDiagnoticTypeService : ICatalogueHospitalService<DiagnoticTypes, BaseHospitalSearch>
    {
    }
}
