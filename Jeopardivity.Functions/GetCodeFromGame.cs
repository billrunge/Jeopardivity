using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Jeopardivity.Libraries;

namespace Jeopardivity.Functions
{
    public static class GetCodeFromGame
    {
        [FunctionName("GetCodeFromGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int game = data.Game;
            Helper helper = new Helper()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            string gameCode = await helper.GetCodeFromGameAsync(game);

            var returnObject = new { GameCode = gameCode };
            
            return (ActionResult)new OkObjectResult(returnObject);
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

    }
}
