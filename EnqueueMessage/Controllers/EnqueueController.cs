using Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EnqueueMessage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnqueueController : ControllerBase
    {

        [HttpPost("PostMessageToServiceBusQueue")]
        public static void PostMessageToServiceBusQueue(RequestObject requestObject)
        {
            TimeSpan ts = new TimeSpan(0, 0, 90);
            var ServiceBusNamespaceURL = "sb://demoazure.servicebus.windows.net";
            var ServiceBusQueueURL = "https://demoazure.servicebus.windows.net/demoqueue/messages";
            var SASKeyName = "IntegrationKey";
            var SASKeyValue = "qkWTne2zdLQvJv6qp2+G1eqNau7XGfZZlZAEQTV9Y+A=";
            string sasToken = clsCommon.GetSASToken(ServiceBusNamespaceURL, SASKeyName, SASKeyValue, ts);

            RequestObject rateObject = new RequestObject
            {
                RequestId=requestObject.RequestId.ToString(),
                RequestTypeId=requestObject.RequestTypeId.ToString(),
                SystemTypeId=requestObject.SystemTypeId.ToString(),
                CreatedDate = requestObject.CreatedDate == null ? string.Empty : requestObject.CreatedDate.ToString()               
            };
            var json = clsCommon.SerializeToJsonString(rateObject);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", sasToken);
            var Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(ServiceBusQueueURL, Content);
            response.Wait();
        }
        }
}
