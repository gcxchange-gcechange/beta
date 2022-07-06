using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Graph.Beta;
using System.Collections.Generic;
using Microsoft.Graph.Beta.Models;

namespace beta
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            Auth_beta auth = new Auth_beta();
            var graphAPIAuth = auth.graphAuth(log);
            var callteams = applylabel(graphAPIAuth, log).GetAwaiter().GetResult();

            return new OkObjectResult($"{callteams}");
        }
        public static async Task<string> applylabel(GraphServiceClient graphClient, ILogger log)
        {
            log.LogInformation("Call teams");
            try
            {
                var group = new Group
                {
                    AssignedLabels = new List<AssignedLabel>()
                    {
                        new AssignedLabel
                        {
                            LabelId = "a1ab9d1a-185f-40cc-97d9-e1177019a70b"
                        }
                    }
                };
                await graphClient.Groups["269b55c3-1d69-47b1-914b-0a251c8ac29e"].Request().UpdateAsync(group);

                return "Good";
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return "not Good";
            }
        }
    }
}
