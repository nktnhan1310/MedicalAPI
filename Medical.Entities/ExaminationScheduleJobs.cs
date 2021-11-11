using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class ExaminationScheduleJobs : MedicalAppDomain
    {
        public int? ExaminationFormId { get; set; }
        public string JobId { get; set; }
    }
}
