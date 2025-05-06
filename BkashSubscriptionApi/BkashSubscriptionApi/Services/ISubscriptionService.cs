using BkashSubscriptionApi.Models;

namespace BkashSubscriptionApi.Services
{
    public interface ISubscriptionService
    {
        SubscriptionRequestModel GenerateRandomSubscriptionRequest();
        Task<string> CreateBkashSubscriptionAsync(SubscriptionRequestModel model, string frequency);
    }
}