using System.Text.Json.Serialization;

namespace SubscriptionApi.Models
{
    public class SubscriptionModel
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("firstPaymentIncludedInCycle")]
        public string FirstPaymentIncludedInCycle { get; set; }

        [JsonPropertyName("serviceld")] // Note: Matches the JSON typo
        public string ServiceId { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }

        [JsonPropertyName("expiryDate")]
        public string ExpiryDate { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("subscriptionType")]
        public string SubscriptionType { get; set; }

        [JsonPropertyName("maxCapRequired")]
        public string MaxCapRequired { get; set; }

        [JsonPropertyName("merchantShortCode")]
        public string MerchantShortCode { get; set; }

        [JsonPropertyName("payerType")]
        public string PayerType { get; set; }

        [JsonPropertyName("paymentType")]
        public string PaymentType { get; set; }

        [JsonPropertyName("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("subscriptionRequestId")]
        public string SubscriptionRequestId { get; set; }

        [JsonPropertyName("subscriptionReference")]
        public string SubscriptionReference { get; set; }

        [JsonPropertyName("ckey")]
        public string Ckey { get; set; }
    }
}