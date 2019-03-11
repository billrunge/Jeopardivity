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
    public static class GetNameFromUser
    {
        [FunctionName("GetNameFromUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int user = data.User;
                       
            var returnObject = new { Name = await GetNameFromUserAsync(user) };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }


        private static async Task<string> GetNameFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 [Name] 
                        FROM   [User] 
                        WHERE  [User] = @User";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });

                return (string)await command.ExecuteScalarAsync();
            }


        }

    }
}
