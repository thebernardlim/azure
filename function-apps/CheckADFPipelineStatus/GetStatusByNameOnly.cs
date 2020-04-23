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
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Web.Mvc.Filters;
using Microsoft.Azure;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;

namespace CheckADFPipelineStatus
{
    public static class GetStatusByNameOnly
    {
        /// <summary>
        /// Gets the status of a data factory pipeline by name assuming the
        /// pipeline was executed within a recent time period.
        /// </summary>
        [FunctionName("GetStatusByNameOnly")]
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

            int daysOfRuns = int.Parse(Environment.GetEnvironmentVariable("DefaultDaysForPipelineRuns"));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            tenantId = tenantId ?? data?.tenantId;
            applicationId = applicationId ?? data?.applicationId;
            authenticationKey = authenticationKey ?? data?.authenticationKey;
            subscriptionId = subscriptionId ?? data?.subscriptionId;
            resourceGroup = resourceGroup ?? data?.resourceGroup;
            factoryName = factoryName ?? data?.factoryName;
            pipelineName = pipelineName ?? data?.pipelineName;

            //Check body for values
            if (
                tenantId == null ||
                applicationId == null ||
                authenticationKey == null ||
                subscriptionId == null ||
                factoryName == null ||
                pipelineName == null
                )
            {
                return new BadRequestObjectResult("Invalid request body, value missing.");
            }

            //Create a data factory management client
            var context = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext("https://login.windows.net/" + tenantId);
            ClientCredential cc = new ClientCredential(applicationId, authenticationKey);
            AuthenticationResult result = context.AcquireTokenAsync("https://management.azure.com/", cc).Result;
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            var client = new DataFactoryManagementClient(cred)
            {
                SubscriptionId = subscriptionId
            };

            //Get pipeline status
            PipelineRun pipelineRuns; //used to find latest pipeline run id
            PipelineRun pipelineRun;  //used to get the status of the last pipeline
            string pipelineStatus = String.Empty;
            string runId = String.Empty;
            string outputString;
            DateTime today = DateTime.Now;
            DateTime lastWeek = DateTime.Now.AddDays(-daysOfRuns);

            /*
            * https://docs.microsoft.com/en-us/rest/api/datafactory/pipelineruns/querybyfactory#runqueryfilteroperand
            */

            //Query data factory for pipeline runs
            IList<string> pipelineList = new List<string> { pipelineName };
            IList<RunQueryFilter> moreParams = new List<RunQueryFilter>();

            moreParams.Add(new RunQueryFilter
            {
                Operand = RunQueryFilterOperand.PipelineName,
                OperatorProperty = RunQueryFilterOperator.Equals,
                Values = pipelineList
            });

            RunFilterParameters filterParams = new RunFilterParameters(lastWeek, today, null, moreParams, null);

            var requiredRuns = client.PipelineRuns.QueryByFactory(resourceGroup, factoryName, filterParams);
            var enumerator = requiredRuns.Value.GetEnumerator();

            //Get latest run id
            for (bool hasMoreRuns = enumerator.MoveNext(); hasMoreRuns;)
            {
                pipelineRuns = enumerator.Current;
                hasMoreRuns = enumerator.MoveNext();

                if (!hasMoreRuns && pipelineRuns.PipelineName == pipelineName)
                {
                    //Get status for run id
                    runId = pipelineRuns.RunId;
                    pipelineStatus = client.PipelineRuns.Get(resourceGroup, factoryName, runId).Status;
                }
            }

            //Prepare output
            outputString = "{ \"PipelineName\": \"" + pipelineName + "\", \"RunIdUsed\": \"" + runId + "\", \"Status\": \"" + pipelineStatus + "\" }";
            JObject json = JObject.Parse(outputString);

            return new OkObjectResult(json);
        }
    }
}
