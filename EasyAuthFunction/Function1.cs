using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EasyAuthFunction
{
    public static class Function1
    {

        private static readonly HttpClient client = new HttpClient();

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var test = "test";
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string accessToken = req.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"];
                log.LogInformation($"Token: {accessToken}");
                // Call into the Azure AD Graph API using HTTP primitives and the
                // Azure AD access token.
                var url = "https://graph.microsoft.com/v1.0/me/memberOf";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" ,accessToken);
                log.LogInformation($"client {client.DefaultRequestHeaders.Authorization}");
                var response = await client.GetAsync(url);
                log.LogInformation($"Status Code : {response.StatusCode} ");
                log.LogInformation($"{await response.Content.ReadAsStringAsync()}");
                test = await response.Content.ReadAsStringAsync();
            }
            catch(Exception e)
            { 
                log.LogInformation(e.ToString());
            }

            return new OkObjectResult(test);
        }
    }
}
