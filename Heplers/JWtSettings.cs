namespace BlazorAppEcommerce.Helpers
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; }
    }
}