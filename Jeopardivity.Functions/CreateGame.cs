using System;
using Jeopardivity.Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Jeopardivity.Functions
{
    public static class CreateGame
    {
        [FunctionName("CreateGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            Helper helper = new Helper()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };

            string gameCode = helper.GenerateGameCode(5);
            int game = await helper.CreateGameAsync(gameCode);

            var returnObject = new { Game = game, GameCode = gameCode };

            return new OkObjectResult(JsonConvert.SerializeObject(returnObject));
        }
    }
}
