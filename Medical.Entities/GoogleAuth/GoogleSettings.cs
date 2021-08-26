using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class GoogleSettings : MedicalAppDomain
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
