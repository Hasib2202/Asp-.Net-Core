using Microsoft.AspNetCore.Mvc;
using SubscriptionApi.Models;
using System.Text.Json;

namespace SubscriptionApi.Controllers
{
    public class RandomModelController : ControllerBase
    {
        private static readonly string[] Frequencies = { "DAILY", "WEEKLY", "MONTHLY" };

        [HttpGet("api/randommodel")]
        public IActionResult GetRandomModel()
        {
            var random = new Random();
            var model = new SubscriptionModel
            {
                Amount = random.Next(1, 1000).ToString(),
                FirstPaymentIncludedInCycle = random.Next(2) == 0 ? "True" : "False",
                ServiceId = "10000" + random.Next(1, 9),
                Currency = "BDT",
                StartDate = DateTime.Now.AddDays(random.Next(1, 30)).ToString("yyyy-MM-dd"),
                ExpiryDate = DateTime.Now.AddMonths(random.Next(1, 12)).ToString("yyyy-MM-dd"),
                Frequency = Frequencies[random.Next(Frequencies.Length)],
                SubscriptionType = "BASIC",
                MaxCapRequired = "False",
                MerchantShortCode = "01307153119",
                PayerType = "CUSTOMER",
                PaymentType = "FIXED",
                RedirectUrl = "https://example.com/redirect",
                SubscriptionRequestId = Guid.NewGuid().ToString(),
                SubscriptionReference = "0123456789", // Replace with your phone number
                Ckey = "000001"
            };

            return Ok(JsonSerializer.Serialize(model));
        }
    }
}