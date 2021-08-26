using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    public class SearchNotification : BaseHospitalSearch
    {
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public int? NotificationId { get; set; }
        public int? NotificationTypeId { get; set; }

    }
}
