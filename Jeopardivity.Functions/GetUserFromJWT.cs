using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Jeopardivity.Functions
{
    public static class GetUserFromJWT
    {
        [FunctionName("GetUserFromJWT")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string jwt = data.JWT;

            var returnObject = new { User = await GetUserFromJwtAsync(jwt) };

            return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(returnObject));
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }


        private static async Task<Int64> GetUserFromJwtAsync(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(jwt);

            return (Int64)token.Payload["User"];
        }
    }
}
