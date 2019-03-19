using System;
using Jeopardivity.Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace Jeopardivity.Functions
{
    public static class CreateGame
    {
        [FunctionName("CreateGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            Game gameHelper = new Game()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            User userHelper = new User()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            JWT jwtHelper = new JWT();


            string userName = data.UserName;

            string gameCode = gameHelper.GenerateGameCode(5);
            int game = await gameHelper.CreateGameAsync(gameCode);

            int user = await userHelper.CreateUserAsync(userName, game, true);

            string jwt = await jwtHelper.GenerateJwtAsync(user, 
                            game, 
                            userName, 
                            gameCode, 
                            true, 
                            Environment.GetEnvironmentVariable("JWT_KEY"));

            var returnObject = new { JWT = jwt };

            return new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }
    }
}
