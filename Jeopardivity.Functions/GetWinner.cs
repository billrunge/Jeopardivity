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
    public static class GetWinner
    {
        [FunctionName("GetWinner")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int question = data.Question;

            var returnObject = new { User = await GetWinnerAsync(question) };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }

        public static async Task<int> GetWinnerAsync(int question)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        DECLARE @User int = (SELECT TOP 1 [User] 
                           FROM   [Buzz] B 
                                  INNER JOIN [Question] Q 
                                          ON B.[Question] = Q.[Question] 
                           WHERE  Q.[Ended] IS NOT NULL 
                                  AND [Buzzed] > [Ended] 
                                  AND B.[Question] = @Question 
                           ORDER  BY [Buzzed] ASC) 

                        IF ( @User IS NULL ) 
                          BEGIN 
                              SET @User = 0 
                          END 

                        SELECT @User";


                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Question", SqlDbType = SqlDbType.Int, Value = question });
            
                int winningUser = (int)await command.ExecuteScalarAsync();

                return winningUser;
            }

        }

    }
}
