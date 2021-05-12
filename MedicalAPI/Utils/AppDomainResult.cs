﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Utils
{
    public class AppDomainResult
    {
        public AppDomainResult()
        {
            Messages = new List<string>();
        }
        public bool Success { get; set; }
        public object Data { get; set; }
        public int ResultCode { get; set; }
        public IList<string> Messages { get; set; }
        public string ResultMessage { get; set; }
    }
}
