using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HomeController : ControllerBase
    {
        //[ApiKey]
        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok();


    }
}
