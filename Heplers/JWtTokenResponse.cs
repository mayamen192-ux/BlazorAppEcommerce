namespace BlazorAppEcommerce.Heplers
{
    public class JwtTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
