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
    public static class GetGameFromUser
    {
        [FunctionName("GetGameFromUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int user = data.User;

            Helper helper = new Helper()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            int game = await helper.GetGameFromUserAsync(user);

            var returnObject = new { Game = game };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }
    }
}
