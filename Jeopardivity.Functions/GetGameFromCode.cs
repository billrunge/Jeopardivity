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
    public static class GetGameFromCode
    {
        [FunctionName("GetGameFromCode")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //requestBody = (requestBody[0] == '{') ? requestBody : "{ " + requestBody + " }";
            log.LogInformation(requestBody);
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string gameCode = data.GameCode;

            int game = await GetGameFromCodeAsync(gameCode);

            var returnObject = new { Game = game };
            
            return (ActionResult)new OkObjectResult(returnObject);
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }


        private static async Task<int> GetGameFromCodeAsync(string gameCode)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
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
    }
}
