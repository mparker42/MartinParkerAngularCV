using MartinParkerAngularCV.SharedUtils.Enums;
using MartinParkerAngularCV.SharedUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MartinParkerAngularCV.SharedUtils.Models.ServiceBus
{
    public class ResetTranslationsCacheMessage : IServiceBusTopicMessage
    {
        public ResetTranslationsCacheMessage(ServiceBusTopic topic, List<string> cacheNames)
        {
            Topic = topic;
            CacheNames = cacheNames;
        }

        public ServiceBusTopic Topic { get; }
        public List<string> CacheNames { get; }
    }
}
