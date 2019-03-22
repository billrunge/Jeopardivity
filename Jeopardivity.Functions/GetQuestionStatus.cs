using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Jeopardivity.Libraries;

namespace Jeopardivity.Functions
{
    public static class GetQuestionStatus
    {
        [FunctionName("GetQuestionStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string jwt = data.JWT;

            User userHelper = new User() { SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") };
            Question questionHelper = new Question() { SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") };
            JWT jwtHelper = new JWT();

            Question.QuestionStatus questionStatus = await questionHelper.GetQuestionStatusFromUserAsync(jwtHelper.GetUserFromJWT(jwt));

            var returnObject = new { Question = questionStatus.question, Answerable = questionStatus.answerable, UserBuzzed = questionStatus.userBuzzed };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }
    }
}
