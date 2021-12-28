using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Onboard_Request_DFunc
{
    public static class OnboardTasks
    {
        [FunctionName("OnboardTasks")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context )
        {
            var outputs = new List<string>();
            try
            {
                var compressedInput = context.GetInput<string>();
                // Replace "hello" with the name of your Durable Activity Function.
                var request = JsonConvert.DeserializeObject<RequestObject>(context.InstanceId);
                if(request.SystemTypeId=="EASI")
                    outputs.Add(await context.CallActivityAsync<string>("OnboardEASITasks", request));
                else
                    outputs.Add(await context.CallActivityAsync<string>("OnboardFISTasks", request));
            }
            catch (System.Exception ex )
            {

                var message = ex.Message;
                throw;
            }

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("OnboardEASITasks")]
        public static string OnboardEASITasks([ActivityTrigger] RequestObject requestObject, ILogger log)
        {

            log.LogInformation($"EASI Task Handler {requestObject.SystemTypeId}.");
            return $"Hello EASI {requestObject.RequestId}!";
        }
        [FunctionName("OnboardFISTasks")]
        public static string OnboardFISTasks([ActivityTrigger] RequestObject requestObject, ILogger log)
        {

            log.LogInformation($"FIS Task Handler {requestObject.SystemTypeId}.");
            return $"Hello FIS {requestObject.RequestId}!";
        }

        //[FunctionName("OnboardEASITasks")]
        //public static async Task OnboardEASITasks([ActivityTrigger] RequestObject requestObject, ILogger log)
        //{
        //    var cosmosUrl = "https://yayobecosmosdb.documents.azure.com:443/";
        //    var cosmoskey = "LVFheqUyNbqF6FweueKMntxVLZPHJRcDOc8m65kJIxQicYh9oHr4Vee0RfdyV2hQFowFxODdhcPEODTFcpxRcg==";
        //    var databaseName = "TaskHandler";
        //    CosmosClient client = new CosmosClient(cosmosUrl, cosmoskey);
        //    Database database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        //    Container container = await database.CreateContainerIfNotExistsAsync("EasiTaskHandler", "/RequestId", 400);

        //    var taskhandler = await container.CreateItemAsync(requestObject);

        //    log.LogInformation($"EASI Task Handler {requestObject.SystemTypeId}.");
        //    //return $"Hello EASI {requestObject.RequestId}!";
        //}

        [FunctionName("OnboardTasks_HttpStart")]
        public static  async Task  HttpStart(
           
             [ServiceBusTrigger("yayobegtopic", "Libarytopic", Connection = "AzureWebJobsServiceBus")] string message,
            
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
         
                      
           
            string instanceId = await starter.StartNewAsync("OnboardTasks", message);
        }
    }
}