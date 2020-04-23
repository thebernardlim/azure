using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Newtonsoft.Json.Linq;

namespace CheckADFPipelineStatus
{
    public static class GetStatusByNameAndRunId
    {
        /// <summary>
        /// Gets the status of a data factory pipeline by name and execution run id.
        /// </summary>
        [FunctionName("GetStatusByNameAndRunId")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //Get body values
            string tenantId = req.Query["tenantId"];
            string applicationId = req.Query["applicationId"];
            string authenticationKey = req.Query["authenticationKey"];
            string subscriptionId = req.Query["subscriptionId"];
            string resourceGroup = req.Query["resourceGroup"];
            string factoryName = req.Query["factoryName"];
            string pipelineName = req.Query["pipelineName"];
            string runId = req.Query["runId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            tenantId = tenantId ?? data?.tenantId;
            applicationId = applicationId ?? data?.applicationId;
            authenticationKey = authenticationKey ?? data?.authenticationKey;
            subscriptionId = subscriptionId ?? data?.subscriptionId;
            resourceGroup = resourceGroup ?? data?.resourceGroup;
            factoryName = factoryName ?? data?.factoryName;
            pipelineName = pipelineName ?? data?.pipelineName;
            runId = runId ?? data?.runId;

            //Check body for values
            if (
                tenantId == null ||
                applicationId == null ||
                authenticationKey == null ||
                subscriptionId == null ||
                factoryName == null ||
                pipelineName == null ||
                runId == null
                )
            {
                return new BadRequestObjectResult("Invalid request body, value missing.");
            }

            //Create a data factory management client
            var context = new AuthenticationContext("https://login.windows.net/" + tenantId);
            ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            AuthenticationResult result = context.AcquireTokenAsync(
                "https://management.azure.com/", cc).Result;
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            var client = new DataFactoryManagementClient(cred)
            {
                SubscriptionId = subscriptionId
            };

            //Get pipeline status with provided run id
            PipelineRun pipelineRun;
            string pipelineStatus = String.Empty;
            string outputString;

            pipelineRun = client.PipelineRuns.Get(resourceGroup, factoryName, runId);
            pipelineStatus = pipelineRun.Status;

            //Prepare output
            outputString = "{ \"PipelineName\": \"" + pipelineName + "\", \"RunIdUsed\": \"" + runId + "\", \"Status\": \"" + pipelineRun.Status + "\" }";
            JObject json = JObject.Parse(outputString);

            return new OkObjectResult(json);
        }
    }
}
