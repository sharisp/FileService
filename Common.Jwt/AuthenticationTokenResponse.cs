using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common.Jwt
{
    public class AuthenticationTokenResponse(IOptionsSnapshot<JwtOptions> optionsSnapshot)
    {
        public TokenWithExpireResponse GetResponseToken(long userId,string userName,List<string> roles=null)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, userName));
            //I do not want to use roles as claims
             var jwtExpire = DateTime.Now.AddMinutes(optionsSnapshot.Value.ExpiresMinutes);

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(optionsSnapshot.Value.SecKey));

            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
         

            var tokenDescriptor = new JwtSecurityToken(optionsSnapshot.Value.Issuer, optionsSnapshot.Value.Audience, claims,
                expires: jwtExpire, signingCredentials: credentials);
            var jwToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return new TokenWithExpireResponse
            {
                AccessToken = jwToken,
                RefreshToken = Guid.NewGuid().ToString(),
                AccessTokenExpiresAt = jwtExpire,
                RefreshTokenExpiresAt = DateTimeOffset.Now.AddHours(optionsSnapshot.Value.RefreshTokenExpiresHours)
            };
        }
    }
}
