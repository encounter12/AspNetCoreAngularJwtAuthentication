using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreJwtAuthentication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        [Authorize(Policy = "RequireAdministratorRole")]
        public IEnumerable<string> Get()
        {
            return new string[] { "report1", "report2" };
        }
    }
}
