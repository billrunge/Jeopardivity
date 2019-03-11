using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace Jeopardivity.Functions
{
    public static class GetCodeFromGame
    {
        [FunctionName("GetCodeFromGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int game = data.Game;

            string gameCode = await GetCodeFromGameAsync(game);

            var returnObject = new { GameCode = gameCode };
            
            return (ActionResult)new OkObjectResult(returnObject);
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static async Task<string> GetCodeFromGameAsync(int game)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [GameCode] 
                        FROM   [Game] 
                        WHERE  [Game] = @Game";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });           

                return (string)await command.ExecuteScalarAsync();
            }
        }
    }
}
