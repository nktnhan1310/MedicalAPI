using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Loại thông báo
    /// </summary>
    [Table("NotificationTypes")]
    public class NotificationTypes : MedicalCatalogueAppDomain
    {
    }
}
