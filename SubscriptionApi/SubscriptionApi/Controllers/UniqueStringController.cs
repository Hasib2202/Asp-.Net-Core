using Microsoft.AspNetCore.Mvc;

namespace SubscriptionApi.Controllers
{
    public class UniqueStringController : ControllerBase
    {
        [HttpGet("api/uniquestring")]
        public IActionResult GetUniqueString()
        {
            return Ok(Guid.NewGuid().ToString());
        }
    }
}