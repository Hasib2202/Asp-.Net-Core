using Microsoft.AspNetCore.Mvc;
using SubscriptionApi.Models;
using System.Net.Http.Headers;

namespace SubscriptionApi.Controllers
{
    public class CheckoutController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("api/checkout/weekly")]
        public async Task<IActionResult> GenerateWeeklyUrl()
        {
            return await GenerateUrl("WEEKLY");
        }

        [HttpPost("api/checkout/monthly")]
        public async Task<IActionResult> GenerateMonthlyUrl()
        {
            return await GenerateUrl("MONTHLY");
        }

        private async Task<IActionResult> GenerateUrl(string frequency)
        {
            var client = _httpClientFactory.CreateClient("CheckoutClient");
            var requestId = Guid.NewGuid().ToString();


            var payload = new SubscriptionModel
            {
                Amount = "1",
                FirstPaymentIncludedInCycle = "True",
                ServiceId = "100001",
                Currency = "BDT",
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ExpiryDate = frequency == "WEEKLY"
                    ? DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")
                    : DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"),
                Frequency = frequency,
                SubscriptionType = "BASIC",
                MaxCapRequired = "False",
                MerchantShortCode = "01307153119",
                PayerType = "CUSTOMER",
                PaymentType = "FIXED",
                RedirectUrl = "https://yourdomain.com/redirect",
                SubscriptionRequestId = requestId,
                SubscriptionReference = "0123456789", // Your phone number
                Ckey = "000001"
            };

            var response = await client.PostAsJsonAsync(
                "home/MultiTournamentInBuildCheckoutUrl",
                payload
            );
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"External API Response: {responseContent}");


            return Ok(new
            {
                PaymentUrl = responseContent,
                SubscriptionRequestId = requestId
            });
        }
    }
}