using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Jeopardivity.Libraries
{
    public class Game
    {
        public string SqlConnectionString { get; set; }

        public async Task<int> GetGameFromCodeAsync(string gameCode)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [Game] 
                        FROM   [Game] 
                        WHERE  GameCode = @GameCode";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@GameCode", SqlDbType = SqlDbType.NVarChar, Value = gameCode });

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<int> CreateGameAsync(string gameCode)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = SqlConnectionString })
            {
                await connection.OpenAsync();
                string sql = @"
                        INSERT INTO [Game] 
                                    ([GameCode]) 
                        VALUES      (@GameCode)

                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@GameCode", SqlDbType = SqlDbType.NVarChar, Value = gameCode });

                return (int)await command.ExecuteScalarAsync();

            }
        }

        public string GenerateGameCode(int size)
        {
            Random rand = new Random();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] chars = new char[size];

            for (int i = 0; i < size; i++)
            {
                chars[i] = alphabet[rand.Next(alphabet.Length)];
            }

            return new string(chars);
        }




    }
}
