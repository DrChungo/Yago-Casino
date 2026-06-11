using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers {
    //comentario
    [ApiController]
    [Route("api/[controller]")]
    
    public class HealthController : ControllerBase
    {

    [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
    
}
