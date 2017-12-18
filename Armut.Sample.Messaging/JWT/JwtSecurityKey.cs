using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Armut.Sample.Messaging.JWT
{
    public static class JwtSecurityKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}
