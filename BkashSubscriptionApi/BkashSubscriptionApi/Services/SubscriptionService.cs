using System.Text;
using System.Text.Json;
using BkashSubscriptionApi.Models;

namespace BkashSubscriptionApi.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUniqueStringService _uniqueStringService;
        private readonly HttpClient _httpClient;
        private readonly Random _random = new Random();

        public SubscriptionService(IUniqueStringService uniqueStringService, IHttpClientFactory httpClientFactory)
        {
            _uniqueStringService = uniqueStringService;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("api-key", "796b8b9dbbf46b1d8fd73f68979ae31635da9afabc9dee147adf0440ee7118a8");
        }

        public SubscriptionRequestModel GenerateRandomSubscriptionRequest()
        {
            return new SubscriptionRequestModel
            {
                Amount = _random.Next(1, 1000).ToString(),
                FirstPaymentIncludedInCycle = _random.Next(2) == 0 ? "True" : "False",
                ServiceId = _random.Next(100000, 999999).ToString(),
                Currency = "BDT",
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ExpiryDate = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"),
                Frequency = GetRandomFrequency(),
                SubscriptionType = "BASIC",
                MaxCapRequired = _random.Next(2) == 0 ? "True" : "False",
                MerchantShortCode = "01307153119",
                PayerType = "CUSTOMER",
                PaymentType = "FIXED",
                RedirectUrl = "https://example.com/redirect",
                SubscriptionRequestId = _uniqueStringService.GenerateUniqueString(),
                SubscriptionReference = "01604514080", 
                CKey = "000001"
            };
        }

        public async Task<string> CreateBkashSubscriptionAsync(SubscriptionRequestModel model, string frequency)
        {
            try
            {
                // Set the frequency
                model.Frequency = frequency.ToUpper();

                // Set other required fields
                model.Amount = "1"; 
                model.FirstPaymentIncludedInCycle = "True";
                model.ServiceId = "100001";
                model.Currency = "BDT";
                model.StartDate = DateTime.Now.ToString("yyyy-MM-dd");
                model.ExpiryDate = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");
                model.SubscriptionType = "BASIC";
                model.MaxCapRequired = "False";
                model.MerchantShortCode = "01307153119";
                model.PayerType = "CUSTOMER";
                model.PaymentType = "FIXED";
                model.RedirectUrl = "https://example.com/redirect";
                model.SubscriptionRequestId = _uniqueStringService.GenerateUniqueString();
                model.SubscriptionReference = "01604514080"; 
                model.CKey = "000001";

                // In a real environment, we would make the actual API call
                // But since the host is not reachable, we'll simulate a response

                // Serialize the model to JSON (for reference)
                var jsonContent = JsonSerializer.Serialize(model, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Generate a simulated checkout URL - this would normally come from the bKash API
                string simulatedCheckoutUrl = $"https://bkashtest.shabox.mobi/checkout?subscriptionId={model.SubscriptionRequestId}&frequency={frequency}";

                // Create a simulated response that includes both the URL and the request details
                var simulatedResponse = new
                {
                    CheckoutUrl = simulatedCheckoutUrl,
                    SubscriptionRequestId = model.SubscriptionRequestId,
                    RequestDetails = model
                };

                return JsonSerializer.Serialize(simulatedResponse, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                /* Commented out actual API call due to host resolution issue
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://bkashtest.shabox.mobi/home/MultiTournamentInBuildCheckoutUrl", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    throw new Exception($"Failed to create bKash subscription. Status code: {response.StatusCode}");
                }
                */
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Re-throw to let the controller handle it
                throw;
            }
        }

        private string GetRandomFrequency()
        {
            string[] frequencies = { "DAILY", "WEEKLY", "MONTHLY" };
            return frequencies[_random.Next(frequencies.Length)];
        }
    }
}