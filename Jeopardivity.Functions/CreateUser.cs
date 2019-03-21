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
    public static class CreateUser
    {
        [FunctionName("CreateUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string userName = data.Name;
            string gameCode = data.GameCode;

            Game gameHelper = new Game()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            User userHelper = new User()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            JWT jwtHelper = new JWT();
            int game = await gameHelper.GetGameFromCodeAsync(gameCode);

            string jwt = await jwtHelper.GenerateJwtAsync(await userHelper.CreateUserAsync(userName, game, false), 
                        game, 
                        userName, 
                        gameCode, 
                        false, 
                        Environment.GetEnvironmentVariable("JWT_KEY"));

            var returnObject = new { JWT = jwt };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
/*                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");*/
        }
    }
}
