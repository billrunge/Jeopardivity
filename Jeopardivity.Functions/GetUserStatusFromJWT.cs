using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jeopardivity.Functions
{
    public static class GetUserStatusFromJWT
    {
        private static readonly HttpClient _client = new HttpClient();

        [FunctionName("GetUserStatusFromJWT")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string jwt = data.JWT;
            long user = await GetUserFromJwtAsync(jwt);
            int game = (int)await GetGameFromJwtAsync(jwt);
            string userName = await GetUserNameFromJwtAsync(jwt);
            bool isAlex = await GetIsAlexFromJwtAsync(jwt);
            string gameCode = await GetGameCodeFromApiAsync(game);

            QuestionStatus questionStatus = await GetQuestionStatusFromApiAsync(game);

            var returnObject = new
            {
                User = user,
                UserName = userName,
                Game = game,
                GameCode = gameCode,
                CurrentQuestion = questionStatus.currentQuestion,
                QuestionCount = questionStatus.questionCount,
                IsAlex = isAlex
            };

            return new OkObjectResult(JsonConvert.SerializeObject(returnObject));
            //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }


        //private static async Task<string> GetUserNameFromApiAsync(long user)
        //{
        //    string payload = @" {'User':" + user + "}";

        //    StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

        //    HttpResponseMessage response = await _client.PostAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/api/GetNameFromUser", content);

        //    string responseString = await response.Content.ReadAsStringAsync();
        //    dynamic data = JsonConvert.DeserializeObject(responseString);

        //    return data.Name;
        //}

        //private static async Task<int> GetGameFromApiAsync(long user)
        //{
        //    string payload = @" {'User':" + user + "}";

        //    StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

        //    HttpResponseMessage response = await _client.PostAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/api/GetGameFromUser", content);

        //    string responseString = await response.Content.ReadAsStringAsync();
        //    dynamic data = JsonConvert.DeserializeObject(responseString);

        //    return data.Game;
        //}


        private static async Task<string> GetGameCodeFromApiAsync(int game)
        {
            string payload = @" {'Game':" + game + "}";

            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/api/GetCodeFromGame", content);

            string responseString = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseString);

            return data.gameCode;
        }

        private static async Task<QuestionStatus> GetQuestionStatusFromApiAsync(int game)
        {
            string payload = @"{'Game':" + game + "}";

            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/api/GetQuestionStatusFromGame", content);

            string responseString = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseString);

            return new QuestionStatus
            {
                currentQuestion = data.CurrentQuestion,
                questionCount = data.QuestionCount
            };

        }

        private static async Task<long> GetUserFromJwtAsync(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(jwt);

            return (long)token.Payload["User"];
        }

        private static async Task<long> GetGameFromJwtAsync(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(jwt);

            return (long)token.Payload["Game"];
        }

        private static async Task<bool> GetIsAlexFromJwtAsync(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(jwt);

            return (bool)token.Payload["IsAlex"];
        }

        private static async Task<string> GetUserNameFromJwtAsync(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(jwt);

            return (string)token.Payload["UserName"];
        }


        public struct QuestionStatus
        {
            public int currentQuestion, questionCount;
        }
    }
}
