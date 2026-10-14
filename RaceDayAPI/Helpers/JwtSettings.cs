namespace RaceDayAPI.Helpers
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = "DefaultSecretKeyAtLeast32CharactersLong!!";
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int TokenValidityMinutes { get; set; } = 60;
    }
}