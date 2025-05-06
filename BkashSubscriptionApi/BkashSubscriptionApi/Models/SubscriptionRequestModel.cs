using System.Text.Json.Serialization;

namespace BkashSubscriptionApi.Models
{
    public class SubscriptionRequestModel
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        [JsonPropertyName("firstPaymentIncludedInCycle")]
        public string FirstPaymentIncludedInCycle { get; set; } = string.Empty;

        [JsonPropertyName("serviceId")]
        public string ServiceId { get; set; } = string.Empty;

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; } = string.Empty;

        [JsonPropertyName("expiryDate")]
        public string ExpiryDate { get; set; } = string.Empty;

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; } = string.Empty;

        [JsonPropertyName("subscriptionType")]
        public string SubscriptionType { get; set; } = string.Empty;

        [JsonPropertyName("maxCapRequired")]
        public string MaxCapRequired { get; set; } = string.Empty;

        [JsonPropertyName("merchantShortCode")]
        public string MerchantShortCode { get; set; } = string.Empty;

        [JsonPropertyName("payerType")]
        public string PayerType { get; set; } = string.Empty;

        [JsonPropertyName("paymentType")]
        public string PaymentType { get; set; } = string.Empty;

        [JsonPropertyName("redirectUrl")]
        public string RedirectUrl { get; set; } = string.Empty;

        [JsonPropertyName("subscriptionRequestId")]
        public string SubscriptionRequestId { get; set; } = string.Empty;

        [JsonPropertyName("subscriptionReference")]
        public string SubscriptionReference { get; set; } = string.Empty;

        [JsonPropertyName("ckey")]
        public string CKey { get; set; } = string.Empty;
    }
}