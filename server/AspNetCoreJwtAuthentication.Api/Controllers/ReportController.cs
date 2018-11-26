using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreJwtAuthentication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
