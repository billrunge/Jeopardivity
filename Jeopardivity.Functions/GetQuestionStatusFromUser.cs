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
    public static class GetQuestionStatusFromUser
    {
        private static int question;
        private static bool answerable;
        private static bool userBuzzed;

        [FunctionName("GetQuestionStatusFromUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int user = data.User;

            await GetQuestionStatusFromUserAsync(user);

            var returnObject = new { Question = question, Answerable = answerable, UserBuzzed = userBuzzed };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }


        private static async Task GetQuestionStatusFromUserAsync(int user)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        SELECT TOP 1 Q.[Question], 
                                     Q.[Answerable], 
                                     IIF((SELECT Count(*) 
                                          FROM   [Buzz] 
                                          WHERE  [User] = @User 
                                                 AND [Question] = Q.Question) > 0, 1, 0) AS [UserBuzzed] 
                        FROM   [Question] Q 
                               INNER JOIN [Game] G 
                                       ON Q.[Game] = G.[Game] 
                               INNER JOIN [User] U 
                                       ON U.[Game] = G.[Game] 
                        WHERE  U.[User] = @User 
                        ORDER  BY [Question] DESC";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@User", SqlDbType = SqlDbType.Int, Value = user });

                SqlDataReader dataReader = await command.ExecuteReaderAsync();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        question = Convert.ToInt32(dataReader["Question"]);
                        answerable = Convert.ToInt32(dataReader["Answerable"]) == 1 ? true : false ;
                        userBuzzed = Convert.ToInt32(dataReader["UserBuzzed"]) == 1 ? true : false;
                    }
                }
                else
                {
                    question = 0;
                    answerable = false;
                    userBuzzed = false;
                }
            }
        }
    }
}
