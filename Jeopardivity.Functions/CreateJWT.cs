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
    public static class CreateJWT
    {
        [FunctionName("CreateJWT")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int user = data.User;
            int game = data.Game;
            string gameCode = data.GameCode;
            string userName = data.UserName;
            bool isAlex = data.IsAlex;
            Helper helper = new Helper()
            {
                SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            };


            var returnObject = new { JWT = await helper.GenerateJwtAsync(user, game, userName, gameCode, isAlex, Environment.GetEnvironmentVariable("JWT_KEY")) };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        } 
    }
}
