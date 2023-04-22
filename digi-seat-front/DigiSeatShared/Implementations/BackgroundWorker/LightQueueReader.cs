using DigiSeatShared.Implementations.Lights;
using DigiSeatShared.Interfaces;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared.Implementations.BackgroundWorker
{
    public static class LightQueueReader
    {
        private readonly static string _connection = @"Endpoint=sb://digiseatlightqueue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Eu0Qr6xe0YWZGQfmbDD2B9OtTAhizY7fd/PLmlkpXBU=";
        private static ILightIntegration _lightIntegration = new PlayBulb();

        public async static Task AddItem(LightQueueCommand obj)
        {
            var queueClient = new QueueClient(_connection, "light-restaurant-1");
            var content = JsonConvert.SerializeObject(obj);
            var message = new Message(Encoding.UTF8.GetBytes(content));
            await queueClient.SendAsync(message);
        }

        public async static Task StartReader()
        {
            var queueClient = new QueueClient(_connection, "light-restaurant-1");
            queueClient.RegisterMessageHandler(async (message, token) =>
            {
                var command = JsonConvert.DeserializeObject<LightQueueCommand>(Encoding.UTF8.GetString(message.Body));
                if (command.TurnOn)
                {
                    await _lightIntegration.TurnOn(command.Address, command.Color);
                }
                else
                {
                    await _lightIntegration.TurnOff(command.Address);
                }

                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            },
            async (exceptionEvent) =>
            {
                Console.WriteLine("Exception = " + exceptionEvent.Exception);
            });
        }
    }
}
