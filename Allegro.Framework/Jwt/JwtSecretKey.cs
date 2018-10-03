using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Allegro.Framework.Jwt
{
    public class JwtSecretKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}
