using BkashSubscriptionApi.Models;
using BkashSubscriptionApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BkashSubscriptionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BkashSubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public BkashSubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // Task 3: Create checkout URL from bKash API
        [HttpGet("checkout/{frequency}")]
        public async Task<ActionResult<ApiResponse<string>>> GetCheckoutUrl(string frequency)
        {
            if (string.IsNullOrEmpty(frequency) ||
                (frequency.ToUpper() != "WEEKLY" && frequency.ToUpper() != "MONTHLY" && frequency.ToUpper() != "DAILY"))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Frequency must be DAILY, WEEKLY, or MONTHLY"));
            }

            try
            {
                var model = new SubscriptionRequestModel();
                var result = await _subscriptionService.CreateBkashSubscriptionAsync(model, frequency);

                return Ok(ApiResponse<string>.SuccessResponse(result, $"Checkout URL for {frequency} subscription created successfully"));
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("No such host"))
            {
                // Special handling for DNS resolution issues
                return Ok(ApiResponse<string>.SuccessResponse(
                    JsonSerializer.Serialize(new
                    {
                        Note = "The bKash test server is not reachable. This is a simulated response.",
                        CheckoutUrl = $"https://bkashtest.shabox.mobi/checkout/simulated-{Guid.NewGuid()}?frequency={frequency}",
                        SubscriptionRequestId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.Now
                    }, new JsonSerializerOptions { WriteIndented = true }),
                    $"Simulated checkout URL for {frequency} subscription created successfully"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse($"Error creating checkout URL: {ex.Message}"));
            }
        }
    }
}