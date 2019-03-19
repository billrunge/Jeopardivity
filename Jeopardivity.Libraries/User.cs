using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Jeopardivity.Libraries
{
    public class User
    {
        public string SqlConnectionString { get; set; }

        public async Task<int> CreateUserAsync(string name, int game, bool isAlex)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        INSERT INTO [User] 
                                    ([Name], 
                                     [Game],
                                     [IsAlex]) 
                        VALUES      (@Name, 
                                     @Game,
                                     @IsAlex)

                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, Value = name });
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });
                command.Parameters.Add(new SqlParameter { ParameterName = "@IsAlex", SqlDbType = SqlDbType.Bit, Value = isAlex });

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public int GetUserFromJWT(string jwt)
        {
            var token = new JwtSecurityToken(jwtEncodedString: jwt);
            return int.Parse(token.Claims.First(c => c.Type == "User").Value);
        }

        public async Task<string> GetUserNameFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT [Name] 
                        FROM   [User] 
                        WHERE  [User] = @User ";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = System.Data.SqlDbType.Int, Value = user });

                return (string)await command.ExecuteScalarAsync();
            }
        }


        public async Task<int> GetAlexFromQuestionAsync(int question)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT U.[User] 
                        FROM   [User] U 
                               INNER JOIN [Question] Q 
                                       ON U.[Game] = Q.[Game] 
                        WHERE  U.[IsAlex] = 1 
                               AND Q.[Question] = @Question";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Question", SqlDbType = SqlDbType.Int, Value = question });

                return (int)await command.ExecuteScalarAsync();
            }
        }



    }
}
