using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Jeopardivity.Libraries;

namespace Jeopardivity.Functions
{
    public static class DatabaseCleanup
    {
        [FunctionName("DatabaseCleanup")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 0 7 * * MON")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"DatabaseCleanup function executed at: {DateTime.Now}");

            Database database = new Database() { SqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") };
            await database.DropAllTablesAsync();
        }
    }
}