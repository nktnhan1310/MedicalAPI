using Medical.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Entities
{
    /// <summary>
    /// Cấu hình đăng nhập facebook
    /// </summary>
    public class FaceBookAuthSettings : MedicalAppDomain
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
