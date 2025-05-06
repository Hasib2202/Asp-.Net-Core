namespace BkashSubscriptionApi.Services
{
    public class UniqueStringService : IUniqueStringService
    {
        public string GenerateUniqueString()
        {
            // Generate a unique string using GUID and timestamp to ensure uniqueness
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string guid = Guid.NewGuid().ToString("N");

            // Combine timestamp and GUID for better uniqueness
            return $"{timestamp}-{guid}";
        }
    }
}