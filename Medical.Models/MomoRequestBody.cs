using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models
{
    public class MomoRequestBody
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public string orderInfo { get; set; }
        public string orderType { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string payType { get; set; }
        public long responseTime { get; set; }
        public string extraData { get; set; }
        //public string accessKey { get; set; }
        //public int errorCode { get; set; }
        //public string localMessage { get; set; }
        public string signature { get; set; }
    }
}
