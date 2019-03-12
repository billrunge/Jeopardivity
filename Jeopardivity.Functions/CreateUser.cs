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
    public static class CreateUser
    {
        [FunctionName("CreateUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string name = data.Name;
            int game = data.Game;
            bool isAlex = data.IsAlex;
            int user = await CreateUserAsync(name, game, isAlex);
            var returnObject = new { User = user };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
/*                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");*/
        }

        private static async Task<int> CreateUserAsync(string name, int game, bool isAlex)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
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
    }
}
