using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Jeopardivity.Libraries;

namespace Jeopardivity.Functions
{
    public static class MakeQuestionAnswerable
    {
        private static readonly HttpClient _client = new HttpClient();
        private static int game;
        private static int question;



        [FunctionName("MakeQuestionAnswerable")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            question = data.Question;
            game = data.Game;

            Question questionHelper = new Question()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            await questionHelper.MakeQuestionAnswerableAsync(question);
            await SendMessageAsync();

            var returnObject = new { Status = "Question is now Answerable" };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }

        private static async Task SendMessageAsync()
        {
            string payload = @" {'UserID':'Game" + game + "', 'Message':'Answerable' }";

            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            await _client.PostAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/api/SendMessage", content);
        }
    }
}
