using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Jeopardivity.Functions
{
    public static class CreateGame
    {

        [FunctionName("CreateGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            var returnObject = new { GameCode = await CreateGameAsync() };

            return new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }

        private static async Task<string> CreateGameAsync()
        {
            string gameCode = GenerateGameCode(5);
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        INSERT INTO [Game] 
                                    ([GameCode], 
                                     [Created]) 
                        VALUES      (@GameCode, 
                                     Getutcdate())";
                
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@GameCode", SqlDbType = SqlDbType.NVarChar, Value = gameCode });

                await command.ExecuteNonQueryAsync();
            }
            return gameCode;
        }

        private static string GenerateGameCode(int size)
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
