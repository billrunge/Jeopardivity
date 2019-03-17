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
using System.Net.Http;
using System.Text;

namespace Jeopardivity.Functions
{
    public static class CreateQuestion
    {

        private static readonly HttpClient _client = new HttpClient();
        private static int game;
        private static int question;

        [FunctionName("CreateQuestion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            game = data.Game;
            question = await CreateQuestionAsync(game);
            await SendMessageAsync();

            var returnObject = new { Question = question };


            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }


        public static async Task<int> CreateQuestionAsync(int game)
        {

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                        INSERT INTO [Question] 
                                    ([Game], 
                                     [Answerable]) 
                        VALUES      (@Game, 
                                     0)

                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@Game", SqlDbType = SqlDbType.Int, Value = game });

                return (int)await command.ExecuteScalarAsync();

            }
        }

        private static async Task SendMessageAsync()
        {
            string payload = @" {'UserID':'Game" + game + "', 'Message':'NewQuestion' }";

            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            await _client.PostAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/api/SendMessage", content);

        }


    }
}
