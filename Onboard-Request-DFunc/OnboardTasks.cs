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
                    outputs.Add(await context.CallActivityAsync<string>("OnboardEASITasks", request.RequestId));
                else
                    outputs.Add(await context.CallActivityAsync<string>("OnboardFISTasks", request.RequestId));
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
        public static string OnboardEASITasks([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"EASI Task Handler {name}.");
            return $"Hello EASI {name}!";
        }
        [FunctionName("OnboardFISTasks")]
        public static string OnboardFISTasks([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"FIS Task Handler {name}.");
            return $"Hello FIS {name}!";
        }

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