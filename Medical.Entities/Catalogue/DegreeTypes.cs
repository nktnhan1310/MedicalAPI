﻿using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Học vị của bác sĩ
    /// </summary>
    [Table("DegreeTypes")]
    public class DegreeTypes : MedicalCatalogueAppDomain
    {
    }
}
