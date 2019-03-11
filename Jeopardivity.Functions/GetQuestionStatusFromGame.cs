using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Jeopardivity.Functions
{
    public static class GetQuestionStatusFromGame
    {
        [FunctionName("GetQuestionStatusFromGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int game = data.Game;

            QuestionStatus questionStatus = await GetQuestionStatusFromGameAsync(game);

            var returnObject = new { CurrentQuestion = questionStatus.currentQuestion, QuestionCount = questionStatus.questionCount };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }

        private static async Task<QuestionStatus> GetQuestionStatusFromGameAsync(int game)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        DECLARE @QuestionCount int = (SELECT Count(*) 
                           FROM   [Question] 
                           WHERE  [Game] = @Game) 
                        DECLARE @CurrentQuestion int 

                        IF @QuestionCount < 1 
                          SET @CurrentQuestion = 0; 
                        ELSE 
                          SET @CurrentQuestion = (SELECT TOP 1 [Question] 
                                                  FROM   [Question] 
                                                  WHERE  [Game] = @Game 
                                                  ORDER  BY [Started] DESC) 

                        SELECT @CurrentQuestion        AS [CurrentQuestion], 
                               (SELECT Count(*) 
                                FROM   [Question] 
                                WHERE  [Game] = @Game) AS [QuestionCount]";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });

                SqlDataReader dataReader = await command.ExecuteReaderAsync();
                QuestionStatus questionStatus = new QuestionStatus();

                while (dataReader.Read())
                {
                    questionStatus.currentQuestion = Convert.ToInt32(dataReader["CurrentQuestion"]);
                    questionStatus.questionCount = Convert.ToInt32(dataReader["QuestionCount"]);
                }
                return questionStatus;         

            }
        }

        public struct QuestionStatus
        {
            public int currentQuestion, questionCount;
        }
    }

}
