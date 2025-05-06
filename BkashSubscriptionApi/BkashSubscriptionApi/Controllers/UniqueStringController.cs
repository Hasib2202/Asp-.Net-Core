using BkashSubscriptionApi.Models;
using BkashSubscriptionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BkashSubscriptionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniqueStringController : ControllerBase
    {
        private readonly IUniqueStringService _uniqueStringService;

        public UniqueStringController(IUniqueStringService uniqueStringService)
        {
            _uniqueStringService = uniqueStringService;
        }

        // Task 1: Create an API which will return a unique string every time the API is getting a hit
        [HttpGet]
        public ActionResult<ApiResponse<string>> GetUniqueString()
        {
            string uniqueString = _uniqueStringService.GenerateUniqueString();
            return Ok(ApiResponse<string>.SuccessResponse(uniqueString, "Unique string generated successfully"));
        }
    }
}