﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medical.Models.DomainModel
{
    public class MedicalCatalogueAppDomainHospitalModel : MedicalAppDomainHospitalModel
    {
        [StringLength(50, ErrorMessage = "Mã không được dài quá 50 kí tự")]
        public string Code { get; set; }
        [StringLength(500, ErrorMessage = "Tên không được dài quá 500 kí tự")]
        public string Name { get; set; }
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 kí tự")]
        public string Description { get; set; }
    }
}
