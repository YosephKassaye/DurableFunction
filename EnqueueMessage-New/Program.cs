using Azure.Messaging.ServiceBus;
using Common;
using System;
using System.Net.Http;
using System.Text;

namespace EnqueueMessage_New
{
    class Program
    {
        private static string connection_string = "Endpoint=sb://yayobegsrvbus.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=dFESak7mwqlQw6Hr6wtif8eqTGxMnc3Nftuy1QEESLE=;EntityPath=yayobegtopic";
        private static string topic_name = "yayobegtopic";

        static void Main(string[] args)
        {

            for (int i = 1212; i < 1220; i++)
            {

         
            RequestObject rateObject = new RequestObject
            {
                RequestId = i,
                RequestTypeId = "Req102",
                SystemTypeId = i%2==0? "EASI": "FIS",
                CreatedDate = DateTime.Now.ToString()
            };

            ServiceBusClient _client = new ServiceBusClient(connection_string);
            ServiceBusSender _sender = _client.CreateSender(topic_name);



            ServiceBusMessage _message = new ServiceBusMessage(rateObject.ToString());
            _message.ContentType = "application/json";
            _sender.SendMessageAsync(_message).GetAwaiter().GetResult();
                Console.WriteLine("Messages Sent Succesfully");
   }

        }
    }
}
