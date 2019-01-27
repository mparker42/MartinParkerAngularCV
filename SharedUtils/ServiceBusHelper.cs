using MartinParkerAngularCV.SharedUtils.Constants;
using MartinParkerAngularCV.SharedUtils.Enums;
using MartinParkerAngularCV.SharedUtils.Interfaces;
using MartinParkerAngularCV.SharedUtils.Models.Configuration;
using MartinParkerAngularCV.SharedUtils.Models.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MartinParkerAngularCV.SharedUtils
{
    public class ServiceBusHelper
    {
        private readonly IOptions<ServiceBusConfiguration> _config;
        private ServiceBusConfiguration Config { get { return _config.Value; } }

        private readonly KeyVaultHelper _keyVaultHelper;

        public ServiceBusHelper(IOptions<ServiceBusConfiguration> config, KeyVaultHelper keyVaultHelper)
        {
            _config = config;
            _keyVaultHelper = keyVaultHelper;
        }

        private async Task<string> GetServiceBusConnectionString()
        {
            return await _keyVaultHelper.GetSecret(Config.ServiceBusSecretURL);
        }

        private string GetName(ServiceBusTopic topic)
        {
            switch (topic)
            {
                case ServiceBusTopic.ResetTranslationsCache:
                    return "resettranslationscache";
                default:
                    throw new NotImplementedException("This topic does not have a defined name yet.");
            }
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        public async Task Subscribe(ISerivceBusTopicSubscription subscriptionModel)
        {
            var client = new ManagementClient(await GetServiceBusConnectionString());

            string topicName = GetName(subscriptionModel.Topic),
                subscriptionName = $"{Environment.MachineName}_{subscriptionModel.Context}";

            if (await client.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                await client.DeleteSubscriptionAsync(topicName, subscriptionName);
            }

            await client.CreateSubscriptionAsync(new SubscriptionDescription(topicName, subscriptionName));

            SubscriptionClient subscriptionClient = new SubscriptionClient(await GetServiceBusConnectionString(), topicName, subscriptionName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = true
            };

            switch (subscriptionModel.Topic)
            {
                case ServiceBusTopic.ResetTranslationsCache:
                    ResetTranslationsCacheSubsriptionRequirements castedModel = (ResetTranslationsCacheSubsriptionRequirements)subscriptionModel;

                    if (castedModel == null)
                        throw new InvalidOperationException("Incorrect model provided for ResetTranslationsCache topic");

                    // Register the function that processes messages.
                    subscriptionClient.RegisterMessageHandler(delegate (Message message, CancellationToken cancellationToken)
                    {
                        string bodyString = Encoding.UTF8.GetString(message.Body);
                        ResetTranslationsCacheMessage castedMessage = JsonConvert.DeserializeObject<ResetTranslationsCacheMessage>(bodyString);

                        if (castedMessage == null)
                            throw new InvalidOperationException("Incorrect message passed to ResetTranslationsCache topic. Actual Message: " + bodyString);

                        foreach (string cacheName in castedMessage.CacheNames)
                            castedModel.DistributedCache.Remove(TranslationConstants._cachePrefix + cacheName);

                        return Task.CompletedTask;
                    }, messageHandlerOptions);

                    break;
            }
        }

        public async Task SendMessage(IServiceBusTopicMessage messageModel)
        {
            TopicClient topicClient = new TopicClient(await GetServiceBusConnectionString(), GetName(messageModel.Topic));

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)));

            await topicClient.SendAsync(message);

            await topicClient.CloseAsync();
        }
    }
}