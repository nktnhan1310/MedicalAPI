using Medical.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface.Services
{
    public interface IHospitalService : IDomainService<Hospitals, SearchHospital>
    {
    }
}
