using System.Text.Json;
using BkashSubscriptionApi.Models;
using BkashSubscriptionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BkashSubscriptionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomJsonController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public RandomJsonController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // Task 2: Create a model which will hold random values and return the new generated JSON as string
        [HttpGet]
        public ActionResult<ApiResponse<string>> GetRandomJson()
        {
            var randomModel = _subscriptionService.GenerateRandomSubscriptionRequest();
            string jsonString = JsonSerializer.Serialize(randomModel, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return Ok(ApiResponse<string>.SuccessResponse(jsonString, "Random JSON generated successfully"));
        }
    }
}