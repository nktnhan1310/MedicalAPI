using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.Controllers.Catalogue
{
    [Route("api/catalogue")]
    [ApiController]
    [Description("Quản lý danh mục")]
    [Authorize]
    public class CatalogueController : ControllerBase
    {
    }
}
