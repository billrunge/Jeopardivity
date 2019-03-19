using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Jeopardivity.Libraries
{
    public class JWT
    {
        public async Task<string> GenerateJwtAsync(int user, int game, string userName, string gameCode, bool isAlex, string key)
        {
            //SymmetricSecurityKey securityKey =
            //    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));

            SymmetricSecurityKey securityKey =
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));

            SigningCredentials credentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtHeader header = new JwtHeader(credentials);

            JwtPayload payload = new JwtPayload
           {
                {"User", user},
                {"UserName", userName },
                {"Game", game },
                {"GameCode", gameCode },
                {"IsAlex", isAlex }
           };

            JwtSecurityToken secToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return await Task.Run(() => handler.WriteToken(secToken));

        }
    }
}
