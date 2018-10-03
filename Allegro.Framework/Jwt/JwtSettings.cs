using Microsoft.IdentityModel.Tokens;

namespace Allegro.Framework.Jwt
{
    internal static class JwtSettings
    {
        public  static TokenValidationParameters JwtValidationParameters { get { return GetJwtValidationParameters(); } }

        private static TokenValidationParameters GetJwtValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "Allegro.Authentication",
                ValidAudience = "Allegro.Authentication",
                IssuerSigningKey = JwtSecretKey.Create("allegrosecretkey")
            };
        }
    }
}
