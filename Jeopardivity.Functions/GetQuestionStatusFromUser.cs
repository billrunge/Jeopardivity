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
using Jeopardivity.Libraries;

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

            var helper = new Helper()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            Helper.QuestionStatus questionStatus = await helper.GetQuestionStatusFromUserAsync(user);

            var returnObject = new { Question = questionStatus.question, Answerable = questionStatus.answerable, UserBuzzed = questionStatus.userBuzzed };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }
    }
}
