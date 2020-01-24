using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace WaterSprinklerIOTGenerator
{
    class Program
    {
        private static EventHubClient eventHubClient;
        private const string EventHubConnectionString = "Endpoint=sb://bl-farms.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ad/958StECqcWa98BTCMQ0FHTTjk/7mP84mj2Iuwh9o=";
        private const string EventHubName = "water-sprinklers";


        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(50);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            for (var i=0; i<numMessagesToSend; i++)
            {
                try
                {
                    var sprinkleEvent = new { field_id = i, amount = i*50 };

                    var message = JsonConvert.SerializeObject(sprinkleEvent);
                    Console.WriteLine($"Sending message: {message}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(1000);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }

    
    }
}
 