using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services.Base
{
    public interface ICatalogueHospitalService<E, T> : ICoreHospitalService<E, T> where E : MedicalCatalogueAppDomainHospital where T : BaseHospitalSearch
    {
        Task<AppDomainImportResult> ImportTemplateFile(Stream stream, string createdBy, int? hospitalId);
    }
}
