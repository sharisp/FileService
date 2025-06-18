namespace Common.Jwt
{
    public class TokenWithExpireResponse : TokenResponse
    {
        public DateTimeOffset AccessTokenExpiresAt { get; set; }

        public DateTimeOffset RefreshTokenExpiresAt { get; set; }
    }
}
