using Microsoft.AspNetCore.Mvc;

namespace IntakeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get(string id)
        {
            return Ok(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        }
    }
}