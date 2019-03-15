using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Jeopardivity.Functions
{
    public static class SignalR
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
            [SignalRConnectionInfo(HubName = "jeopardivity")]SignalRConnectionInfo connectionInfo,
            ILogger log)
        {
            return connectionInfo;
        }

        [FunctionName("SendMessage")]
        public static Task SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req,
            [SignalR(HubName = "jeopardivity")]IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string userId = data.UserID;
            string message = data.Message;

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = userId,
                    Arguments = new[] { message }
                });
        }

    }
}
