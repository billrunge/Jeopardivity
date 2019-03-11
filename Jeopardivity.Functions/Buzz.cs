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
    public static class Buzz
    {
        [FunctionName("Buzz")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int user = data.User;
            int game = data.Game;
            int question = data.Question;

            await BuzzAsync(user, game, question);

            var returnObject = new { Winner = await IsUserWinner(user, question) };


            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
            //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        public static async Task BuzzAsync(int user, int game, int question)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        IF NOT EXISTS (SELECT TOP 1 [User] 
                                       FROM   [Buzz] 
                                       WHERE  [User] = @User 
                                              AND [Game] = @Game) 
                          BEGIN 
                              INSERT INTO [Buzz] 
                                          ([User], 
                                           [Game], 
                                           [Buzzed], 
                                           [Question]) 
                              VALUES      (@User, 
                                           @Game, 
                                           Getutcdate(), 
                                           @Question)
                          END";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });
                command.Parameters.Add(new SqlParameter { ParameterName = "@Question", SqlDbType = SqlDbType.Int, Value = question });

                await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task<bool> IsUserWinner(int user, int question)
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

                return (winningUser == user) ? true : false;
            }    

        }

    }
}

