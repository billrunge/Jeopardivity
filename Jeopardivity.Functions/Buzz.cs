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
    public static class Buzz
    {
        private static readonly HttpClient _client = new HttpClient();

        [FunctionName("Buzz")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string jwt = data.JWT;

            Question questionHelper = new Question()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            User userHelper = new User()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            Libraries.Buzz buzzHelper = new Libraries.Buzz()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };


            int user = userHelper.GetUserFromJWT(jwt);
            Question.QuestionStatus questionStatus = await questionHelper.GetQuestionStatusFromUserAsync(user);

            await buzzHelper.BuzzAsync(user, questionStatus.question);
            int alex = await userHelper.GetAlexFromQuestionAsync(questionStatus.question);
            string userName = await userHelper.GetUserNameFromUserAsync(user);
            await SendMessageAsync(alex, userName);

            var returnObject = new { Success = true };


            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
            //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static async Task SendMessageAsync(int alex, string userName)
        {
            string payload = @" {'UserID':'User" + alex + "', 'Message':'" + userName + "' }";

            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            await _client.PostAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/api/SendMessage", content);

        }
    }
}

